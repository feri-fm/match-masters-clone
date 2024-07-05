using System.Threading.Tasks;
using MMC.Server.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MMC.Server
{
    [Module]
    public class Database : Module
    {
        public string connectionString = "mongodb://localhost:27017";

        public MongoClient client;
        public IMongoDatabase db;

        public IMongoCollection<UserModel> users => db.GetCollection<UserModel>("users");
        public IMongoCollection<GameModel> games => db.GetCollection<GameModel>("games");

        public override void Build()
        {
            base.Build();
            client = new MongoClient(connectionString);
            db = client.GetDatabase("match-masters-clone");
        }
    }
}