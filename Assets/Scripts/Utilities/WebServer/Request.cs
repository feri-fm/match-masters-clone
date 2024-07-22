using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace WebServer
{
    public class Context
    {
        public Request request;
        public Response response;
        public HttpListenerContext httpListenerContext;

        public Router router;

        public bool closed;
        public Exception exception = null;

        public Context(HttpListenerContext httpListenerContext)
        {
            this.httpListenerContext = httpListenerContext;
            request = new Request(this);
            response = new Response(this);
        }

        public void Close()
        {
            closed = true;
        }
    }

    public class Request
    {
        public Context context;
        public HttpListenerRequest httpRequest;

        public List<object> contents = new();
        public Dictionary<object, object> contentsDic = new();

        public Request(Context context)
        {
            this.context = context;
            httpRequest = context.httpListenerContext.Request;
        }

        public void Put(object data)
        {
            contents.Add(data);
        }
        public T Get<T>()
        {
            return (T)contents.FirstOrDefault(e => e is T);
        }

        public void Put(object key, object data)
        {
            contentsDic.Add(key, data);
        }
        public T Get<T>(object key)
        {
            if (contentsDic.TryGetValue(key, out var value))
                return (T)value;
            return default;
        }

        public string GetHeader(string key)
        {
            return httpRequest.Headers.Get(key);
        }

        public async Task<byte[]> ReadBytes()
        {
            var stream = httpRequest.InputStream;
            var length = httpRequest.ContentLength64;
            var bytes = new byte[length];
            await stream.ReadAsync(bytes, 0, (int)length);
            return bytes;
        }
        public async Task<string> ReadString()
        {
            var bytes = await ReadBytes();
            var text = httpRequest.ContentEncoding.GetString(bytes);
            return text;
        }
        public async Task<T> ReadJson<T>()
        {
            var text = await ReadString();
            return JObject.Parse(text).ToObject<T>();
        }
        public async Task<JObject> ReadJson()
        {
            var text = await ReadString();
            return JObject.Parse(text);
        }
    }

    public class Response
    {
        public Context context;
        public HttpListenerResponse httpResponse;

        public Response(Context context)
        {
            this.context = context;
            httpResponse = context.httpListenerContext.Response;
        }

        public void Close()
        {
            httpResponse.Close();
            context.Close();
        }

        public Response Status(ushort status)
        {
            httpResponse.StatusCode = status;
            return this;
        }

        public Response AddHeader(string key, string value)
        {
            httpResponse.AddHeader(key, value);
            return this;
        }

        public async Task Send(object data)
        {
            var json = data.ToJson();

            // Construct a response message
            byte[] buffer = Encoding.UTF8.GetBytes(json);

            // Set the response content type and length
            httpResponse.ContentType = "application/json";
            httpResponse.ContentLength64 = buffer.Length;

            // Write the response to the output stream
            System.IO.Stream output = httpResponse.OutputStream;
            await output.WriteAsync(buffer, 0, buffer.Length);

            // Close the output stream
            output.Close();

            Close();
        }

        public async Task SendError(ushort status, string message)
        {
            Status(status);
            await Send(new { message });
        }
    }
}