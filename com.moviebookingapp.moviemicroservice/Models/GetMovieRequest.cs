using MongoDB.Bson.Serialization.Attributes;

namespace com.moviebookingapp.moviemicroservice.Models
{
    public class GetMovieRequest
    {
        [BsonElement("movieName")]
        public string MovieName { get; set; }

        [BsonElement("theatreName")]
        public string TheatreName { get; set; }
    }
}
