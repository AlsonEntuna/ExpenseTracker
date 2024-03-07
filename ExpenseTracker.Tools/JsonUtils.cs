using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExpenseTracker.Tools
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

        public static T DeserializeFromString<T>(string jsonString)
        {
            try
            {
                JObject o = JObject.Parse(jsonString);
                return o.ToObject<T>();
            }
            catch { return default(T); }
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

        public static T DeserializeArrayFromString<T>(string jsonString)
        {
            try
            {
                JArray o = JArray.Parse(jsonString);
                return o.ToObject<T>();
            }
            catch { return default(T); }
        }    

        public static void Serialize<T>(string pFilePath, T pObject)
        {
            if (pObject == null)
                throw new NullReferenceException("Object to serialize is null.");

            JObject tabData = (JObject)JToken.FromObject(pObject);
            using (StreamWriter file = File.CreateText(pFilePath))
            using (JsonTextWriter writer = new JsonTextWriter(file))
            {
                writer.Formatting = Formatting.Indented;
                tabData.WriteTo(writer);
            }
        }

        public static string SerializeToString<T>(T pObject)
        {
            if (pObject == null) return string.Empty;
            JObject tabData = (JObject)JToken.FromObject(pObject);
            return tabData.ToString();
        }

        public static void SerializeArray<T>(string pFilePath, T pObject)
        {
            if (pObject == null)
                throw new NullReferenceException("Object to serialize is null.");

            JArray tabData = (JArray)JToken.FromObject(pObject);
            using (StreamWriter file = File.CreateText(pFilePath))
            using (JsonTextWriter writer = new JsonTextWriter(file))
            {
                writer.Formatting = Formatting.Indented;
                tabData.WriteTo(writer);
            }
        }

        public static string SerializeArrayToString<T>(T pObject)
        {
            if (pObject == null) return string.Empty;
            JArray tabData = (JArray)JToken.FromObject(pObject);
            return tabData.ToString();
        }

        public static JToken GetJArrayValue(JObject jarray, string key)
        {
            foreach (KeyValuePair<string, JToken> keyValuePair in jarray)
            {
                if (key == keyValuePair.Key)
                {
                    return keyValuePair.Value;
                }
            }
            return null;
        }
    }
}