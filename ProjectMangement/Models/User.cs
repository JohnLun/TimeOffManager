using System.ComponentModel.DataAnnotations.Schema;

namespace TimeOffManager.Models
{
    [Table("users")]
    public class User
    {
        [Column("user_id")]
        public int UserId { get; set; }
        
        [Column("email")]
        public string Email { get; set; } = null!;

        [Column("password")]
        public string Password { get; set; } = null!;

        [Column("f_name")]
        public string FirstName {  get; set; } = null!;

        [Column("l_name")]
        public string LastName { get; set; } = null!;

        [Column("pto_hours")]
        public int PTOHours { get; set; } = 160;

        [Column("ci_hours")]
        public int CIHours { get; set; } = 16;

        [Column("fh_hours")]
        public int FHHours { get; set; } = 8;

        public List<Request> Requests { get; set; } = [];
    }
}
