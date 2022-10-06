using System.ComponentModel.DataAnnotations.Schema;

namespace LmsApi.Models
{
    [Table("subjects")]
    public class Subject
    {
        public int Id { get; set; }
        public string Name { get; set; }


        public List<Tutor> Tutors { get; set; } = new();
    }
}
