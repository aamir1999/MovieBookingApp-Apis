using System.ComponentModel.DataAnnotations;

namespace com.moviebookingapp.usermicroservice.Models
{
    public class ResetPasswordRequest
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}
