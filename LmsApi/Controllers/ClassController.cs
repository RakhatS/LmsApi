using LmsApi.Data;
using LmsApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LmsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassController : ControllerBase
    {

        private readonly Context _context;

        public ClassController(Context context)
        {
            _context = context;
        }

        [HttpPost("add")]
        public async Task<ActionResult<Class>> addClass(Class request)
        {
            _context.Add(request);
            await _context.SaveChangesAsync();
            return Ok();

        }

        [HttpGet("getList")]
        public async Task<ActionResult<IList<Class>>> getClassList()
        {
            return Ok(await _context.Classes.ToListAsync());
        }

        [HttpGet("get/{id}")]
        public async Task<ActionResult<Class>> getClassById(int id)
        {
            var c = await _context.Classes.Include(x => x.TutorSubCLasses).Include(x => x.Students).Select(x => new Class
            {
                Id = id,
                Number = x.Number,
                Seria = x.Seria,
                Students = x.Students,
                TutorSubCLasses = x.TutorSubCLasses,
                SchoolId = x.SchoolId
            }).FirstAsync();
            if (c == null) return BadRequest("Class not found");
            return Ok(c);
        }
        [HttpGet("getSimple/{id}")]
        public async Task<ActionResult<Class>> getClassSimpleById(int id)
        {
            var c = await _context.Classes.Where(x => x.Id == id).Select(x => new Class
            {
                Id = id,
                Number = x.Number,
                Seria = x.Seria,         
            }).FirstAsync();
            if (c == null) return BadRequest("Class not found");
            return Ok(c);

        }

        [Authorize(Roles = "Admin")]
        [HttpPut("save")]
        public async Task<ActionResult<Class>> saveClass(Class request)
        {
            var c = await _context.Classes.FindAsync(request.Id);
            if (c == null) return BadRequest("Class not found");
            c.Number = request.Number;
            c.Seria = request.Seria;

            await _context.SaveChangesAsync();
            return Ok(c);
        }


        //[Authorize(Roles = "Admin")]
        //[HttpDelete("delete/{id}")]
        //public async Task<ActionResult<Class>> deleteClassById(int id)
        //{
        //    var c = await _context.Classes.FindAsync(id);
        //    if (c == null) return BadRequest("Class not found");

        //    _context.Classes.Remove(c);
        //    await _context.SaveChangesAsync();
        //    return Ok();
        //}

        [HttpPut("addStudent/{id}")]

        public async Task<ActionResult<Class>> addStudentToSchool([FromBody] int studendId, int id)
        {
            var cl = await _context.Classes.FindAsync(id);
            var student = await _context.Students.FindAsync(studendId);
            if (cl == null || student == null) return NotFound();
            if (student.SchoolId != cl.SchoolId)
                return BadRequest("Another school");

            cl.Students.Add(student);
            await _context.SaveChangesAsync();
            return Ok();
        }


        [HttpGet("getSubjectByClassId/{id}")]
        public async Task<ActionResult<List<TutorSubCLass>>> getT(int id)
        {
            var t = await _context.TutorSubCLasses.Include(x => x.Subject).Include(x => x.Class)
                .Where(x => x.ClassId == id).Select(x => new TutorSubCLass
                {
                   
                    Subject = x.Subject,
                    Class = x.Class,
                    TutorId = x.TutorId,
                    ClassId = x.ClassId,
                    SubjectId = x.SubjectId,
                    Id = x.Id
                }).ToListAsync();
            if (t == null)
                return NotFound();
            return Ok(t);
        }


    }
}
