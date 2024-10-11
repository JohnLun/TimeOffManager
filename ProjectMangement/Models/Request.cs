using System.ComponentModel.DataAnnotations.Schema;

namespace TimeOffManager.Models
{
    [Table("requests")]
    public class Request
    {
        [Column("request_id")]
        public int RequestId { get; set; }

        [Column("request_type")]
        public string RequestType { get; set; } = null!;

        [Column("s_date")]
        public DateOnly StartDate { get; set; }

        [Column("e_date")]
        public DateOnly EndDate { get; set; }

        [Column("reason")]
        public string Reason { get; set; } = null!;

        [Column("status")]
        public string Status { get; set; } = "Pending";



        [ForeignKey(nameof(UserId))]
        [Column("user_id")]
        public int UserId { get; set; }
        
        public User User { get; set; } = null!;
        
    }
}
