# Gemini# 
## A community-built asyncronous C# library for Gemini APIs.

Originally built as a personal tool for my Unity project, this library supports both Unity and standard .NET usage. Supported features and models are listed below. This was born out of a passion project, and continues to be a work in progress. 
If you wish to contribute to this library, please see the Contributing section below.

### Supported Models:
- Gemini 1.5 Pro
- Gemini 1.5 Flash
- Text Embedding 004

### Supported Features:
- Text Generation (single-shot & chat prompting)
- Text Embedding
- System Instructions
- Function Calling
- Safety & Tool Settings

### To-Do (Not Yet Supported):
- Multi-Modal Input
- Text Streaming
- Caching

## Installation

To install this package in Unity, go to Window -> Package Manager, and click the '+' button to choose "Install package from git URL"...

![Screenshot 2024-07-23 021958](https://github.com/user-attachments/assets/f4e563ad-42aa-43e6-9d16-6984a64d7a02)

Paste this repo's URL in the line displayed

`https://github.com/MachaLvl99/gemini-sharp.git`

*Et voila!* This should automatically install the necessary dependencies, which in this case is just Newtonsoft's serialization library.

Now that we have that out of the way, we're ready to rumble!

## Using Gemini#

First thing's first, don't forget to set the library with your other libraries in your script.

`using GeminiSharp`

Everything you need is contained into one simple namespace.

Before you get started, make sure to grab your API Key from Google as well. Instructions for generating and retrieving your Gemini API Key can be found here:

https://ai.google.dev/gemini-api/docs/api-key

### NOTE: Do NOT hard-code your API Key into your scripts. 
This is an **extremely** dangerous mistake in Unity, because Unity game applications are typically given **client-side**. This means if you send your game out to your friends, and you placed your API key directly in your script, you just **gave away your API Key to everyone who has your application**. 

Instructions for setting up a workaround in Unity is **TBD**. It is currently beyond the scope of this initial launch.

### Easy, Breezy, Beautiful, Gemini#.

This library was designed to be as simple to use as possible while still being extremely powerful and flexible. Object-Oriented-Programming is well suited for these kinds of API calls.

Each language model is treated as its own object. To set up a model for use, simply construct a new object and pass through your API KEy.

```
void Start() {
  GeminiFlash Flash = new GeminiFlash(apikey); // Gemini 1.5 Flash
  GeminiPro Pro = new GeminiPro(apikey); // Gemini 1.5 Pro
  TextEmbedding TestEmbed = new TextEmbedding(apikey, "Place the text you want to embed here!"); //Text Embedding - 004
  BatchEmbedding batch = new BatchEmbedding(apikey, ListOfStringsToEmbed); //Text Embedding -004 Batch
}
```
This library handles all the messy serialization and deserialization for you automatically. All you need to do is bring your `string` query, methods for handling the responses, and run a coroutine! 
In order to do so, you must create the task first. Gemini Pro/Flash return a `Task<Response>`, while embedding models return a `Task<EmbeddingResponse>`.

There are a couple types of tasks you can run, but if you want to produce a single quick and easy query, the simplest way to do so is by using the `SingleShot(string your_query_here)` method for single-shot prompting. Simply call the `.Run()` method within the model object to execute the task. You must place this method call inside the `StartCoroutine` method in Unity (this tutorial does not cover standard .NET, but otherwise handle this how you would handle any other async task). Forgetting to do so will cause problems.

```
void Start() {
  GeminiPro Pro = new GeminiPro(apikey);

  Task<Response> task = Pro.SingleShot("Hello World!");
  StartCoroutine(Pro.Run(task, OnSuccess, OnFailure));
}
```
The methods `OnSuccess/Failure` are methods that you create to handle the responses however you'd like. This is where you can actually retrieve the model's repsonse and do stuff with it.

```
private void OnSuccess(Response response) {
  Debug.Log(response.candidates[0].content.parts[0].text);
}

private void OnFailure(Exception e) {
  Debug.LogError("Error: " + e.Message);
}

private void OnEmbeddingSuccess(EmbeddingResponse response) {
  Debug.Log(response.embeddings);
}
```
Make sure these methods are place within the class you're calling the model from (or inside one of Unity's default methods).

And that's it! That's all you need to make a minimal API call to Gemini in C#!

### You Can Chat Too!

Gemini Pro & Flash model objects support Chat-style prompting as well with the `.Chat()` method. This method accepts an array of `(string, string)` tuples. The first string represents the `role` (who generated the text), while the second string represents the `text` itself. The order of the tuple array represents the order of each chat component. 
```
var chats = new (string role, string text)[] { 
  ("user", query),
  ("model", modelResp),
  ("user", query2) 
};

Task<Response> task = Pro.Chat(chats);
StartCoroutine(Pro.Run(task, OnSuccess, OnFailure));
```
### Embeddings are even Easier

Want to send some text over to embed? Just use the `Send()` method. This does not require any input parameters; This embeds the text of whatever text you instantiated the embedding model with. This works for both a single embedding call and a batch embedding.

```
TextEmbedding embed = new TextEmbedding(apikey, "Honk!");
Task<EmbeddingResponse> eTask = embed.Send();
StartCoroutine(embed.Run(eTask, OnEmbeddingSuccess, OnFailure));
```
### Add Stuff At-Will

This library contains class & struct definitions for nearly every possible JSON object you can pass and receive to Gemini. This includes safety settings, tool configurations, and system instructions.

Gemini# serializes these objects upon the execution of the task when you start the coroutine. These extra objects are entirely optional; Gemini# handles everything you choose not to provide automatically. 

The Enums, structs, and classes in Gemini# are identical to the same objects that you would pass to the Gemini API. This means that the safetySettings variable in the model object for example, is a safetySettings class that can be either an array of safetySetting objects, an individual safetySetting object, or nothing at all.

Refer to Google's documentation for further explanations of their JSON object schemas.

```
SafetySetting setting = new SafetySetting(HarmCategory.HARM_CATEGORY_HARASSMENT, HarmBlockThreshold.BLOCK_NONE);

SystemInstruction instruct = new SystemInstruction("You are a mischevous goose. You like to cause as much chaos as possible in a small town. Honk repeatedly.");

ToolConfig toolSetting = new ToolConfig(new FunctionCallingConfig("ANY", new List<string> { "some_function" }));

Pro.safetySettings = new SafetySettings(setting);
Pro.system_instruction = instruct;
Pro.tool_config = toolSetting;

```

### Build your own tools! (intermediate)

In order to produce a function call, You need to pass what is essentially the JSON schema representation of that function, known as a function decleration. In a language like C#, this means you have to manually construct that schema yourself to then serialize and pass to the API. This can become rather complex and burdensome. Thankfully, Gemini# has built-in classes to make this as easy and straightforward as possible. We can thank object inheritance for simplifying this process.

The core of each function declaration you produce is the `Property` object, which represents a parameter that you, the developer, need Gemini, the language model, to give to you. This object includes the type of parameter that it is, and a description of it.

```
Property item = new Property("string", "This is a description of the property.");
```
Properties can include pretty much anything in a JSON object schema, including other properties. This is fine for JSON; this is *not* fine for languages like C#. You can end up in a recursive loop of doom, death, and destruction. To prevent the recursive apocalyse, Gemini# includes 3 separate classes that all inherit from the base `Property` class. They are `ObjectProperty`, `ArrayProperty`, and `EnumProperty`. They are what it says on the box; each of these properties exist so they can include other properties that may or may not be sets of some kind. As Google's documentation states, it is recommended to pass an enum to Gemini (or pass what Gemini *thinks* is an enum) to reduce model hallucination. Hence, we have EnumProperty for precisely that use case.

```
EnumProperty enumItem = new EnumProperty("string", "This is a property with a set of specified values.", new List<string> { "value1", "value2" });
ArrayProperty arrayItem = new ArrayProperty("array", "This is an array of stuff.", enumItem);
ObjectProperty objItem = new ObjectProperty("object", "This is an object with properties.", new Dictionary<string, Property> { { "property_type", item }, { "property_type2", enumItem } });
```

After you define what parameters you'd like your function decleration to have, you can now construct the FunctionParameters object.

```
FunctionParameters arguments = new FunctionParameters(
  "object",
  new Dictionary<string, Property> {
    { "property1", item },
    { "property2", arrayItem },
    { "property3", objItem }
  },
  new List<string> { "property1", "property2" }
);
```
Thanks to object inheritance, we can pass in different kinds of properties in the dictionary above because they all inherit from the base class `Property`. Gemini likes the properties you pass to it bundled up as an object, so it is recommended to set the first parameter in this constructor to `object`, and then set all your parameters/properties inside the dictionary you pass to it. The final parameter, the list of strings, represents the `required` parameters necessary for your function to work. You are free to construct your properties in whatever way works for you and the function you want to call. This structure was meant to provide as much flexibility as possible without breaking things. Be warned that Function Calling is still in Beta, and if there are going to be problems, it is very likely going to be in how you create your properties here. Refer to the source code for further details.

Once you've constructed your arguments, you can then construct the  `FunctionDecleration` object, Which you can use to place into the language model object.  These classes are meant to represent the JSON schema Gemini accepts, so while you may notice some unecessary arrays in some places, this is meant to reflect the JSON schema of what we want to pass. 

```
FunctionDeclaration Func = new FunctionDeclaration(
  "some_function", 
  "This function does something.", 
  arguments
);

Pro.tools = new Tool[] { new Tool(new FunctionDeclaration[] { Func }) };
```

And with that, you have officially given Gemini a tool it can use.

Whether you pass a tool or not, it does not change how you would retrieve the response or generate a request to the API. This is because the `Part` struct (the "substance" nested deep inside a request/response) can be either `text` *or* a `functionCall` object. Gemini# handles both cases. It us up to you to handle this data once it is given to you as a `Response` object (inside the method you pass when you `Run` the coroutine).

## Contributing

This repo is built by the community, for the community! If you would like to contribute to this project, please follow these steps:

1. Fork the repository to your own GitHub account.
2. Create a new branch for your changes.
3. Make your changes in this new branch.
4. Test your changes thoroughly.
5. Submit a pull request with a clear description of your changes and the motivation behind them.

By submitting a pull request, you agree that your contributions will be licensed under the same license as this project.

Thank you for your helping make this repo better!
