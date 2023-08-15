using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace com.moviebookingapp.usermicroservice.Models
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        [BsonElement("email")]
        public string Email { get; set; }
    }
}
