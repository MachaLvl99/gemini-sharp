using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GeminiSharp {
//------------------------------------------------------------------------------------------------------------------------
// ENUMS
//------------------------------------------------------------------------------------------------------------------------
    
    /// <summary>
    /// HarmCategory enumerator. Mirrors Gemini's enum for consistency, but be warned this includes values Gemini does not accept. Use this for declaring safety settings.
    /// The following values may be used with Gemini models: HARM_CATEGORY_HARASSMENT(7), HARM_CATEGORY_HATE_SPEECH(8), HARM_CATEGORY_SEXUALLY_EXPLICIT(9), HARM_CATEGORY_DANGEROUS_CONTENT(10)
    /// </summary>
    public enum HarmCategory {
        HARM_CATEGORY_UNSPECIFIED,
        HARM_CATEGORY_DEROGATORY, //NOT USED BY GEMINI. Used to mirror Google's enum for consistency ONLY
        HARM_CATEGORY_TOXICITY, //NOT USED BY GEMINI. Used to mirror Google's enum for consistency ONLY
        HARM_CATEGORY_VIOLENCE, //NOT USED BY GEMINI. Used to mirror Google's enum for consistency ONLY
        HARM_CATEGORY_SEXUAL, //NOT USED BY GEMINI. Used to mirror Google's enum for consistency ONLY
        HARM_CATEGORY_MEDICAL, //NOT USED BY GEMINI. Used to mirror Google's enum for consistency ONLY
        HARM_CATEGORY_DANGEROUS, //NOT USED BY GEMINI. Used to mirror Google's enum for consistency ONLY
        HARM_CATEGORY_HARASSMENT,
        HARM_CATEGORY_HATE_SPEECH,
        HARM_CATEGORY_SEXUALLY_EXPLICIT,
        HARM_CATEGORY_DANGEROUS_CONTENT
    }

    /// <summary>
    /// HarmBlockThreshold enumerator. Mirrors Gemini's enum for consistency. This is sets the value for the specified HarmCategory.
    /// </summary>
    public enum HarmBlockThreshold {
        HARM_BLOCK_THRESHOLD_UNSPECIFIED,
        BLOCK_LOW_AND_ABOVE,
        BLOCK_MEDIUM_AND_ABOVE,
        BLOCK_ONLY_HIGH,
        BLOCK_NONE
    }

    /// <summary>
    /// Probability enumerator. This represents the expected safety probability result of a given response from Gemini.
    /// </summary>
    public enum Probability {
        NEGLIGIBLE,
        LOW,
        MEDIUM,
        HIGH,
        EXTREME
    }

//------------------------------------------------------------------------------------------------------------------------

//------------------------------------------------------------------------------------------------------------------------
// STRUCTS & DATA TYPES
//------------------------------------------------------------------------------------------------------------------------
    
    /// <summary>
    /// SafetySetting struct. Use this to set the safety settings for a Gemini model. It enum values for consistency with Gemini's API. integers representing the enum values are also accepted.
    /// </summary>
    [Serializable]
    public struct SafetySetting {
        [JsonProperty(Order = 1)] public HarmCategory category { get; set; }
        [JsonProperty(Order = 2)] public HarmBlockThreshold threshold { get; set; }
        public SafetySetting(HarmCategory category, HarmBlockThreshold threshold) {
            this.category = category;
            this.threshold = threshold;
        }
    }

    /// <summary>
    /// Function calling configuration struct. Use this to set the MODE paramater and to provide the names of functions that Gemini can call.
    /// Function declerations for each provided function must be provided in the Tool object when a request is passed to Gemini.
    /// </summary> 
    [Serializable]
    public struct FunctionCallingConfig {
        [JsonProperty(Order = 1)] public string mode { get; set; }
        [JsonProperty(Order = 2)] public List<string> allowed_function_names { get; set; }
        public FunctionCallingConfig(string mode, List<string> allowed_function_names) {
            this.mode = mode;
            this.allowed_function_names = allowed_function_names;
        }
    }


    /// <summary>
    /// Content struct representing the content(s) objects both passed to and received from Gemini models.
    /// </summary>
    [Serializable]
    public struct Content {
        [JsonProperty(Order = 1, NullValueHandling = NullValueHandling.Ignore)] public string role;
        [JsonProperty(Order = 2)] public List<Part> parts;
    }

    /// <summary>
    /// Part struct representing the "substance" of what data is passed and received. Can include either text or a function call.
    /// Null values are ignored by the JSON serializer.
    /// </summary>
    [Serializable]
    public struct Part {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public string text;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public FunctionCall functionCall;
    }

    /// <summary>
    /// An Embedding struct.
    /// Contains an array of floats representing the embedding of a given text.
    /// </summary>
    [Serializable]
    public struct Embedding {
        [JsonProperty(Order = 1)] public float[] values { get; set; }
    }

//------------------------------------------------------------------------------------------------------------------------


//------------------------------------------------------------------------------------------------------------------------
// INTERFACES
//------------------------------------------------------------------------------------------------------------------------
    
    /// <summary>
    /// empty interface for Response objects. Used to ensure that all Response objects are of the same type.
    /// </summary>
    public interface IResponse {}

    /// <summary>
    /// Logging interface to allow the logs (and the library) to be usable in both standard .NET and Unity.
    /// </summary>
    public interface ILogger { 
        void Log(string message);
        void Write(string message);
        void WriteLine(string message);
        void Fail(string message);
        void Assert(bool condition, string message);
        void WriteIf(bool condition, string message);
        void WriteLineIf(bool condition, string message); 
    }

    public interface IUnityLogger : ILogger {
        void LogWarning(string message);
        void LogError(string message);
    }

//------------------------------------------------------------------------------------------------------------------------
}