using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class WebRequestBuilder : MonoBehaviour
{
    public WebRequestConfig config;

    public List<Request> currentRequests { get; } = new List<Request>();

    public bool isLoading => currentRequests.Count(e => e.showLoading) > 0;

    public Request Get(string url) => CreateRequest(UnityWebRequest.kHttpVerbGET, config.baseUrl + url);
    public Request Post(string url, object data = null) => CreateRequest(UnityWebRequest.kHttpVerbPOST, config.baseUrl + url, data);

    public Request CreateRequest(string method, string url, object data = null)
    {
        var json = data == null ? "{}" : data.ToJson();
        var req = new Request(method, url, json, (r) => SendRequest(r));
        OnRequestCreated(req);
        return req;
    }
    public Request CreateRequest(UnityWebRequest unityWebRequest)
    {
        var req = new Request(unityWebRequest, (r) => SendRequest(r));
        OnRequestCreated(req);
        return req;
    }

    public void SendRequest(Request request)
    {
        StartCoroutine(ISendRequest(request));
    }

    private IEnumerator ISendRequest(Request request)
    {
        var r = request.request;
        using (r)
        {
            r.SendWebRequest();
            currentRequests.Add(request);
            Loading(request, 0, false);
#if UNITY_EDITOR
            yield return new WaitForSeconds(Random.Range(0.2f, 0.5f));
#endif
            while (!r.isDone &&
                r.result != UnityWebRequest.Result.ConnectionError
                && r.result != UnityWebRequest.Result.ProtocolError)
            {
                Loading(request, r.downloadProgress, false);
                yield return new WaitForEndOfFrame();
            }
            Loading(request, 1, true);
            currentRequests.Remove(request);

            if (r.result == UnityWebRequest.Result.Success
                && r.responseCode.ToString().StartsWith("2"))
            {
                OnResponse(request);
                request._OnResponse();
            }
            else
            {
                OnFailure(request);
                request._OnFailure();
            }
        }
    }

    public virtual void Loading(Request request, float value, bool done)
    {

    }
    public virtual void OnRequestCreated(Request request)
    {
        Debug.Log($"Request: {request.request.uri.LocalPath}: {request.sendJson.Beautify()}");
    }
    public virtual void OnResponse(Request request)
    {
        Debug.Log($"Response: {request.status}: {request.body.Beautify()}");
    }
    public virtual void OnFailure(Request request)
    {
        Debug.Log($"Failure: {request.status}: {request.error}: {request.body}");
    }
}

public class Request
{
    public UnityWebRequest request { get; private set; }

    public string sendJson { get; private set; }

    public bool showLoading { get; private set; }
    public bool showError { get; private set; }

    private UnityAction<Request> onResponse { get; set; } = new UnityAction<Request>((r) => { });
    private UnityAction<Request> onFailure { get; set; } = new UnityAction<Request>((r) => { });

    public string body => request.downloadHandler.text;
    public string error => request.error;
    public long status => request.responseCode;

    public long[] ignoreStatusCodes = new long[] { };
    private UnityAction<Request> send;

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

    public Request(string method, string url, string json, UnityAction<Request> send)
    {
        this.send = send;
        sendJson = json;
        showLoading = true;
        showError = true;
        request = new UnityWebRequest(url, method);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        Header("content-type", "application/json");
    }

    public Request(UnityWebRequest unityWebRequest, UnityAction<Request> send)
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

    public Request Header(string name, string value)
    {
        request.SetRequestHeader(name, value ?? "");
        return this;
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

    public Request R(UnityAction action) => R((r) => action.Invoke());
    public Request R<T>(UnityAction<Request, T> action) => R((r) => action.Invoke(r, r.GetBody<T>()));
    public Request R(UnityAction<Request> action)
    { onResponse += action; return this; }

    public Request F(UnityAction action) => F((r) => action.Invoke());
    public Request F<T>(UnityAction<Request, T> action) => F((r) => action.Invoke(r, r.GetBody<T>()));
    public Request F(UnityAction<Request> action)
    { onFailure += action; return this; }
}

[System.Serializable]
public class WebRequestConfig
{
    public string baseUrl;
}
