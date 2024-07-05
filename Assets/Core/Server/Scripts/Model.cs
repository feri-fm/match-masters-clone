using MongoDB.Bson;
using Newtonsoft.Json;

namespace MMC.Server.Models
{
    public class Model
    {
        [JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId id;

    }
}