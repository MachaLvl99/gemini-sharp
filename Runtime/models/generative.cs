using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeminiSharp {
    /// <summary>
    /// Gemini 1.5 Flash object.
    /// </summary>
    public class GeminiFlash : BaseModel<Response> {
        public SafetySettings safetySettings { get; set; }
        public SystemInstruction system_instruction { get; set; }
        public Contents contents { get; set; }
        public List<Contents> chat_contents { get; set; }
        public Tool[] tools { get; set; } 
        public ToolConfig tool_config { get; set; }
        /// <summary>
        /// Creates a new Gemini 1.5 Flash object. Object values begin as null, but are not necessary to set before sending a request.
        /// To change the values that are sent in a request, simply set the object's properties directly.
        /// </summary>
        /// <param name="apikey">Your Gemini API_KEY. DO NOT HARD-CODE YOUR KEY INTO YOUR PROGRAM. </param>
        /// <param name="version">Defaults to "v1beta". To change it, pass in "v1" (or whatever other version is available)</param>
        /// <param name="release">Defaults to "-latest". To change it, pass in the version directly as "-00x" or pass an empty string "" for latest stable version.</param>
        /// <returns>A Response-typed async Task </returns>
        public GeminiFlash(string apikey, string version = "v1beta", string release = "-latest") : base(apikey, "models/gemini-1.5-flash", version, release) { }

        /// <summary>
        /// Single-Shot prompt request. Quickest way to query the model.
        /// </summary>
        /// <param name="text"> The query to give to Gemini.</param>
        /// <returns>A Response-typed async Task to run. </returns>
        public async Task<Response> SingleShot(string text) {
            var pass = new Part { text = text };
            contents = new Contents(new Content { role = "user", parts = new List<Part> { pass } } );
            string pkg = BuildRequest().jsonify();
            Logger.Log(pkg);
            return await Post(pkg);
        }

        /// <summary>
        /// A multi-turn prompt request. Use this to simulate a chat conversation with the model.
        /// </summary>
        /// <param name="chats"> An array of tuples (string, string). 
        /// Each tuple represents who produced what text, and the text itself. 
        /// The order of the values in the array represents the chronological order of the conversation. </param>
        /// <returns>IEnumerator</returns>
        public async Task<Response> Chat((string role, string text)[] chats) {
            chat_contents = new List<Contents>();
            foreach (var (role, text) in chats) {
                var chunk = new Content{
                    role = role,
                    parts = new List<Part> { new Part { text = text } }
                };
                chat_contents.Add(new Contents(chunk));
            }
            string pkg = BuildChat(chat_contents).jsonify();
            Logger.Log(pkg);
            return await Post(pkg);
        }

        /// <summary>
        /// Internal use only. Builds the request object that is to be sent to Gemini from the object's properties.
        /// </summary>
        /// <returns>Request object. </returns>
        private Request BuildRequest() { return new Request(contents, safetySettings, system_instruction, tool_config, tools); }

        /// <summary>
        /// Internal use only. Builds the chat request object that is to be sent to Gemini from the object's properties.
        /// This method constructs the list of Contents objects instead of the single Contents object.
        /// </summary>
        /// <param name="c">The list of Contents objects to construct for serialization. </param>
        /// <returns>ChatRequest object. </returns>
        private ChatRequest BuildChat(List<Contents> c) { return new ChatRequest(c, safetySettings, system_instruction, tool_config, tools); }
    }


    /// <summary>
    /// Gemini 1.5 Pro object.
    /// </summary>
    public class GeminiPro : BaseModel<Response> {
        public SafetySettings safetySettings { get; set; }
        public SystemInstruction system_instruction { get; set; }
        public Contents contents { get; set; }
        public List<Contents> chat_contents { get; set; }
        public Tool[] tools { get; set; } 
        public ToolConfig tool_config { get; set; }
        /// <summary>
        /// Creates a new Gemini 1.5 Pro object. Object values begin as null, but are not necessary to set before sending a request.
        /// To change the values that are sent in a request, simply set the object's properties directly.
        /// </summary>
        /// <param name="apikey">Your Gemini API_KEY. DO NOT HARD-CODE YOUR KEY INTO YOUR PROGRAM. </param>
        /// <param name="version">Defaults to "v1beta". To change it, pass in "v1" (or whatever other version is available)</param>
        /// <param name="release">Defaults to "-latest". To change it, pass in the version directly as "-00x" or pass an empty string "" for latest stable version.</param>
        /// <returns>A Response-typed async Task </returns>
        public GeminiPro(string apikey, string version = "v1beta", string release = "-latest") : base(apikey, "models/gemini-1.5-pro", version, release) { }

        /// <summary>
        /// Single-Shot prompt request. Quickest way to query the model.
        /// </summary>
        /// <param name="text"> The query to give to Gemini.</param>
        /// <returns>A Response-typed async Task to run. </returns>
        public async Task<Response> SingleShot(string text) {
            var pass = new Part { text = text };
            contents = new Contents(new Content { role = "user", parts = new List<Part> { pass } } );
            string pkg = BuildRequest().jsonify();
            Logger.Log(pkg);
            return await Post(pkg);
        }

        /// <summary>
        /// A multi-turn prompt request. Use this to simulate a chat conversation with the model.
        /// </summary>
        /// <param name="chats"> An array of tuples (string, string). 
        /// Each tuple represents who produced what text, and the text itself. 
        /// The order of the values in the array represents the chronological order of the conversation. </param>
        /// <returns>IEnumerator</returns>
        public async Task<Response> Chat((string role, string text)[] chats) {
            chat_contents = new List<Contents>();
            foreach (var (role, text) in chats) {
                var chunk = new Content{
                    role = role,
                    parts = new List<Part> { new Part { text = text } }
                };
                chat_contents.Add(new Contents(chunk));
            }
            string pkg = BuildChat(chat_contents).jsonify();
            Logger.Log(pkg);
            return await Post(pkg);
        }

        /// <summary>
        /// Internal use only. Builds the request object that is to be sent to Gemini from the object's properties.
        /// </summary>
        /// <returns>Request object. </returns>
        private Request BuildRequest() { return new Request(contents, safetySettings, system_instruction, tool_config, tools); }

        /// <summary>
        /// Internal use only. Builds the chat request object that is to be sent to Gemini from the object's properties.
        /// This method constructs the list of Contents objects instead of the single Contents object.
        /// </summary>
        /// <param name="c">The list of Contents objects to construct for serialization. </param>
        /// <returns>ChatRequest object. </returns>
        private ChatRequest BuildChat(List<Contents> c) { return new ChatRequest(c, safetySettings, system_instruction, tool_config, tools); }
    }
}