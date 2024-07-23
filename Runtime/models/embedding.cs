using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;


namespace GeminiSharp {
    /// <summary>
    /// Object for calling Gemini's text embedding model. Currently supports text-embedding-004.
    /// Whereas other model objects are designed to store and change their own values anywhere in the lifetime of the object, 
    /// this object is designed to be an encapsulator for the text it was instantiated with.
    /// To retrieve the task this object provides, use the Send method. Execute the coroutine with Run().
    /// </summary>
    [Serializable]
    public class TextEmbedding : BaseModel<EmbeddingResponse> {
        [JsonProperty(Order = 2)] public Content content;
        /// <summary>
        /// TextEmbedding constructor. Takes an API key and a string of text for the model to embed.
        /// </summary>
        /// <param name="apikey">Your Gemini API_KEY. DO NOT HARD-CODE YOUR KEY INTO YOUR PROGRAM. </param>
        /// <param name="text">The string value to vectorize as a text embedding. </param>
        public TextEmbedding(string apikey, string text) : base(apikey, "models/text-embedding-004", "v1beta", "") { content = new Content { role = null, parts = new List<Part> { new Part { text = text } } }; } 

        ///<summary>
        /// Sends a POST request to the embedding model asyncronously. For internal use only; do not call this directly. Instead, use Send.
        /// </summary>
        private protected override async Task<EmbeddingResponse> Post(string req) {
            url = $"https://generativelanguage.googleapis.com/{version}/{model}:embedContent?key={API_KEY}";
            var content = new StringContent(req, Encoding.UTF8, "application/json");
            EmbeddingResponse ResponseObject;
            
            try {
                var response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                
                ResponseObject = JsonConvert.DeserializeObject<EmbeddingResponse>(responseContent);
                Logger.Log("Deserialized Response: " + responseContent.ToString());
            }
            catch (Exception e) {
                Logger.LogError($"Request Error: {e.Message}");
                ResponseObject = null;
            }
            return ResponseObject;
        }

        /// <summary>
        /// Constructs the task that sends the request to the Gemini API and returns the response as an EmbeddingResponse object.
        /// </summary>
        /// <returns>An EmbeddingResponse-typed Task. </returns>
        public async Task<EmbeddingResponse> Send() { return await Post(jsonify()); }
    }

    /// <summary>
    /// Object for running a batch of texts for Gemini's text embedding model. Currently supports text-embedding-004.
    /// Functions similarly to TextEmbedding, but takes a list of strings instead of a single string upon construction.
    /// Inherits from TextEmbedding.
    /// </summary>
    [Serializable]
    public class BatchEmbedding : TextEmbedding {
        [JsonProperty(Order = 1)] public List<TextEmbedding> requests;
        
        /// <summary>
        /// BatchEmbedding constructor. Takes an API key and a list of strings for the model to embed.
        /// </summary>
        /// <param name="apikey">Your Gemini API_KEY. DO NOT HARD-CODE YOUR KEY INTO YOUR PROGRAM. </param>
        /// <param name="texts">The list of strings to vectorize as a text embedding in one process. </param>
        public BatchEmbedding(string apikey, IEnumerable<string> texts) : base (apikey, null) {
            requests = new List<TextEmbedding>();
            foreach(string text in texts) { if(!string.IsNullOrEmpty(text)) { requests.Add(new TextEmbedding(apikey, text)); }}
        }

        /// <summary>
        /// Converts the object into a JSON object string.
        /// </summary>
        /// <returns> JSON string </returns>
        public override string jsonify() {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new IgnoreResolver(typeof(BatchEmbedding), "model", "content")
            });
        }

        ///<summary>
        /// Sends a POST request to the embedding model asyncronously. For internal use only; do not call this directly. Instead, use Send.
        /// </summary>
        private protected override async Task<EmbeddingResponse> Post(string req) {
            url = $"https://generativelanguage.googleapis.com/{version}/{model}:batchEmbedContents?key={API_KEY}";
            var content = new StringContent(req, Encoding.UTF8, "application/json");
            EmbeddingResponse ResponseObject;
            try {
                var response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                ResponseObject = JsonConvert.DeserializeObject<EmbeddingResponse>(responseContent);
                Logger.Log("Deserialized Response: " + responseContent.ToString());
            }
            catch (Exception e) {
                Logger.LogError($"Request Error: {e.Message}");
                ResponseObject = null;
            }
            return ResponseObject;
        }
    }
}