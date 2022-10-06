using Common;
using LmsApi.Data;
using LmsApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LmsApi.Controllers
{
    [Route("api/Admin")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly Context _context;
        public AdminController(Context context, IOptions<AuthOptions> authOptions)
        {
            this._context = context;
            
        }
        
        [HttpDelete("delete/account/{id}")]
        public async Task<ActionResult<Account>> deleteAccount(int id)
        {
            var a = await _context.accounts.FindAsync(id);
            if (a == null)
                return NotFound();
            _context.accounts.Remove(a);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("getAccounts")]

        public async Task<ActionResult<IList<Account>>> getAccounts()
        {
            var account = await _context.accounts.Include(x => x.Tutor)
                .Include(x => x.Student).Select(x => new Account
                    {
                        Id = x.Id,
                        Email = x.Email,
                        Password = x.Password,
                        Tutor = x.Tutor,
                        Roles = x.Roles,
                        Student = x.Student

                    }).ToListAsync();
            if (account == null) return BadRequest("Accounts not found");

            return account;
        }
    }
}
