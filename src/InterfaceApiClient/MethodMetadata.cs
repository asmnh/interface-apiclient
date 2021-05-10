using InterfaceApiClient.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InterfaceApiClient
{
    internal class MethodMetadata
    {
        public string GroupName { get; }
        public bool IsAsync { get; }
        public bool IsApiMethod { get; }
        public string HttpMethod { get; }

        public Func<object?[], string> BuildPath { get; }
        public Func<object?[], IDictionary<string, string>> BuildQuery { get; }
        public Func<object?[], object?> BuildBody { get; }
        public Func<object?[], IDictionary<string, string>> BuildHeaders { get; }
        public Func<HttpStatusCode, string?, Exception?> BuildResponseException { get; }
        public Type ReturnType { get; }

        public bool HasReturnValue { get { return ReturnType != typeof(void); } }

        internal MethodMetadata(MethodInfo method)
        {
            GroupName = AttributeReader.GetGroupName(method);
            IsAsync = method.ReturnType.IsAssignableTo(typeof(Task));
            ReturnType = IsAsync ? UnpackAsyncReturnType(method.ReturnType) : method.ReturnType;
            ApiEndpointAttribute? endpoint = method.GetCustomAttribute<ApiEndpointAttribute>();
            if(endpoint == null)
            {
                BuildPath = x => "";
                BuildQuery = x => new Dictionary<string, string>();
                BuildBody = x => null;
                BuildHeaders = x => new Dictionary<string, string>();
                HttpMethod = "";
                BuildResponseException = (_, _) => null;
                IsApiMethod = false;
                return;
            }

            IsApiMethod = true;
            HttpMethod = endpoint.HttpMethodString;

            try
            {
                var parameters = method.GetParameters();
                BuildPath = MakePathBuilder(endpoint.Endpoint, parameters);
                BuildQuery = MakeQueryBuilder(parameters);
                BuildBody = MakeBodyBuilder(parameters);
                BuildHeaders = MakeHeaderBuilder(parameters);
                BuildResponseException = MakeResponseExceptionBuilder(method);
            }
            catch(InvalidMetadataException ex)
            {
                throw new InvalidMetadataException(method.DeclaringType!, ex.Endpoint, ex.Message);
            }
        }

        private static Func<HttpStatusCode, string?, Exception?> MakeResponseExceptionBuilder(MethodInfo method)
        {
            var attributes = method.GetCustomAttributes<MapErrorAttribute>();
            Dictionary<HttpStatusCode, Func<string?, Exception?>> callbacks = new();
            foreach(var attr in attributes)
            {
                var type = attr.ExceptionType;
                var exceptionFactory = GetTryDeserialize(type);
                callbacks[attr.StatusCode] = exceptionFactory;
            }
            return (code, data) => callbacks.ContainsKey(code) ? callbacks[code](data) ?? new Exception() : null;
        }

        private static Func<string?, Exception?> GetTryDeserialize(Type type)
        {
            return data =>
            {
                if (data is null) return Activator.CreateInstance(type) as Exception;
                try
                {
                    Exception? e = JsonSerializer.Deserialize(data, type) as Exception;
                    return e;
                }
                catch(System.Text.Json.JsonException)
                {
                    return Activator.CreateInstance(type) as Exception;
                }
            };
        }

        private Type UnpackAsyncReturnType(Type returnType)
        {
            Type taskTemplateType = typeof(Task<>);
            if (returnType.IsConstructedGenericType)
                return returnType.GetGenericArguments()[0];
            else
                return typeof(void);
        }

        private Func<object?[], IDictionary<string, string>> MakeHeaderBuilder(ParameterInfo[] parameters)
        {
            Dictionary<string, Func<object?[], string>> extractors = new();
            for (int i = 0; i < parameters.Length; ++i)
            {
                int idx = i;
                var param = parameters[i];
                InHeaderAttribute[] attrs = param.GetCustomAttributes<InHeaderAttribute>().ToArray();
                foreach (var attr in attrs)
                {
                    string name = ResolveParamName(param, attr);
                    Func<object?, object?> unpacker = Unpack(param.ParameterType, attr.Property);
                    extractors[name] = args => unpacker(args[idx])?.ToString() ?? "";
                }

            }
            return args => args != null ? extractors.ToDictionary(d => d.Key, d => d.Value(args)) : new Dictionary<string, string>();
        }

        // resulting function should return null if there are no body parameters at all
        private Func<object?[], object?> MakeBodyBuilder(ParameterInfo[] parameters)
        {
            // special case: single AsBody attribute without excludes and no InBody attributes - pass through original object
            if(parameters.All(p => !p.GetCustomAttributes<InBodyAttribute>().Any()) && parameters.Sum(p => p.GetCustomAttributes<AsBodyAttribute>().Count()) == 1)
            {
                var param = parameters.Single(p => p.GetCustomAttribute<AsBodyAttribute>() != null);
                var attr = param.GetCustomAttribute<AsBodyAttribute>()!;
                int paramIdx = Array.IndexOf(parameters, param);
                if(!attr.SkipProperties.Any())
                    return args => args[paramIdx];
            }
            // we're making dictionary containing request body and then adding each value to said dictionary
            Dictionary<string, Func<object?[], object?>> valuesResolver = new();
            for(int i = 0; i < parameters.Length; ++i)
            { 
                var param = parameters[i];
                var idx = i;
                var inBodyAttributes = param.GetCustomAttributes<InBodyAttribute>();
                foreach(var bodyAttr in inBodyAttributes)
                {
                    AddInBodyParam(valuesResolver, bodyAttr, idx, param);
                }
                AsBodyAttribute? asBodyAttribute = param.GetCustomAttribute<AsBodyAttribute>();
                if(asBodyAttribute != null)
                {
                    UnpackAsBodyAttribute(valuesResolver, asBodyAttribute, idx, param);
                }
            }
            if (valuesResolver.Count == 0)
                return args => null;
            return args => valuesResolver.ToDictionary(x => x.Key, x => x.Value(args));
        }

        private void UnpackAsBodyAttribute(Dictionary<string, Func<object?[], object?>> valuesResolver, AsBodyAttribute asBodyAttribute, int idx, ParameterInfo param)
        {
            // iterate over fields and properties, add each to resolver
            foreach(var memberInfo in param.ParameterType.GetMembers())
            {
                if (asBodyAttribute.SkipProperties.Contains(memberInfo.Name))
                    continue;
                string name = NormalizeParamName(memberInfo.Name);
                Func<object?, object?> getter;
                if (memberInfo is FieldInfo field)
                {
                    getter = arg => field.GetValue(arg);
                }
                else if (memberInfo is PropertyInfo prop)
                {
                    getter = arg => prop.GetValue(arg);
                }
                else continue;
                valuesResolver[name] = args => getter(args[idx]);
            }
        }

        private void AddInBodyParam(Dictionary<string, Func<object?[], object?>> valuesResolver, InBodyAttribute bodyAttr, int idx, ParameterInfo param)
        {
            var name = NormalizeParamName(ResolveParamName(param, bodyAttr));
            Func<object?, object?> unpacker = Unpack(param.ParameterType, bodyAttr.Property);
            valuesResolver[name] = args => unpacker(args[idx]);
        }

        private Func<object?[], IDictionary<string, string>> MakeQueryBuilder(ParameterInfo[] parameters)
        {
            Dictionary<string, Func<object?[], string>> extractors = new();
            for (int i = 0; i < parameters.Length; ++i)
            {
                int idx = i;
                var param = parameters[i];
                InQueryAttribute[] attrs = param.GetCustomAttributes<InQueryAttribute>().ToArray();
                foreach(var attr in attrs)
                {
                    string name = NormalizeParamName(ResolveParamName(param, attr));
                    Func<object?, object?> unpacker = Unpack(param.ParameterType, attr.Property);
                    extractors[name] = args => unpacker(args[idx])?.ToString() ?? "";
                }

            }
            return args => args != null ? extractors.ToDictionary(d => d.Key, d => d.Value(args)) : new Dictionary<string, string>();
        }

        private Func<object?[], string> MakePathBuilder(string endpoint, ParameterInfo[] parameters)
        {
            static string Unwrap(string arg) => arg.Replace("{", "").Replace("}", "");
            Regex pathRegex = new Regex("\\{(?:[A-Za-z0-9]+)\\}");
            var matches = pathRegex.Matches(endpoint);
            var matchedArgs = matches.AsEnumerable().Select(m => Unwrap(m.Groups[0].Value)).ToArray();
            Dictionary<string, Func<object?[], string>> fills = new();
            for(int i = 0; i < parameters.Length; ++i)
            {
                var param = parameters[i];
                InPathAttribute[] attrs = param.GetCustomAttributes<InPathAttribute>().ToArray();
                foreach (var attr in attrs)
                {
                    string name = NormalizeParamName(ResolveParamName(param, attr));
                    int idx = i;
                    Func<object?, object?> unpacker = Unpack(param.ParameterType, attr.Property);
                    fills[name] = objs => unpacker(objs[idx])?.ToString() ?? "";
                }
            }
            var missing = matchedArgs.Where(a => !fills.ContainsKey(a)).ToArray();
            if (missing.Any())
                throw new InvalidMetadataException(typeof(MethodMetadata), endpoint, $"Missing required path parameters: {string.Join(", ", missing)}");
            return args =>
                Regex.Replace(endpoint, "\\{(?:[A-Za-z0-9]+)\\}", match => fills[Unwrap(match.Groups[0].Value)](args));
        }

        private Func<object?, object?> Unpack(Type paramType, string? property)
        {
            if (property is null)
                return data => data;
            string[] parts = property.Trim().Split('.');
            Type cursor = paramType;
            Func<object?, object?> currentResult = data => data;
            foreach(var partName in parts)
            {
                string part = partName;
                MemberInfo[] members = cursor.GetMember(part, BindingFlags.Public|BindingFlags.Instance)
                    .Where(member => member is PropertyInfo || member is FieldInfo)
                    .ToArray();
                if (members.Length == 0)
                    throw new KeyNotFoundException($"Member {part} was not found in type {cursor}");
                MemberInfo member = members.Single();
                var prevResult = currentResult;
                if(member is FieldInfo fieldInfo)
                {
                    currentResult = data => { 
                        object? d = prevResult(data); return d != null ? fieldInfo.GetValue(d) : null; 
                    };
                    cursor = fieldInfo.FieldType;
                }
                else if (member is PropertyInfo propInfo)
                {
                    currentResult = data => {
                        object? d = prevResult(data); return d != null ? propInfo.GetValue(d) : null; 
                    };
                    cursor = propInfo.PropertyType;
                }
            }
            return currentResult;
        }

        private string NormalizeParamName(string name)
        {
            return StringUtilities.ToCamelCase(name.Trim());
        }

        private string ResolveParamName(ParameterInfo param, RequestAttribute attr)
        {
            string? result = attr.Name ?? attr.Property?.Split('.')?.LastOrDefault() ?? param.Name;
            if (result is null)
                throw new ArgumentNullException(nameof(attr), $"Attribute {attr} can't have a name match for {param}");
            return result;
        }
    }
}