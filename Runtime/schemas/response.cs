using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;

namespace GeminiSharp {
    [Serializable]
    public class Response {
        [JsonProperty("candidates")] public List<Candidate> candidates { get; set; }
        [JsonProperty("usageMetadata")] public UsageMetadata usageMetadata { get; set; }
    }

    [Serializable]
    public class Candidate {
        [JsonProperty(Order = 1)] public Content content { get; set; }
        [JsonProperty(Order = 2)] public string finishReason { get; set; }
        [JsonProperty(Order = 3)] public int index { get; set; }
        [JsonProperty(Order = 4)] public List<SafetyRating> safetyRatings { get; set; }
    }

    [Serializable]
    public class UsageMetadata {
        [JsonProperty(Order = 1)] public int promptTokenCount { get; set; }
        [JsonProperty(Order = 2)] public int candidatesTokenCount { get; set; }
        [JsonProperty(Order = 3)] public int totalTokenCount { get; set; }
    }

    [Serializable]
    public class SafetyRating {
        [JsonProperty(Order = 1)] [JsonConverter(typeof(StringEnumConverter))] public HarmCategory category { get; set; }
        [JsonProperty(Order = 2)] [JsonConverter(typeof(StringEnumConverter))] public Probability probability { get; set; }
    }
}