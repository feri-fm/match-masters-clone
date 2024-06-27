using System;
using MongoDB.Bson;
using MongoDB.Driver;
using UnityEngine;
using WebServer;

namespace Server
{
    public class ServerManager : MonoBehaviour
    {
        public WebServerApp app;

        public class BooException : Exception
        {
            public BooException(string message) : base(message)
            {
            }
        }
        public class FooException : Exception
        {
            public FooException(string message) : base(message)
            {
            }
        }

        private void Start()
        {
            app = new WebServerApp();

            var router = new Router();

            router.Use("/boo", (req, res) =>
            {
                throw new BooException("boo exception");
            });

            router.Use("/foo", (req, res) =>
            {
                throw new FooException("foo exception");
            });

            router.Use("/error", (req, res) =>
            {
                throw new Exception("normal exception");
            });

            router.Use(new ExceptionHandler<BooException>(async (req, res) =>
            {
                await res.SendJson("Boo exception caught: " + req.context.exception);
            }));

            router.Use(new ExceptionHandler<FooException>(async (req, res) =>
            {
                await res.SendJson("FOO exception caught: " + req.context.exception);
            }));

            router.Use(async (req, res) => { await res.SendJson(new { msg = "Not Found" }); });

            router.Use(new ExceptionHandler());

            app.Setup(router);

            ushort port = 3000;
            app.Listen(port);
            Debug.Log("Listening on " + port);
        }

        private void _Start()
        {
            app = new WebServerApp();

            AsyncMiddlewareCall SimpleHandler(string name)
            {
                return async (req, res) =>
                {
                    await res.SendJson(new
                    {
                        name,
                    });
                };
            }

            var router = new Router();

            // router
            // .In("/root")
            //     .In("/child")
            //         .Use("/nest", SimpleHandler("nest"))
            //         .Use("/nest1", SimpleHandler("nest1"))
            //         .Use("/nest2", SimpleHandler("nest2"))
            //         .Use(SimpleHandler("child"))
            //         .Out()
            //     .In("/child1")
            //         .Use("/nest", SimpleHandler("nest"))
            //         .Use("/nest1", SimpleHandler("nest1"))
            //         .Use("/nest2", SimpleHandler("nest2"))
            //         .Use(SimpleHandler("child1"))
            //         .Out()
            //     .Use("/child2", SimpleHandler("child2"))
            //     .Use("/child3", SimpleHandler("child3"))
            //     .Out()
            // .In("/root2")
            //     .In("/child")
            //         .Use("/nest", SimpleHandler("nest"))
            //         .Use("/nest1", SimpleHandler("nest1"))
            //         .Use("/nest2", SimpleHandler("nest2"))
            //         .Use(SimpleHandler("child"))
            //         .Out()
            //     .In("/child1")
            //         .Use("/nest", SimpleHandler("nest"))
            //         .Use("/nest1", SimpleHandler("nest1"))
            //         .Use("/nest2", SimpleHandler("nest2"))
            //         .Use(SimpleHandler("child1"))
            //         .Out()
            //     .Use("/child2", SimpleHandler("child2"))
            //     .Use("/child3", SimpleHandler("child3"))
            //     .Out();

            router
            .In("/posts")
                .Get("/:id", SimpleHandler("Auth"), SimpleHandler("Post with id"))
                .Get("/", SimpleHandler("Post with id"));

            router.Use(async (req, res) =>
            {
                await res.SendJson(new
                {
                    name = "this is the not found page",
                    Time.time,
                    check = true,
                    req.httpRequest.HttpMethod,
                    req.httpRequest.Url,
                    req.httpRequest.UrlReferrer,
                    req.httpRequest.RawUrl,
                    req.httpRequest.QueryString,
                });
            });

            app.Setup(router);
            app.Listen($"http://*:3000/");
        }

        private void OnDestroy()
        {
            app.Stop();
        }

        private void __Start()
        {
            var connectionString = "mongodb://localhost:27017";
            // var connectionString = Environment.GetEnvironmentVariable("MONGODB_URI");
            // if (connectionString == null)
            // {
            //     Console.WriteLine("You must set your 'MONGODB_URI' environment variable. To learn how to set it, see https://www.mongodb.com/docs/drivers/csharp/current/quick-start/#set-your-connection-string");
            //     Environment.Exit(0);
            // }
            var client = new MongoClient(connectionString);
            var collection = client.GetDatabase("karo-ludo").GetCollection<BsonDocument>("users");
            var filter = Builders<BsonDocument>.Filter.Eq("username", "User_17");
            var document = collection.Find(filter).First();
            Debug.Log(document);
        }
    }
}