using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace MMC.Server.Models
{
    public class Model
    {
        [JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId id;

        [JsonIgnore]
        [BsonIgnore]
        public Database database;

        public void _Setup(Database database)
        {
            this.database = database;
            Setup();
        }

        public virtual void Setup() { }
    }

    public class Model<TModel> : Model
        where TModel : Model
    {
        [JsonIgnore]
        [BsonIgnore]
        public IMongoCollection<TModel> collection;

        public virtual void PreSave() { }

        public override void Setup()
        {
            base.Setup();
            collection = database.GetCollection<TModel>();
        }

        public async Task Update(Func<UpdateDefinitionBuilder<TModel>, UpdateDefinition<TModel>> builder)
        {
            var filter = Builders<TModel>.Filter.Eq("_id", id);
            var update = builder.Invoke(Builders<TModel>.Update);
            await collection.UpdateOneAsync(filter, update);
        }
    }
}