using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LmsApi.Models
{
    [Table("accounts")]
    public class Account
    {
        public int Id { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }
        public Role[] Roles { get; set; }


        public Tutor? Tutor { get; set; }
      
        public Student? Student { get; set; }

    }

    public enum Role
    {
        Tutor,
        Student,
        Admin
    }
}
