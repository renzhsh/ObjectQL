using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jinhe.Converts
{
    public class EnumConvert: JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            long bValue = (long)value;
            writer.WriteValue(bValue.ToString());
        } 

        public override bool CanConvert(Type objectType)
        {
            if (objectType == typeof(System.Int64) || objectType == typeof(System.Int64?))
            {
                return true;
            }
            return false;
        }

        public bool IsNullableType(Type t)
        {
            if (t == null)
            {
                throw new ArgumentNullException("t");
            }
            return (t.BaseType.FullName == "System.ValueType" && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}