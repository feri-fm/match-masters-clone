using System.Threading.Tasks;
using MMC.Server.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MMC.Server
{
    [Repository]
    public class UsersRepository : Repository
    {
        [UseModule] public Database database;

        public async Task<UserModel> CreateNewUser()
        {
            var user = new UserModel();
            var count = await database.users.CountDocumentsAsync(Builders<UserModel>.Filter.Empty);
            user.username = "user_" + count;
            await database.users.InsertOneAsync(user);
            return user;
        }

        public async Task<UserModel> GetUser(ObjectId id)
        {
            var cursor = await database.users.FindAsync(Builders<UserModel>.Filter.Eq("_id", id));
            var user = await cursor.FirstOrDefaultAsync();
            return user;
        }
    }
}