using System;
using System.Collections;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace GeminiSharp {
    /// <summary>
    /// This is the abstract base model object from which all other language model objects inherit.
    /// To call a Gemini model, please use one of the derived classes.
    /// </summary>
    public abstract class BaseModel<T> where T: IResponse {
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
        /// Converts the object into a JSON object string.
        /// </summary>
        public virtual string jsonify() { return JsonConvert.SerializeObject(this); }

        ///<summary>
        /// Sends a POST request to the Gemini API asyncronously. For internal use only; do not call this directly. Instead, use SingleShot, Chat, or Send.
        /// </summary>
        private protected virtual async Task<T> Post(string req) {
            var content = new StringContent(req, Encoding.UTF8, "application/json"); //plaintext could technically be supported, but it is not recommended here. UTF-8 is standard for LLM token processing.
            
            try {
                var response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                var ResponseObject = JsonConvert.DeserializeObject<T>(responseContent);
                if(ResponseObject == null) { throw new Exception("The deserialized response is null."); }
                else if(ResponseObject is Response resp) {  
                    foreach (var candidate in resp.candidates) {  // Process the parts to ensure proper handling of text and functionCall
                        foreach (var part in candidate.content.parts) {
                            if (!string.IsNullOrEmpty(part.text)) {
                                Logger.Log("Response Text: " + part.text);
                            } else if (part.functionCall != null) {
                                Logger.Log("Function: " + part.functionCall.name);
                            }
                        }
                    }
                }
                Logger.Log("Deserialized Response: " + responseContent.ToString());
                return ResponseObject;
            }
            catch (Exception e) {
                Logger.Log($"Request Error: {e.Message}");
                return default;
            }

        }

        /// <summary>
        /// Runs the asyncronous task and invokes the appropriate callback provided based on the result.
        /// Use StartCoroutine to run this method.
        /// </summary>
        /// <param name="task"> The async task to perform. This should typically SingleShot, Chat, or Send. type T can be either a Response or EmbeddingResponse. </param>
        /// <param name="OnSuccess"> The method to call upon successful execution of the given task. </param>
        /// <param name="OnFailure"> The method to call upon any failure or error message resulting from the attempted task execution. </param>
        /// <returns>IEnumerator (this is an asyncronous method) </returns>
        public virtual IEnumerator Run(Task<T> task, Action<T> OnSuccess, Action<Exception> OnFailure) {
            while (!task.IsCompleted) { yield return null; }
            if (task.IsFaulted) { OnFailure?.Invoke(task.Exception); }
            else { OnSuccess?.Invoke(task.Result); }
        }
    }
}