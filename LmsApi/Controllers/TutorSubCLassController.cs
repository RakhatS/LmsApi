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
    public class TutorSubCLassController : ControllerBase
    {

        private readonly Context _context;

        public TutorSubCLassController(Context context)
        {
            _context = context;
        }

        [HttpPost("addSubject")]
        public async Task<ActionResult<TutorSubCLass>> postAddSubjecToTutor(TutorSubCLass request)
        {
            _context.TutorSubCLasses.Add(request);
            await _context.SaveChangesAsync();
            return Ok();

        }
        //[Authorize(Roles = "Admin")]
        //[HttpPut("edit")]
        //public async Task<ActionResult<TutorSubCLass>> putEditSubjecToTutor(TutorSubCLass request)
        //{
        //    var t = await _context.TutorSubCLasses.FindAsync(request.Id);
        //    if (t == null)
        //        return NotFound();
        //    t.TutorId = request.TutorId;
        //    t.ClassId = request.ClassId;
        //    t.SubjectId = request.SubjectId;
        //    await _context.SaveChangesAsync();
        //    return Ok();
        //}


        [HttpGet("get/{id}")]
        public async Task<ActionResult<TutorSubCLass>> getTutorSubClassById(int id)
        {
            var t = await _context.TutorSubCLasses.Include(x => x.Subject).Include(x => x.Class).Where(x => x.Id == id).Select(x => new TutorSubCLass
            {
                Id = x.Id,
                Class = x.Class,
                Subject = x.Subject,
                ClassId = x.ClassId,
                TutorId = x.TutorId
            }).FirstAsync();
            if (t == null)
                return NotFound();
            return Ok(t);
        }


    }
}
