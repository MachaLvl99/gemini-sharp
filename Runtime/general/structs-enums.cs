using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.VisualScripting;

namespace GeminiSharp {
//------------------------------------------------------------------------------------------------------------------------
// ENUMS
//------------------------------------------------------------------------------------------------------------------------
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

    public enum HarmBlockThreshold {
        HARM_BLOCK_THRESHOLD_UNSPECIFIED,
        BLOCK_LOW_AND_ABOVE,
        BLOCK_MEDIUM_AND_ABOVE,
        BLOCK_ONLY_HIGH,
        BLOCK_NONE
    }

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
    [Serializable]
    public struct SafetySetting {
        [JsonProperty(Order = 1)] public HarmCategory category { get; set; }
        [JsonProperty(Order = 2)] public HarmBlockThreshold threshold { get; set; }
        public SafetySetting(HarmCategory category, HarmBlockThreshold threshold) {
            this.category = category;
            this.threshold = threshold;
        }
    }

    [Serializable]
    public struct FunctionCallingConfig {
        [JsonProperty(Order = 1)] public string mode { get; set; }
        [JsonProperty(Order = 2)] public List<string> allowed_function_names { get; set; }
        public FunctionCallingConfig(string mode, List<string> allowed_function_names) {
            this.mode = mode;
            this.allowed_function_names = allowed_function_names;
        }
    }

    [Serializable]
    public struct Content {
        [JsonProperty(Order = 1)] public string role;
        [JsonProperty(Order = 2)] public List<Parts> parts;
    }

    [Serializable]
    public struct Parts {
        [JsonProperty(Order = 1)] public string text;
        public Parts(string text) { this.text = text; }
    }

//------------------------------------------------------------------------------------------------------------------------

}