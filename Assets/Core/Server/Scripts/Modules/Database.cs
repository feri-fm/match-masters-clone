using System.Collections.Generic;
using System.Threading.Tasks;
using MMC.Server.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MMC.Server
{
    [Module]
    public class Database : Module
    {
        public string connectionString = "mongodb://localhost:27017"; //TODO: this shouldn't be here

        public MongoClient client;
        public IMongoDatabase db;

        public IMongoCollection<UserModel> users => GetCollection<UserModel>();
        public IMongoCollection<GameModel> games => GetCollection<GameModel>();

        public Dictionary<string, string> modelNames = new() {
            { nameof(UserModel), "users" },
            { nameof(GameModel), "games" },
        };

        public override void Build()
        {
            base.Build();
            client = new MongoClient(connectionString);
            db = client.GetDatabase("match-masters-clone");
        }

        public IMongoCollection<TModel> GetCollection<TModel>()
        {
            var name = typeof(TModel).Name;
            return db.GetCollection<TModel>(modelNames[name]);
        }
    }
}