using LitJson.P;
using System;

namespace findText.Script
{
    public abstract class ActionBaseUnity : BaseActionFor
    {
        protected override void OpenRun(string file)
        {
            throw new NotImplementedException();
        }

        protected override string ToJson(object json)
        {
            return JsonMapper.ToJson(json);
        }

        internal static class Program2
        {

            [Serializable]
            public class Test
            {
                public string fffff { get; set; }
                public vector3 aaa { get; set; }
                public vector[] vector { get; set; }
            }

            [Serializable]
            public class vector
            {
                public float x { get; set; }
                public float y { get; set; }
                public float z { get; set; }
            }

            [Serializable]
            public struct vector3
            {
                public float x { get; set; }
                public float y { get; set; }
                public float z { get; set; }
            }

            private static void Main(string[] args)
            {
                var serializer = new YamlDotNet.Serialization.SerializerBuilder().Build();
                var str = serializer.Serialize(new Test()
                {
                    fffff = "ddddddddddd",
                    aaa = new vector3 { x = 1, y = 6, z = 8 },
                    vector = new vector[] { new vector { x = 1, y = 6, z = 8 }, new vector { x = 1, y = 6, z = 8 }, new vector { x = 1, y = 6, z = 8 } },
                });


                Console.WriteLine(str);
            }
        }
    }
}
