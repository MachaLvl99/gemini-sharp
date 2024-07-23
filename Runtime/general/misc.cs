using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Linq;

namespace GeminiSharp {

    /// <summary>
    /// This class is used internally to handle properties for certain JSON serialization conditions. Not meant for external use.
    /// </summary>
    public class IgnoreResolver : DefaultContractResolver {
        private readonly Type _targetType;
        private readonly HashSet<string> _propertiesToIgnore;

        public IgnoreResolver(Type targetType, params string[] propertiesToIgnore) {
            _targetType = targetType;
            _propertiesToIgnore = new HashSet<string>(propertiesToIgnore);
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization) {
            var properties = base.CreateProperties(type, memberSerialization);
            if (type == _targetType) { properties = properties.Where(p => !_propertiesToIgnore.Contains(p.PropertyName)).ToList(); }
            return properties;
        }
    }

    /// <summary>
    /// Debug Logger for standard .NET applications.
    /// </summary>
    public class StdLogger : ILogger {
        public void Log(string message) { System.Diagnostics.Debug.WriteLine(message); }
        public void Write(string message) { System.Diagnostics.Debug.Write(message); }
        public void WriteLine(string message) { System.Diagnostics.Debug.WriteLine(message); }
        public void Fail(string message) { System.Diagnostics.Debug.Fail(message); }
        public void Assert(bool condition, string message) { System.Diagnostics.Debug.Assert(condition, message); }
        public void WriteIf(bool condition, string message) { System.Diagnostics.Debug.WriteIf(condition, message); }
        public void WriteLineIf(bool condition, string message) { System.Diagnostics.Debug.WriteLineIf(condition, message); }
    }

    #if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL || UNITY_IOS || UNITY_ANDROID
    /// <summary>
    /// Debug Logger for Unity applications.
    /// </summary>
    public class UnityLogger : IUnityLogger {
        public void Log(string message) { UnityEngine.Debug.Log(message); }
        public void Write(string message) { UnityEngine.Debug.Log(message); }
        public void WriteLine(string message) { UnityEngine.Debug.Log(message); }
        public void Fail(string message) { UnityEngine.Debug.LogError(message); }
        public void Assert(bool condition, string message) { if (!condition) UnityEngine.Debug.LogError(message); }
        public void WriteIf(bool condition, string message) { if (condition) UnityEngine.Debug.Log(message); }
        public void WriteLineIf(bool condition, string message) { if (condition) UnityEngine.Debug.Log(message); }
        public void LogWarning(string message) { UnityEngine.Debug.LogWarning(message); }
        public void LogError(string message) { UnityEngine.Debug.LogError(message); }
    }
    #endif

    /// <summary>
    /// Custom Logger class to allow for logging in both standard .NET and Unity.
    /// </summary>
    public static class Logger {
        private static ILogger _logger;

        /// <summary>
        /// This static constructor initializes the logger based on the platform the application is running on.
        /// </summary>
        static Logger() {
            #if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL || UNITY_IOS || UNITY_ANDROID
            _logger = new UnityLogger();
            #else
            _logger = new StdLogger();
            #endif
        }

        #if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL || UNITY_IOS || UNITY_ANDROID
        public static void Log(string message) { 
            if (_logger is IUnityLogger unityLogger) { unityLogger.Log(message); }
        }
        #else
        public static void Log(string message) { _logger.Log(message); }
        #endif
        
        public static void Write(string message) { _logger.Write(message); }
        public static void WriteLine(string message) { _logger.WriteLine(message); }
        public static void Fail(string message) { _logger.Fail(message); }
        public static void Assert(bool condition, string message) { _logger.Assert(condition, message); }
        public static void WriteIf(bool condition, string message) { _logger.WriteIf(condition, message); }
        public static void WriteLineIf(bool condition, string message) { _logger.WriteLineIf(condition, message); }
        public static void SetLogger(ILogger logger) { _logger = logger; }

        #if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL || UNITY_IOS || UNITY_ANDROID
        public static void LogWarning(string message) {
            if (_logger is IUnityLogger unityLogger) { unityLogger.LogWarning(message); }
        }

        public static void LogError(string message) {
            if (_logger is IUnityLogger unityLogger) { unityLogger.LogError(message); }
        }
        #endif
    }
}