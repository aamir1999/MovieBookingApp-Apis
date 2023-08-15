using com.moviebookingapp.usermicroservice.Collection;
using MongoDB.Driver;
using MongoDB.Driver.Core.Operations;
using static MongoDB.Driver.WriteConcern;

namespace com.moviebookingapp.usermicroservice.Repository
{
    public class UserRepository:IUserRepository
    {
        private readonly IMongoCollection<Users> _userCollection;
        public UserRepository(IMongoDatabase mongoDatabase)
        {
            _userCollection = mongoDatabase.GetCollection<Users>("Users");
        }
        /// <summary>
        /// Validate new user  
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="userLoginId"></param>
        /// <returns></returns>
        public async Task<Users> ValidateUser(string userEmail, string userLoginId)
        {
            var emailFilter = Builders<Users>.Filter
                .Eq(e => e.Email, userEmail);
            var loginIdFilter = Builders<Users>.Filter
                .Eq(l => l.LoginId, userLoginId);
            var userExitsFilter = Builders<Users>.Filter
                .Or(emailFilter, loginIdFilter);
            return await _userCollection.Find(userExitsFilter).FirstOrDefaultAsync();
        }
        /// <summary>
        /// Validate if user already exists
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="userLoginId"></param>
        /// <returns></returns>
        public async Task<Users> ValidateLoginUser(string userEmail, string userLoginId)
        {
            var emailFilter = Builders<Users>.Filter
                .Eq(e => e.Email, userEmail);
            var loginIdFilter = Builders<Users>.Filter
                .Eq(l => l.LoginId, userLoginId);
            var userExitsFilter = Builders<Users>.Filter
                .And(emailFilter, loginIdFilter);
            return await _userCollection.Find(userExitsFilter).FirstOrDefaultAsync();
        }


        /// <summary>
        /// Register new user into UserCollection
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public async Task Register(Users users)
        {
            await _userCollection.InsertOneAsync(users);
        }
        /// <summary>
        /// validate users for forgot password
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public async Task<Users> ForgotPassword(string userEmail)
        {
            return await _userCollection.Find(x=>x.Email== userEmail).FirstOrDefaultAsync();
        }
        /// <summary>
        /// update reset token in db
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public async Task UpdateResetToken(Users users)
        {
            var emailFilter = Builders<Users>.Filter
                .Eq(e => e.Email, users.Email);
            var updateResetToken= Builders<Users>.Update
                .Set(u =>u.ResetToken , users.ResetToken)
                .Set(u=>u.ResetTokenExpires,users.ResetTokenExpires);
            await _userCollection.UpdateOneAsync(emailFilter, updateResetToken);
        }

        /// <summary>
        /// Validate reset token against db
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Users> ValidateResetToken(string token)
        {
            return await _userCollection.Find(u=>u.ResetToken==token).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Reset users password
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public async Task UpdatePassword(Users users)
        {
            var emailFilter = Builders<Users>.Filter
              .Eq(e => e.Email, users.Email);
            var updatePassword = Builders<Users>.Update
                .Set(u => u.Password, users.Password)
                .Set(u => u.ResetToken, users.ResetToken)
                .Set(u => u.ResetTokenExpires, users.ResetTokenExpires);
            await _userCollection.UpdateOneAsync(emailFilter, updatePassword);
        }

        
    }
}
