using System.ComponentModel.DataAnnotations.Schema;

namespace LmsApi.Models
{

    [Table("tutors")]
    public class Tutor
    {
        public int Id { get; set; }

        public string Surname { get; set; }
        public string Name { get; set; }
        public string? Patronymic { get; set; }

        public int AccountId { get; set; }
        public Account? Account { get; set; }


        public int SubjectId { get; set; }
        public Subject? Subject { get; set; }

        public List<TutorSubCLass> TutorSubCLasses { get; set; } = new();

        public int SchoolId { get; set; }
        public School? School { get; set; }

    }
}
