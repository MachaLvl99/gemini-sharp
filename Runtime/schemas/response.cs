using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GeminiSharp {

    /// <summary>
    /// A Response object. 
    /// Contains all the components encapsulating a REST API response from Gemini.
    /// Used for representing the response from a Gemini model as a hard-typed object.
    /// </summary>
    [Serializable]
    public class Response : IResponse {
        [JsonProperty("candidates")] public List<Candidate> candidates { get; set; }
        [JsonProperty("usageMetadata")] public UsageMetadata usageMetadata { get; set; }
    }

    /// <summary>
    /// A Candidate object. 
    /// Contains the content, finish reason, index, and safety ratings of a given response from Gemini.
    /// Used for representing the response from a Gemini model as a hard-typed object.
    /// </summary>
    [Serializable]
    public class Candidate {
        [JsonProperty(Order = 1)] public Content content { get; set; }
        [JsonProperty(Order = 2)] public string finishReason { get; set; }
        [JsonProperty(Order = 3)] public int index { get; set; }
        [JsonProperty(Order = 4)] public List<SafetyRating> safetyRatings { get; set; }
    }

    /// <summary>
    /// A UsageMetadata object.
    /// Contains the token counts for the prompt, the candidates object itself, and total tokens in a given response from Gemini.
    /// Used for representing the response from a Gemini model as a hard-typed object.
    /// </summary>
    [Serializable]
    public class UsageMetadata {
        [JsonProperty(Order = 1)] public int promptTokenCount { get; set; }
        [JsonProperty(Order = 2)] public int candidatesTokenCount { get; set; }
        [JsonProperty(Order = 3)] public int totalTokenCount { get; set; }
    }

    /// <summary>
    /// A SafetyRating object.
    /// Contains the category and safety probability objects from Gemini.
    /// Used for representing the response from a Gemini model as a hard-typed object.
    /// </summary>
    [Serializable]
    public class SafetyRating {
        [JsonProperty(Order = 1)] [JsonConverter(typeof(StringEnumConverter))] public HarmCategory category { get; set; }
        [JsonProperty(Order = 2)] [JsonConverter(typeof(StringEnumConverter))] public Probability probability { get; set; }
    }

    /// <summary>
    /// A FunctionCall response object.
    /// Contains the name of the function and the arguments that Gemini passed to it, represented as a string dictionary of a standard object data type.
    /// Used for representing the response from a Gemini model as a hard-typed object.
    /// This does not represent an actual function call, but rather the data that was passed to it. It is up to you to implement the function call.
    /// </summary>
    [Serializable]
    public class FunctionCall {
        [JsonProperty(Order = 1)] public string name { get; set; }
        [JsonProperty(Order = 2)] public Dictionary<string, object> args { get; set; }
    }

    /// <summary>
    /// An EmbeddingResponse object.
    /// Contains either the embedding struct or an array of embedding structs. Null values will be ignored when (de)serialized.
    /// Used for representing the response from an embedding model as a hard-typed object.
    /// </summary>
    [Serializable]
    public class EmbeddingResponse : IResponse {
        [JsonProperty(Order = 1, NullValueHandling = NullValueHandling.Ignore)] public Embedding embedding { get; set; }
        [JsonProperty(Order = 1, NullValueHandling = NullValueHandling.Ignore)] public Embedding[] embeddings { get; set; }
    }
}