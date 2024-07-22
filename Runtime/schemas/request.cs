using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GeminiSharp {
    [Serializable]
    public class Request {
        [JsonExtensionData] private IDictionary<string, JToken> _additionalData;
        [JsonIgnore] public SafetySettings safetySettings { get; set; }
        [JsonIgnore] public SystemInstruction system_instruction { get; set; }
        [JsonIgnore] public Contents contents { get; set; }
        [JsonIgnore] public Tool[] tools { get; set; } 
        [JsonIgnore] public ToolConfig tool_config { get; set; }
        public Request(Contents con, SafetySettings safety = null, SystemInstruction sys = null, ToolConfig config = null, params Tool[] tools) {
            this.safetySettings = safety;
            this.system_instruction = sys;
            this.contents = con;
            this.tools = tools;
            this.tool_config = config;
        }

        public string jsonify() { PrepareData(); return JsonConvert.SerializeObject(this); }

        private void PrepareData() {
            _additionalData = new Dictionary<string, JToken>();
            if(safetySettings != null) { _additionalData.Add("safety_settings", JToken.FromObject(safetySettings.safetySettings)); }
            if(system_instruction != null) { _additionalData.Add("system_instruction", JToken.FromObject(system_instruction)); }
            if(contents != null) { _additionalData.Add("contents", JToken.FromObject(contents)); }
            if(tools != null) { _additionalData.Add("tools", JToken.FromObject(tools)); }
            if(tool_config != null) { _additionalData.Add("tool_config", JToken.FromObject(tool_config)); }
        }
    }

    [Serializable]
    public class SystemInstruction {
        [JsonProperty(Order = 1)] public Parts parts;
        public SystemInstruction(string text) { parts = new Parts(text); }
    }

    [Serializable]
    public class SafetySettings {
        [JsonProperty(Order = 1)] public SafetySetting[] safetySettings;
        public SafetySettings(params SafetySetting[] settings) { this.safetySettings = settings; }
    }

    [Serializable]
    public class EmbeddingContent {
        [JsonProperty(Order = 2)] public List<Parts> parts;
        public EmbeddingContent(string text) { parts = new List<Parts> { new Parts(text) }; }
    }

    [Serializable]
    public class Contents {
        [JsonProperty(Order = 1)] public string role;
        [JsonProperty(Order = 2)] public IEnumerable<Parts> parts;
        public Contents(Content items) { role = items.role; parts = items.parts; }
    }

    [Serializable]
    public class Tool {
        [JsonProperty(Order = 1)] public FunctionDeclaration[] function_declarations; //these are the declerations of the functions that can be called

        public Tool(FunctionDeclaration[] functions) {
            this.function_declarations = functions;
        }
    }

    [Serializable]
    public class ToolConfig {
        public FunctionCallingConfig function_calling_config;
        public ToolConfig(FunctionCallingConfig config) { this.function_calling_config = config; }
    }

    [Serializable]
    public class FunctionDeclaration {
        [JsonProperty(Order = 1)] public string name;
        [JsonProperty(Order = 2)] public string description;
        [JsonProperty(Order = 3)] public FunctionParameters parameters;

        public FunctionDeclaration(string name, string desc, FunctionParameters args) {
            this.name = name;
            this.description = desc;
            this.parameters = args;
        }
    }

    [Serializable]
    public class FunctionParameters {
        [JsonProperty(Order = 1)] public string type; //defines the overall data type, ex. object
        [JsonProperty(Order = 2)] public Dictionary<string, Property> properties; //this is a dictionary of the parameters and their data types
        [JsonProperty(Order = 3)] public List<string> required; //the required parameter names mandatory for the function to operate

        public FunctionParameters(string type, Dictionary<string, Property> props, List<string> req) {
            this.type = type;
            this.properties = props;
            this.required = req;
        }
    }

    [Serializable]
    public class Property {
        [JsonProperty(Order = 1)] public string type; //this represents the data type of the parameter
        [JsonProperty(Order = 2)]public string description; //this describes the parameter's purpose and expected format
        public Property(string type, string desc) 
            { this.type = type; this.description = desc; }
    }

    [Serializable]
    public class EnumProperty : Property {
        [JsonProperty(Order = 3, PropertyName = "enum")] public List<string> values; //this is a list of possible values for the parameter, represented as an enum to the LLM
        public EnumProperty(string type, string desc, List<string> vals)  : base(type, desc)
            { this.type = type; this.description = desc; this.values = vals; }
    }

    [Serializable]
    public class ObjectProperty : Property {
        [JsonProperty(Order = 3)] public Dictionary<string, Property> properties; //this is a dictionary of parameters and their data types
        public ObjectProperty(string type, string desc, Dictionary<string, Property> props) : base(type, desc) { this.properties = props; }
    }

    [Serializable]
    public class ArrayProperty : Property {
        [JsonProperty(Order = 3)] public Property items; //this is the data type of an array's elements
        public ArrayProperty(string type, string desc, Property items) : base(type, desc) { this.items = items; }
    }
}