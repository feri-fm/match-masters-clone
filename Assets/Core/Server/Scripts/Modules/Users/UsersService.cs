using System.Threading.Tasks;
using MMC.Server.Models;
using MongoDB.Bson;

namespace MMC.Server
{
    [Service]
    public class UsersService : Service
    {
        [UseModule] public UsersRepository repository;

        public async Task<UserModel> CreateNewUser() => await repository.CreateNewUser();
        public async Task<UserModel> GetUser(ObjectId id) => await repository.GetUser(id);
        public async Task<UserModel> FindUser(string username) => await repository.FindUser(username);
    }
}