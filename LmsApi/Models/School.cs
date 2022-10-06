using System.ComponentModel.DataAnnotations.Schema;

namespace LmsApi.Models
{
    [Table("schools")]
    public class School
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<Student> Students { get; set; } = new();
        public List<Tutor> Tutors { get; set; } = new();

        public List<Class> Classes { get; set; } = new();
    }
}
