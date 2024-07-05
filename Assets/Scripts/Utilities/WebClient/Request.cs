using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;

namespace WebClient
{
    public class Request
    {
        public UnityWebRequest request { get; private set; }

        public string requestBody { get; private set; }

        public bool showLoading { get; private set; }
        public bool showError { get; private set; }

        private Action<Request> onResponse { get; set; } = new Action<Request>((r) => { });
        private Action<Request> onFailure { get; set; } = new Action<Request>((r) => { });

        public string body => request.downloadHandler.text;
        public string error => request.error;
        public long status => request.responseCode;

        public float downloadProgress => request.downloadProgress;

        public long[] ignoreStatusCodes = new long[] { };
        private Action<Request> send;

        public bool ignore => ignoreStatusCodes.Contains(status);

        public T GetBody<T>()
        {
            return body.FromJson<T>();
        }

        public T GetField<T>(string field)
        {
            try
            {
                var json = JObject.Parse(body);
                return json.GetValue(field).ToObject<T>();
            }
            catch
            {
                return default(T);
            }
        }

        public Request(string method, string url, string json, Action<Request> send)
        {
            this.send = send;
            requestBody = json;
            showLoading = true;
            showError = true;
            request = new UnityWebRequest(url, method);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            AddHeader("content-type", "application/json");
        }

        public Request(UnityWebRequest unityWebRequest, Action<Request> send)
        {
            this.send = send;
            showLoading = true;
            showError = true;
            request = unityWebRequest;
        }

        public void _OnResponse()
        {
            onResponse?.Invoke(this);
        }
        public void _OnFailure()
        {
            onFailure?.Invoke(this);
        }

        public Request AddHeader(string name, string value)
        {
            request.SetRequestHeader(name, value ?? "");
            return this;
        }

        public string GetResponseHeader(string key)
        {
            return request.GetResponseHeader(key);
        }

        public Request Ignore(params long[] codes)
        {
            ignoreStatusCodes = codes;
            return this;
        }

        public Request Send()
        {
            send.Invoke(this);
            return this;
        }

        public Request SetLoading(bool value)
        {
            showLoading = value;
            return this;
        }
        public Request SetError(bool value)
        {
            showError = value;
            return this;
        }

        public Request R(Action action) => R((r) => action.Invoke());
        public Request R<T>(Action<Request, T> action) => R((r) => action.Invoke(r, r.GetBody<T>()));
        public Request R(Action<Request> action)
        { onResponse += action; return this; }

        public Request F(Action action) => F((r) => action.Invoke());
        public Request F<T>(Action<Request, T> action) => F((r) => action.Invoke(r, r.GetBody<T>()));
        public Request F(Action<Request> action)
        { onFailure += action; return this; }
    }
}
