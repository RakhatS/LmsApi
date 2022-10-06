using Common;
using LmsApi.Authorization;
using LmsApi.Data;
using LmsApi.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Writers;
using System.IdentityModel.Tokens.Jwt;
using System.IO.Compression;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LmsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly Context _context;
        private readonly IOptions<AuthOptions> authOptions;

        public AuthController(Context context, IOptions<AuthOptions> authOptions)
        {
            this._context = context;
            this.authOptions = authOptions;
        }

        [HttpPost("login")]
        public IActionResult login([FromBody] Login request)
        {

            var user = AuthenticateUser(request.Email, request.Password);
            if (user != null)
            {
                var token = GenerateJWT(user);

                return Ok(new
                {
                    access_token = token
                });
            }
            return Unauthorized();
        }

        [HttpPost("create")]
        public async Task<ActionResult<Account>> createAcc(Account request)
        {

            if (await _context.accounts.AnyAsync(x => x.Email == request.Email))
                return BadRequest("The email address already exists.");
            request.Password = hashPassword(request.Password);
            _context.accounts.Add(request);
            await _context.SaveChangesAsync();
            return Ok();
        }



        [HttpGet("myprofile")]
        [Authorize]
        public async Task<ActionResult<Account>> getMyAccount()
        {
         
            var uId = int.Parse(User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value);



            var acc = await _context.accounts.Include(x => x.Student).Include(x => x.Tutor)
                .Where(x => x.Id == uId).Select(x => new Account
                {
                    Id = x.Id,
                    Email = x.Email,
                    Password = x.Password,
                    Tutor = x.Tutor,
                    Roles = x.Roles,
                    Student = x.Student,
                 
                }).FirstAsync();
            if (acc == null)
                return NotFound();

            return Ok(acc);

        }





        [HttpGet("get/{id}")]
        public async Task<ActionResult<Account>> getAccount(int id)
        {
            var account = await _context.accounts.Include(x => x.Tutor)
                .Include(x => x.Student)
                    .Where(x => x.Id == id).Select(x => new Account
                    {
                        Id = x.Id,
                        Email = x.Email,
                        Password = x.Password,
                        Tutor = x.Tutor,
                        Roles = x.Roles,
                        Student = x.Student
                       
                    }).FirstAsync();
            if (account == null) return BadRequest("Account not found");
           
            return account;
        }


        [HttpGet("emailIsExist")]
        public async Task<ActionResult<bool>> emailIsExist(string email)
        {
            var acc = await _context.accounts.Where(x => x.Email == email).FirstOrDefaultAsync();
            if (acc == null)
                return Ok(false);
            else 
                return Ok(true);
        }

        private Account AuthenticateUser(string email, string password)
        {
            var hassPass = hashPassword(password);
            var a = _context.accounts.Where(x => x.Email == email && x.Password == hassPass).First();
            if (a == null)
                return null;
            return a;

        }

        private string GenerateJWT(Account user)
        {
            var authParams = authOptions.Value;

            var securityKey = authParams.GetSymmetricSecurityKey();
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString())
            };

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim("role", role.ToString()));
            }

            var token = new JwtSecurityToken(authParams.Issuer,
                authParams.Audience,
                claims,
                expires: DateTime.Now.AddSeconds(authParams.TokenLifetime),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);

        }


        private string hashPassword(string password)
        {
            MD5 md5 = MD5.Create();
            byte[] b = Encoding.ASCII.GetBytes(password);
            byte[] hash = md5.ComputeHash(b);

            StringBuilder sb = new StringBuilder();
            foreach (var a in hash)
                sb.Append(a.ToString("X2"));
            return Convert.ToString(sb);

        }
      

    }
}
