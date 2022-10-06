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
    public class StudentController : ControllerBase
    {


        private readonly Context _context;

        public StudentController(Context context)
        {
            this._context = context;
        }

        [HttpGet("get/{id}")]
        public async Task<ActionResult<Student>> getStudentById(int id)
        {
            var t = await _context.Students.Include(x => x.Account).Include(x => x.Class).ThenInclude(x => x.TutorSubCLasses)
                .Where(x => x.Id == id).Select(x => new Student
                {
                    Id = x.Id,
                    Name = x.Name,
                    Surname = x.Surname,
                    Patronymic = x.Patronymic,
                    Account = x.Account,
                    Class = x.Class,
                    SchoolId = x.SchoolId,


                }).FirstAsync();
            if (t == null) return BadRequest("Student not found");
            return Ok(t);

        }

        [Authorize(Roles = "Tutor,Admin")]
        [HttpGet("getList/")]
        public async Task<ActionResult<IList<Student>>> getStudentList()
        {
            var t = await _context.Students.Include(x => x.School).Select(x => new Student
            {
                Id = x.Id,
                Name = x.Name,
                Surname = x.Surname,
                Patronymic = x.Patronymic,
                School = x.School,
                ClassId = x.ClassId
            }).ToListAsync();
            if (t == null) return BadRequest("Students not found");
            return Ok(t);
        }


        [Authorize]
        [HttpGet("getGrades/{stud_id}")]
        public async Task<ActionResult<IList<Grade>>> getStudentGrades(int stud_id)
        {
            var g = await _context.Grades.Where(x => x.StudentId == stud_id).ToListAsync();
            if (g == null) return NotFound();
            return Ok(g);
        }
        [Authorize]
        [HttpGet("getGrades/{stud_id}/{tut_id}")]
        public async Task<ActionResult<IList<Grade>>> getStudentGradesBySubID(int stud_id, int tut_id)
        {
            var g = await _context.Grades.Where(x => x.StudentId == stud_id && x.TutorSubCLassId == tut_id).ToListAsync();
            if (g == null) return NotFound();
            g.Sort((x, y) => DateTime.Compare(x.GradeDate.Value, y.GradeDate.Value));
            return Ok(g);
        }
        [Authorize]
        [HttpGet("getByClassId/{id}")] 
        public async Task<ActionResult<IList<Student>>> getByClassId(int id)
        {
            var s = await _context.Students.Where(x => x.ClassId == id).Select(x => new Student
            {
                Id = x.Id,
                Name = x.Name,
                Patronymic = x.Patronymic,
                Surname = x.Surname,
                ClassId = x.ClassId,
                SchoolId = x.SchoolId

            }).ToListAsync();
            if (s == null)
                return NotFound();
            return Ok(s);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("save")]
        public async Task<ActionResult<Student>> putSaveStudentEdit(Student request)
        {
            var student = await _context.Students.FindAsync(request.Id);
            if (student == null)
                return BadRequest("Student not found");
            student.Name = request.Name;
            student.Surname = request.Surname;
            student.Patronymic = request.Patronymic;
            student.SchoolId = request.SchoolId;
            student.ClassId = request.ClassId;
            await _context.SaveChangesAsync();
            return Ok();
        }
    }

}
