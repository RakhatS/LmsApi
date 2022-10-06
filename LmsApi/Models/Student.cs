using System.ComponentModel.DataAnnotations.Schema;

namespace LmsApi.Models
{


    [Table("students")]
    public class Student
    {
        public int Id { get; set; }

        public string Surname { get; set; }
        public string Name { get; set; }
        public string? Patronymic { get; set; }

        public int AccountId { get; set; }
        public Account? Account { get; set; }


        public int SchoolId { get; set; }
        public School? School { get; set; }


        public int ClassId { get; set; }
        public Class? Class { get; set; }

        public List<Grade> Grades { get; set; } = new();

      
    }
}
