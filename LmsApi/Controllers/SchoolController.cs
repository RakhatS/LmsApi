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
    public class SchoolController : ControllerBase
    {
        private readonly Context _context;
        
        public SchoolController(Context context)
        {
            this._context = context;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("add")]
        public async Task<ActionResult<School>> addSchool(School request)
        {
            _context.Add(request);
            await _context.SaveChangesAsync();
            return Ok(request);

        }

        [HttpGet("getList")]
        public async Task<ActionResult<IList<School>>> getSchoolList()
        {
            return Ok(await _context.Schools.Include(x => x.Classes).Select(x => new School
            {
                Id = x.Id,
                Name = x.Name,
                Classes = x.Classes,
                
            }).ToListAsync());
        }

        [HttpGet("get/{id}")]
        public async Task<ActionResult<School>> getSchoolById(int id)
        {
            var c = await _context.Schools.Include(x => x.Students).Include(x => x.Tutors).Where(x => x.Id == id).Select(x => new School
            {
                Id = x.Id,
                Name = x.Name,
                Tutors = x.Tutors,
                Students = x.Students
            }).FirstAsync();
            if (c == null) return BadRequest("School not found");
            return Ok(c);
        }
        [HttpGet("find/{id}")]
        public async Task<ActionResult<School>> getSimpleSchoolById(int id)
        {
            var c = await _context.Schools.FindAsync(id);
            if (c == null) return BadRequest("School not found");
            return Ok(c);
        }
        //[Authorize(Roles = "Admin")]
        //[HttpPut("save")]
        //public async Task<ActionResult<School>> saveSchool(School request)
        //{
        //    var c = await _context.Schools.FindAsync(request.Id);
        //    if (c == null) return BadRequest("School not found");
        //    c.Name = request.Name;            

        //    await _context.SaveChangesAsync();
        //    return Ok(c);
        //}

        //[HttpDelete("delete/{id}")]
        //public async Task<ActionResult<School>> deleteSchoolById(int id)
        //{
        //    var c = await _context.Schools.FindAsync(id);
        //    if (c == null) return BadRequest("School not found");

        //    _context.Schools.Remove(c);
        //    await _context.SaveChangesAsync();
        //    return Ok();
        //}



        [HttpPut("addStudent/{id}")]

        public async Task<ActionResult<School>> addStudentToSchool([FromBody]int studendId, int id)
        {
            var school = await _context.Schools.FindAsync(id);
            var student = await _context.Students.FindAsync(studendId);
            if (school == null || student == null) return NotFound();


            school.Students.Add(student);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("addTutor/{id}")]

        public async Task<ActionResult<School>> addTutorToSchool([FromBody]int tutorId, int id)
        {
            var school = await _context.Schools.FindAsync(id);
            var tutor = await _context.Tutors.FindAsync(tutorId);
            if (school == null || tutor == null) return NotFound();

            tutor.School = school;
            tutor.SchoolId = id;
            school.Tutors.Add(tutor);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
