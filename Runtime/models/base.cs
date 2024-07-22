using System;
using System.Collections;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using UnityEngine;

namespace GeminiSharp {
/// <summary>
/// This is the abstract base model object from which all other language model objects inherit.
/// To call a Gemini model, please use one of the derived classes.
/// </summary>
    public abstract class BaseModel {
        private protected static readonly HttpClient client = new HttpClient();
        private protected string url; //the url to post the http request to
        [JsonIgnore] public string version; //v1 or v1beta? That's what this resolves
        [JsonIgnore] public string release; //do they want -latest, -00x, or stable (empty)?
        [JsonProperty(Order = 1)] public string model;
        [JsonIgnore] private protected string API_KEY;
        public BaseModel(string apikey, string model, string ver = "v1beta", string rel = "-latest") { 
            API_KEY = apikey;
            version = ver;
            this.model = model + rel;
            url = $"https://generativelanguage.googleapis.com/{version}/{this.model}:generateContent?key={API_KEY}";
        }

        ///<summary>
        /// Converts the object into a JSON string to be sent in the POST request.
        /// </summary>
        public virtual string jsonify() { return JsonConvert.SerializeObject(this); }

        ///<summary>
        /// Sends a POST request to the Gemini API asyncronously.
        /// </summary>
        public virtual async Task<Response> Post(string req) {
            var content = new StringContent(req, Encoding.UTF8, "application/json"); //plaintext could technically be supported, but it is not recommended here. UTF-8 is standard for LLM token processing.
            Response ResponseObject;
            try {
                var response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                ResponseObject = JsonConvert.DeserializeObject<Response>(responseContent);
                Debug.Log("Deserialized Response: " + responseContent.ToString());
                return ResponseObject;
            }
            catch (Exception e) {
                Debug.Log($"Request Error: {e.Message}");
                return null;
            }

        }

        public virtual IEnumerator Run<T>(Task<T> task, Action<T> OnSuccess, Action<Exception> OnFailure) {
            while (!task.IsCompleted) { yield return null; }
            if (task.IsFaulted) { OnFailure?.Invoke(task.Exception); }
            else { OnSuccess?.Invoke(task.Result); }
        }
    }
}

/* 
            using (var response = client.PostAsync(url, content).Result) {
                if (response.IsSuccessStatusCode) {
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    OnSuccess?.Invoke(responseContent);
                } 
                else { OnFailure?.Invoke($"Request Error: {response.StatusCode}"); }
            }*/