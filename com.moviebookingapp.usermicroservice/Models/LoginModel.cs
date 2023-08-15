using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace com.moviebookingapp.usermicroservice.Models
{
    public class LoginModel
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }
        [Required]
        [BsonElement("loginId")]
        public string LoginId { get; set; }
        [BsonElement("userName")]
        public string UserName { get; set; }
        [Required,EmailAddress]
        [BsonElement("email")]
        public string Email { get; set; }
        [BsonElement("password")]
        public string Password { get; set; }
    }
}
