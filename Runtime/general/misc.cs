using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Linq;

namespace GeminiSharp {
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
}