using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GeminiSharp {

    /// <summary>
    /// A Gemini Request object. Contains all the necessary components for a REST API request. 
    /// This is not typically needed, but is provided publicly in case manual construction is necessary.
    /// </summary>
    [Serializable]
    public class Request {
        [JsonExtensionData] private IDictionary<string, JToken> _additionalData;
        [JsonIgnore] public SafetySettings safetySettings { get; set; }
        [JsonIgnore] public SystemInstruction system_instruction { get; set; }
        [JsonIgnore] public Contents contents { get; set; }
        [JsonIgnore] public Tool[] tools { get; set; } 
        [JsonIgnore] public ToolConfig tool_config { get; set; }
        /// <summary>
        /// Constructor for the Request object. The values passed are the objects that will be serialized into a JSON object.
        /// </summary>
        public Request(Contents con, SafetySettings safety = null, SystemInstruction sys = null, ToolConfig config = null, params Tool[] tools) {
            this.safetySettings = safety;
            this.system_instruction = sys;
            this.contents = con;
            this.tools = tools;
            this.tool_config = config;
        }

        ///<summary>
        /// Converts the object into a JSON object string.
        /// </summary>
        public string jsonify() { PrepareData(); return JsonConvert.SerializeObject(this); }

        /// <summary>
        /// This method prepares the data for serialization by placing them inside an IDictionary with a string key and JToken value. 
        /// This must be called before the request is serialized.
        /// </summary>
        private void PrepareData() {
            _additionalData = new Dictionary<string, JToken>();
            if(safetySettings != null) { _additionalData.Add("safety_settings", JToken.FromObject(safetySettings.safetySettings)); }
            if(system_instruction != null) { _additionalData.Add("system_instruction", JToken.FromObject(system_instruction)); }
            if(contents != null) { _additionalData.Add("contents", JToken.FromObject(contents)); }
            if(tools != null) { _additionalData.Add("tools", JToken.FromObject(tools)); }
            if(tool_config != null) { _additionalData.Add("tool_config", JToken.FromObject(tool_config)); }
        }
    }

    /// <summary>
    /// A Chat Request object. Contains the same informations as the Request class, but instead includes a List of Contents objects instead of an individual Contents object.
    /// This is not typically needed, but is provided publicly in case manual construction is necessary.
    /// </summary>
    [Serializable]
    public class ChatRequest {
        [JsonExtensionData] private IDictionary<string, JToken> _additionalData;
        [JsonIgnore] public SafetySettings safetySettings { get; set; }
        [JsonIgnore] public SystemInstruction system_instruction { get; set; }
        [JsonIgnore] public List<Contents> contents { get; set; }
        [JsonIgnore] public Tool[] tools { get; set; } 
        [JsonIgnore] public ToolConfig tool_config { get; set; }
        /// <summary>
        /// Constructor for the ChatRequest object. The values passed are the objects that will be serialized into a JSON object. 
        /// Note this takes a List of Contents objects instead of a single Contents object.
        /// </summary>
        public ChatRequest(List<Contents> con, SafetySettings safety = null, SystemInstruction sys = null, ToolConfig config = null, params Tool[] tools) {
            this.safetySettings = safety;
            this.system_instruction = sys;
            this.contents = con;
            this.tools = tools;
            this.tool_config = config;
        }

        ///<summary>
        /// Converts the object into a JSON object string.
        /// </summary>
        public string jsonify() { PrepareData(); return JsonConvert.SerializeObject(this); }

        /// <summary>
        /// This method prepares the data for serialization by placing them inside an IDictionary with a string key and JToken value. 
        /// This must be called before the request is serialized.
        /// </summary>
        private void PrepareData() {
            _additionalData = new Dictionary<string, JToken>();
            if(safetySettings != null) { _additionalData.Add("safety_settings", JToken.FromObject(safetySettings.safetySettings)); }
            if(system_instruction != null) { _additionalData.Add("system_instruction", JToken.FromObject(system_instruction)); }
            if(contents != null) { _additionalData.Add("contents", JToken.FromObject(contents)); }
            if(tools != null) { _additionalData.Add("tools", JToken.FromObject(tools)); }
            if(tool_config != null) { _additionalData.Add("tool_config", JToken.FromObject(tool_config)); }
        }
    }

    /// <summary>
    /// A System Instruction object. Contains a Part struct containing the text to be sent to the model as a system instruction.
    /// </summary>
    [Serializable]
    public class SystemInstruction {
        [JsonProperty(Order = 1)] public Part parts;
        public SystemInstruction(string text) { parts = new Part(); parts.text = text; }
    }

    /// <summary>
    /// A safety settings object. This contains an array of SafetySetting structs to represent the JSON object passed.
    /// </summary>
    [Serializable]
    public class SafetySettings {
        [JsonProperty(Order = 1)] public SafetySetting[] safetySettings;
        public SafetySettings(params SafetySetting[] settings) { this.safetySettings = settings; }
    }

    /// <summary>
    /// A Contents object. Contains a role string (either user or model) and an IEnumerable of Part structs.
    /// </summary>
    [Serializable]
    public class Contents {
        [JsonProperty(Order = 1)] public string role;
        [JsonProperty(Order = 2)] public IEnumerable<Part> parts;
        public Contents(Content items) { role = items.role; parts = items.parts; }
    }

    /// <summary>
    /// A Tool object. Contains an array of FunctionDeclaration objects.
    /// </summary>
    [Serializable]
    public class Tool {
        [JsonProperty(Order = 1)] public FunctionDeclaration[] function_declarations; //these are the declerations of the functions that can be called

        public Tool(FunctionDeclaration[] functions) {
            this.function_declarations = functions;
        }
    }

    /// <summary>
    /// A ToolConfig object. Contains a FunctionCallingConfig object, which is a struct containing the data to pass in this object instance.
    /// </summary>
    [Serializable]
    public class ToolConfig {
        public FunctionCallingConfig function_calling_config;
        public ToolConfig(FunctionCallingConfig config) { this.function_calling_config = config; }
    }

    /// <summary>
    /// A FunctionDecleration object. Contains the name of the function, a description of it, and a FunctionParameters object.
    /// </summary>
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

    /// <summary>
    /// A FunctionParameters object. 
    /// Contains the data type of the parameters represented as a string, a dictionary of the parameters (as a Property object) and their data types, and a list of required parameters represented as strings.
    /// </summary>
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

    /// <summary>
    /// A Basic Property object. Contains the data type of the parameter and a description of it.
    /// </summary>
    [Serializable]
    public class Property {
        [JsonProperty(Order = 1)] public string type; //this represents the data type of the parameter
        [JsonProperty(Order = 2)]public string description; //this describes the parameter's purpose and expected format
        public Property(string type, string desc) 
            { this.type = type; this.description = desc; }
    }

    /// <summary>
    /// An EnumProperty object. 
    /// Contains the data type of the parameter, a description of it, and a list of possible values for the parameter, represented as an enum to the LLM.
    /// Use this for parameters that have a set list of possible values.
    /// inherits from Property class.
    /// </summary>
    [Serializable]
    public class EnumProperty : Property {
        [JsonProperty(Order = 3, PropertyName = "enum")] public List<string> values; //this is a list of possible values for the parameter, represented as an enum to the LLM
        public EnumProperty(string type, string desc, List<string> vals)  : base(type, desc)
            { this.type = type; this.description = desc; this.values = vals; }
    }

    /// <summary>
    /// An ObjectProperty object.
    /// Contains the data type of the parameter, a description of it, and a dictionary of parameters and their data types.
    /// Use this for parameters that are themselves objects.
    /// Inherits from Property class.
    /// </summary>
    [Serializable]
    public class ObjectProperty : Property {
        [JsonProperty(Order = 3)] public Dictionary<string, Property> properties; //this is a dictionary of parameters and their data types
        public ObjectProperty(string type, string desc, Dictionary<string, Property> props) : base(type, desc) { this.properties = props; }
    }

    /// <summary>
    /// An ArrayProperty object.
    /// Contains the data type of the parameter, a description of it, and the data type of the elements in the array.
    /// Use this for parameters that are arrays.
    /// Inherits from Property class.
    /// </summary>
    [Serializable]
    public class ArrayProperty : Property {
        [JsonProperty(Order = 3)] public Property items; //this is the data type of an array's elements
        public ArrayProperty(string type, string desc, Property items) : base(type, desc) { this.items = items; }
    }
}