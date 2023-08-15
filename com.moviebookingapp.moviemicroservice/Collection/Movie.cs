using MongoDB.Bson.Serialization.Attributes;

namespace com.moviebookingapp.moviemicroservice.Collection
{
    public class Movie
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonElement("movieImage")]
        public string MovieImage { get; set; }
        [BsonElement("movieName")]
        public string MovieName { get; set; }

        [BsonElement("theatreName")]
        public string TheatreName { get; set; }

        [BsonElement("numberOfSeatsAlloted")]
        public int NoOfSeatsAlloted { get; set; }
        [BsonElement("remainingSeats")]
        public int RemainingSeats { get; set; }
    }
}
