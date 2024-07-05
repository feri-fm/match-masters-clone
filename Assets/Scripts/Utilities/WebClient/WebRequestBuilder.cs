using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace WebClient
{
    public sealed class WebRequestBuilder
    {
        public WebClientConfig config;

        public List<Request> currentRequests { get; } = new List<Request>();

        public event Action<Request> onRequestCreated = delegate { };
        public event Action<Request> onResponse = delegate { };
        public event Action<Request> onFailure = delegate { };

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

        private void SendRequest(Request request)
        {
            WebClientManager.instance.StartCoroutine(ISendRequest(request));
        }
        private IEnumerator ISendRequest(Request request)
        {
            var unityWebRequest = request.request;
            using (unityWebRequest)
            {
                unityWebRequest.SendWebRequest();
                currentRequests.Add(request);
#if UNITY_EDITOR
                yield return new WaitForSeconds(UnityEngine.Random.Range(0.2f, 0.5f));
#endif
                while (!unityWebRequest.isDone &&
                    unityWebRequest.result != UnityWebRequest.Result.ConnectionError
                    && unityWebRequest.result != UnityWebRequest.Result.ProtocolError)
                {
                    yield return new WaitForEndOfFrame();
                }
                currentRequests.Remove(request);

                if (unityWebRequest.result == UnityWebRequest.Result.Success
                    && unityWebRequest.responseCode.ToString().StartsWith("2"))
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

        public void OnRequestCreated(Request request)
        {
            Debug.Log($"Request: {request.request.uri.LocalPath}: {request.requestBody.Beautify()}");
            onRequestCreated.Invoke(request);
        }
        public void OnResponse(Request request)
        {
            Debug.Log($"Response: {request.status}: {request.body.Beautify()}");
            onResponse.Invoke(request);
        }
        public void OnFailure(Request request)
        {
            Debug.Log($"Failure: {request.status}: {request.error}: {request.body}");
            onFailure.Invoke(request);
        }
    }

    [System.Serializable]
    public class WebClientConfig
    {
        public string baseUrl;
    }
}
