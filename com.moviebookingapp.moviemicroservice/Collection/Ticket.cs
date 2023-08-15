using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace com.moviebookingapp.moviemicroservice.Collection
{
    public class Ticket
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonElement("email")]
        [EmailAddress]
        public string Email { get; set; }
        [BsonElement("movieName")]
        public string MovieName { get; set; }

        [BsonElement("theatreName")]
        public string TheatreName { get; set; }
        [BsonElement("numberOfTickets")]
        public int NumberOfTickets { get; set; }
        [BsonElement("seatNumber")]
        public List<string> SeatNumber { get; set; }
    }
}
