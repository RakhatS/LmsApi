using System.ComponentModel.DataAnnotations.Schema;

namespace LmsApi.Models
{



    [Table("tutor_sub_cl")]
    public class TutorSubCLass
    {
        public int Id { get; set; }


        public int SubjectId { get; set; }
        public Subject? Subject { get; set; }


        public int ClassId { get; set; }
        public Class? Class { get; set; }


        public int TutorId { get; set; }
        public Tutor? Tutor { get; set; }


    }
}
