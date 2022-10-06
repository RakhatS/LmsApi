using System.ComponentModel.DataAnnotations.Schema;

namespace LmsApi.Models
{

    [Table("classes")]
    public class Class
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Seria { get; set; }

        public List<Student> Students { get; set; } = new();

        public List<TutorSubCLass> TutorSubCLasses { get; set; } = new();

        public int SchoolId { get; set; }
        public School? School { get; set; }
    }
}
