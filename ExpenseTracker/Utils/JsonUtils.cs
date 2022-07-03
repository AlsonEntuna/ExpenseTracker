using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace ExpenseTracker.Utils
{
    public static class JsonUtils
    {
        public static T Deserialize<T>(string pFilePath)
        {
            using (StreamReader file = File.OpenText(pFilePath))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                try
                {
                    JObject o = (JObject)JToken.ReadFrom(reader);
                    return o.ToObject<T>();
                }
                catch (Exception e)
                {
                    throw new Exception(e.ToString());
                }
            }
        }

        public static T DeserializeArray<T>(string pFilePath)
        {
            using (StreamReader file = File.OpenText(pFilePath))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                try
                {
                    JArray o = (JArray)JToken.ReadFrom(reader);
                    return o.ToObject<T>();
                }
                catch (Exception e)
                {
                    throw new Exception(e.ToString());
                }
            }
        }

        public static void Serialize<T>(string pFilePath, T pObject)
        {
            JObject tabData = (JObject)JToken.FromObject(pObject);
            using (StreamWriter file = File.CreateText(pFilePath))
            using (JsonTextWriter writer = new JsonTextWriter(file))
            {
                writer.Formatting = Formatting.Indented;
                tabData.WriteTo(writer);
            }
        }

        public static void SerializeArray<T>(string pFilePath, T pObject)
        {
            JArray tabData = (JArray)JToken.FromObject(pObject);
            using (StreamWriter file = File.CreateText(pFilePath))
            using (JsonTextWriter writer = new JsonTextWriter(file))
            {
                writer.Formatting = Formatting.Indented;
                tabData.WriteTo(writer);
            }
        }
    }
}
