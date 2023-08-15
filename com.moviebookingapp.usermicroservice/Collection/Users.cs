using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace com.moviebookingapp.usermicroservice.Collection
{
    public class Users
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonElement("loginId")]
        public string LoginId { get; set; }
        [BsonElement("firstName")]
        public string FirstName { get; set; }
        [BsonElement("lastName")]
        public string LastName { get; set; }
        [BsonElement("userName")]
        public string UserName { get; set; }
        [BsonElement("email")]
        [EmailAddress]
        public string Email { get; set; }
        [BsonElement("password")]
        public string Password { get; set; }
        [BsonElement("contactNumber")]
        public string ContactNumber { get; set; }
        [BsonElement("role")]
        public string Role { get; set; }
        [BsonElement("resetToken")]
        public string? ResetToken { get; set; }
        [BsonElement("resetTokenExpires")]
        public DateTime? ResetTokenExpires { get; set; }
        [BsonElement("createdBy")]
        public string? CreatedBy { get; set; }
        [BsonElement("createdOn")]
        public DateTime? CreatedOn { get; set; }
        [BsonElement("modifiedBy")]
        public string? ModifiedBy { get; set; }
        [BsonElement("modifiedOn")]
        public DateTime? ModifiedOn { get; set; }
    }
}
