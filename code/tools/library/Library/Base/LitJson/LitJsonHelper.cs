using LitJson;

namespace Library.LitJson
{
    public class LitJsonHelper
    {
        static LitJsonHelper()
        {
            JsonMapper.RegisterImporter<double, float>((double input) => (float)input);
            JsonMapper.RegisterExporter<float>(delegate(float v, JsonWriter w)
            {
                w.Write((double)v);
            });
        }

        public static T ToObject<T>(string res)
        {
            return JsonMapper.ToObject<T>(res);
        }

        public static JsonData ToObject(string res)
        {
            return JsonMapper.ToObject(res);
        }

        public static string ToJson<T>(T t)
        {
            return JsonMapper.ToJson(t);
        }
    }
}
