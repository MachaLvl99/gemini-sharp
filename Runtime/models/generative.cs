using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace GeminiSharp {
    /// <summary>
    /// Gemini 1.5 Flash object.
    /// </summary>
    public class GeminiFlash : BaseModel {
        public SafetySettings safetySettings { get; set; }
        public SystemInstruction system_instruction { get; set; }
        public Contents contents { get; set; }
        public Tool[] tools { get; set; } 
        public ToolConfig tool_config { get; set; }
        /// <summary>
        /// Creates a new Gemini 1.5 Flash object. Object values begin as null, but are not necessary to set before sending a request.
        /// To change the values that are sent in a request, simply set the object's properties directly.
        /// </summary>
        /// <param name="apikey">Your Gemini API_KEY. DO NOT HARD-CODE YOUR KEY INTO YOUR PROGRAM. Instead, use Unity Cloud services, and pass a string variable/object here.</param>
        /// <param name="version">Defaults to "v1beta". To change it, pass in "v1" (or whatever other version is available)</param>
        /// <param name="release">Defaults to "-latest". To change it, pass in the version directly as "-00x" or pass an empty string "" for latest stable version.</param>
        /// <returns>A new GeminiFlash object</returns>
        public GeminiFlash(string apikey, string version = "v1beta", string release = "-latest") : base(apikey, "models/gemini-1.5-flash", version, release) { }

        /// <summary>
        /// Single-Shot prompt request. Quickest way to query the model.
        /// </summary>
        /// <param name="text"> The query to give to Gemini.</param>
        /// <returns>IEnumerator</returns>
        public async Task<Response> SingleShot(string text) {
            contents = new Contents(new Content { role = "user", parts = new List<Parts> { new Parts(text) }});
            string pkg = BuildRequest().jsonify();
            Debug.Log(pkg);
            return await Post(pkg);
        }

        public IEnumerator Chat() {yield return null; }

        private Request BuildRequest() { return new Request(contents, safetySettings, system_instruction, tool_config, tools); }
    }

    public class GeminiPro : BaseModel {
        [JsonProperty(Order = 1)] public SafetySettings safetySettings { get; set; }
        [JsonProperty(Order = 2)] public SystemInstruction system_instruction { get; set; }
        [JsonProperty(Order = 3)] public Contents contents;
        [JsonProperty(Order = 4)] public Tool[] tools { get; set; } 
        [JsonProperty(Order = 5)] public ToolConfig tool_config { get; set; }
        public GeminiPro(string apikey, string version = "v1beta", string release = "-latest") : base(apikey, "models/gemini-1.5-pro", version, release) { }

        public Request BuildRequest() { return new Request(contents, safetySettings, system_instruction, tool_config, tools); }

        public IEnumerator SingleShot() { 
            yield return null; }
        public IEnumerator Chat() {yield return null; }
    }
}