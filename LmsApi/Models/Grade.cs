using System.ComponentModel.DataAnnotations.Schema;

namespace LmsApi.Models
{
    [Table("grades")]
    public class Grade
    {
        public int Id { get; set; }

        public int Score { get; set; }

        public int StudentId { get; set; }
        public Student? Student { get; set; }
        public int TutorSubCLassId { get; set; }
        public TutorSubCLass? TutorSubCLass { get; set; }
        public int TutorId { get; set; }
        public Tutor? Tutor { get; set; }

        public DateTime? GradeDate { get; set; }
    }
}
