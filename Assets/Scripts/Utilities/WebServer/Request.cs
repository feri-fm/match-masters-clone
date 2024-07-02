using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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

        public Request(Context context)
        {
            this.context = context;
            httpRequest = context.httpListenerContext.Request;
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
    }
}