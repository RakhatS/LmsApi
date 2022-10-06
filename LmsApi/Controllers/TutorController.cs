using LmsApi.Data;
using LmsApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;

namespace LmsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TutorController : ControllerBase
    {
        private readonly Context _context;

        public TutorController(Context context)
        {
            this._context = context;
        }
        [Authorize]
        [HttpGet("get/{id}")]
        public async Task<ActionResult<Tutor>> getTutorById(int id)
        {
            var t = await _context.Tutors.Include(x => x.Account).Where(x => x.Id == id).Select(x => new Tutor
            {
                Id = x.Id,
                Name = x.Name,
                Surname = x.Surname,
                Patronymic = x.Patronymic,
                Subject = x.Subject,
                Account = x.Account
            }).FirstAsync();
            if (t == null) return BadRequest("Tutor not found");
            return Ok(t);
        }

        [Authorize]
        [HttpGet("getList")]
        public async Task<ActionResult<IList<Tutor>>> getTutorList()
        {
            var t = await _context.Tutors.Include(x => x.Account).Select(x => new Tutor
            {
                Id = x.Id,
                Name = x.Name,
                Surname = x.Surname,
                Patronymic = x.Patronymic,
                Subject = x.Subject,
                Account = x.Account
            }).ToListAsync();
            if (t == null) return BadRequest("Tutors not found");
            return Ok(t);
        }


        [HttpPut("addSubject/{id}")]
        public async Task<ActionResult<Tutor>> putAddSubjecToTutor(int id, [FromBody]int sub_id)
        {
            var t = await _context.Tutors.FindAsync(id);
            var s = await _context.Subjects.FindAsync(sub_id);
            if (t == null || s == null) return BadRequest("Tutor/Subject not found");
            t.SubjectId = s.Id;
            t.Subject = s;
            await _context.SaveChangesAsync();
            return Ok();

        }

        [HttpPost("rate")]
        public async Task<ActionResult<Grade>> postGradeToSubject(Grade request)
        {
            request.GradeDate = DateTime.Now;
            _context.Grades.Add(request);
            await _context.SaveChangesAsync();
            return Ok("Success");
        }

        [Authorize(Roles = "Tutor")]
        [HttpPut("rate/edit")]
        public async Task<ActionResult<Grade>> putEditGradeToSubject(Grade request)
        {
            var s = await _context.Grades.FindAsync(request.Id);
            if (s == null)
                return NotFound();
            s.Score = request.Score;
            await _context.SaveChangesAsync();
            return Ok("Success");
        }

        [HttpGet("getSubjectByTutor/{id}")]
        public async Task<ActionResult<IList<TutorSubCLass>>> getTutorSubClTutor(int id)
        {
            var t = await _context.TutorSubCLasses.Include(x => x.Class).Where(x => x.TutorId == id).Select(x => new TutorSubCLass
            {
                Id = x.Id,
                SubjectId = x.SubjectId,
                Subject = x.Subject,
                Class = x.Class,
                ClassId = x.ClassId

            }).ToListAsync();
            if (t.IsNullOrEmpty())
                return NotFound();
            return Ok(t);
        }
        [Authorize(Roles = "Tutor")]
        [HttpPost("saveGrades")]
        public async Task<ActionResult<IList<Grade>>> saveGrades(IList<Grade> grades)
        {
            foreach(var grade in grades)
            {
                var g = await _context.Grades.FindAsync(grade.Id);
                if(g != null)
                {
                    g.Score = grade.Score;
                    await _context.SaveChangesAsync();
                }
                else
                { 
                    grade.GradeDate = DateTime.Now;
                    _context.Add(grade);
                    await _context.SaveChangesAsync();
                }
            }
            return Ok();
        }
        [Authorize(Roles = "Tutor")]
        [HttpDelete("deleteGrade/{id}")]
        public async Task<ActionResult<Grade>> deleteGrages(int id)
        {
            var t = await _context.Grades.FindAsync(id);
            if(t == null)
                return NotFound();
            _context.Remove(t);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("getSimple/{id}")]
        public async Task<ActionResult<Tutor>> getFullNameTutor(int id)
        {
            var t = await _context.Tutors.FindAsync(id);
            if (t == null)
                return NotFound();
            if (t.Patronymic == null)
                t.Patronymic = "";
            return Ok(t);
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("save")]
        public async Task<ActionResult<Tutor>> putSaveStudentEdit(Tutor request)
        {
            var tutor = await _context.Tutors.FindAsync(request.Id);
            if (tutor == null)
                return BadRequest("Tutor not found");
            tutor.Name = request.Name;
            tutor.Surname = request.Surname;
            tutor.Patronymic = request.Patronymic;
            tutor.SchoolId = request.SchoolId;
            tutor.SubjectId = request.SubjectId;
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
