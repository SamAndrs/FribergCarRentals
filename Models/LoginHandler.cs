using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FribergRentalCars.Models
{
    public class LoginHandler
    {
        [Key]
        public int HandlerId { get; set; }

        [ForeignKey("User")]
        public int? UserId { get; set; }

        [ForeignKey("Admin")]
        public int AdminId { get; set; }

        public string? UserName { get; set; }

        public string? Password { get; set; }       
    }
}
