using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Diagnostics;


namespace GeminiSharp {
    /// <summary>
    /// Object for calling Gemini's text embedding model. Currently supports text-embedding-004.
    /// </summary>
    [Serializable]
    public class TextEmbedding : BaseModel {
        [JsonProperty(Order = 2)] public EmbeddingContent content;
        public TextEmbedding(string apikey, string text) : base(apikey, "models/text-embedding-004", "v1beta", "") { content = new EmbeddingContent(text); }

        public override async Task<Response> Post(string req) {
            url = $"https://generativelanguage.googleapis.com/{version}/{model}:embedContent?key={API_KEY}";
            var content = new StringContent(req, Encoding.UTF8, "application/json");
            Response ResponseObject;
            
            try {
                var response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                ResponseObject = JsonConvert.DeserializeObject<Response>(responseContent);
            }
            catch (Exception e) {
                Debug.WriteLine($"Request Error: {e.Message}");
                ResponseObject = null;
            }
            return ResponseObject;
        }
    }

    /// <summary>
    /// Object for running a batch of texts for Gemini's text embedding model. Currently supports text-embedding-004.
    /// </summary>
    [Serializable]
    public class BatchEmbedding : TextEmbedding {
        [JsonProperty(Order = 1)] public List<TextEmbedding> requests;
        
        public BatchEmbedding(string apikey, IEnumerable<string> texts) : base (apikey, null) {
            requests = new List<TextEmbedding>();
            foreach(string text in texts) { if(!string.IsNullOrEmpty(text)) { requests.Add(new TextEmbedding(apikey, text)); }}
        }

        public override string jsonify() {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new IgnoreResolver(typeof(BatchEmbedding), "model", "content")
            });
        }

        public override async Task<Response> Post(string req) {
            url = $"https://generativelanguage.googleapis.com/{version}/{model}:batchEmbedContents?key={API_KEY}";
            var content = new StringContent(req, Encoding.UTF8, "application/json");
            Response ResponseObject;

            try {
                var response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                ResponseObject = JsonConvert.DeserializeObject<Response>(responseContent);
            }
            catch (Exception e) {
                Debug.WriteLine($"Request Error: {e.Message}");
                ResponseObject = null;
            }
            return ResponseObject;
        }
    }
}