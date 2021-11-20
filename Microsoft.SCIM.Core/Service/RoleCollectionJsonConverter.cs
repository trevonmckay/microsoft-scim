using AirCo.AzureAD.Licensing;
using Newtonsoft.Json;
using System;

namespace Microsoft.SCIM.Service
{
    public class RoleCollectionJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            RoleCollection result = new();
            bool inValue = false;
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        if (reader.Depth == 2 && reader.Value is string valueString && string.Equals("value", valueString, StringComparison.OrdinalIgnoreCase))
                        {
                            inValue = true;
                        }
                        break;
                    case JsonToken.String:
                        if (inValue)
                        {
                            string jsonValue = reader.Value as string;
                            Role role = JsonConvert.DeserializeObject<Role>(jsonValue);
                            result.Add(role);
                        }
                        break;
                    case JsonToken.EndObject:
                        if (inValue)
                        {
                            inValue = false;
                        }
                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
