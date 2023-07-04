using Infrastructure.Data.Entity;
using Infrastructure.Data.Interface;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Infrastructure.Data.Repository
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly IMongoCollection<UserProfileEntity> _userProfileCollection;

        public UserProfileRepository(IOptions<DatabaseSettings> databaseSettings)
        {
            var mongoClient = new MongoClient(
                databaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                databaseSettings.Value.DatabaseName);

            _userProfileCollection = mongoDatabase.GetCollection<UserProfileEntity>(
                databaseSettings.Value.UserProfileCollectionName);
        }

        public async Task CreateAsync(UserProfileEntity user) => 
            await _userProfileCollection.InsertOneAsync(user);

        public async Task<UserProfileEntity> GetUserAsync(string email) =>
            await _userProfileCollection.Find(x => x.Email == email).FirstOrDefaultAsync();

        public async Task<UserProfileEntity> GetUsernameAsync(string username) =>
            await _userProfileCollection.Find(x => x.UserName == username).FirstOrDefaultAsync();
        
        public async Task<UserProfileEntity> GetUserAsync(Guid id) =>
            await _userProfileCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task UpdateAsync(Guid id, UserProfileEntity user) =>
            await _userProfileCollection.ReplaceOneAsync(x => x.Id == id, user);

        public async Task RemoveAsync(Guid id) {
            await _userProfileCollection.DeleteOneAsync(x => x.Id == id);
        }
    }
}