using System.IO;
using Library.Extensions;

namespace protobuf_excel
{
    public class WriteProto : BaseSystemConsole
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static byte[] RuntimeSerialize<T>(T instance)
        {
            if (instance == null)
            {
                return new byte[0];
            }

            using (var stream = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(stream, instance);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static T RuntimeDeserialize<T>(byte[] bytes)
        {
            if (bytes.Length == 0)
            {
                return default(T);
            }

            using (var stream = new MemoryStream(bytes))
            {
                return ProtoBuf.Serializer.Deserialize<T>(stream);
            }
        }

        /// <summary>
        /// 序列化文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static void RuntimeSerializeFile<T>(string path, T instance)
        {
            using (var file = File.Create(path))
            {
                ProtoBuf.Serializer.Serialize(file, instance);
            }
        }

        /// <summary>
        /// 反序列化文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T RuntimeDeserializeFile<T>(string path)
        {
            using (var file = File.OpenRead(path))
            {
                return ProtoBuf.Serializer.Deserialize<T>(file);
            }
        }

        /// <summary>
        /// 克隆对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public static T Clone<T>(T obj)
        {
            return RuntimeDeserialize<T>(RuntimeSerialize(obj));
        }
    }
}