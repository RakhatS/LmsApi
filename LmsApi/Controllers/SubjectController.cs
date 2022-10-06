using LmsApi.Data;
using LmsApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LmsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : ControllerBase
    {


        private readonly Context _context;

        public SubjectController(Context context)
        {
            this._context = context;
        }
     
        [HttpPost("add")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult<Subject>> addSubject(Subject request)
        {
            _context.Add(request);
            await _context.SaveChangesAsync();
            return Ok(request);

        }

        [HttpGet("getList")]
        public async Task<ActionResult<IList<Subject>>> getSubjectList()
        {
            return Ok(await _context.Subjects.Include(x => x.Tutors).Select(x => new Subject
            {
                Id = x.Id,
                Tutors = x.Tutors,
                Name = x.Name
            }).ToListAsync());
        }

        [HttpGet("get/{id}")]
        public async Task<ActionResult<Subject>> getSubjectById(int id)
        {
            var c = await _context.Subjects.FindAsync(id);
            if (c == null) return BadRequest("Subject not found");
            return Ok(c);
        }


        [HttpPut("save")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Subject>> saveSubject(Subject request)
        {
            var c = await _context.Subjects.FindAsync(request.Id);
            if (c == null) return BadRequest("Subject not found");
            c.Name = request.Name;

            await _context.SaveChangesAsync();
            return Ok(c);
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<Subject>> deleteSubjectById(int id)
        {
            var c = await _context.Subjects.FindAsync(id);
            if (c == null) return BadRequest("Subject not found");

            _context.Subjects.Remove(c);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
