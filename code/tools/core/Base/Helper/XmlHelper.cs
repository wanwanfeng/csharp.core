using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Library.Helper
{
    public class XmlHelper
    {
        #region xml 序列化

        /// <summary>
        /// 对象序列化成 xml string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize<T>(T obj)
        {
            XmlSerializer xmlserializer = new XmlSerializer(typeof (T));
            using (MemoryStream ms = new MemoryStream())
            {
                xmlserializer.Serialize(ms, obj);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        /// <summary>
        /// 对象序列化成 xml string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize(object obj)
        {
            XmlSerializer xmlserializer = new XmlSerializer(obj.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                xmlserializer.Serialize(ms, obj);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }


        /// <summary>
        /// xml string 反序列化成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlstring"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string xmlstring)
        {
            return (T) Deserialize(typeof (T), xmlstring);
        }

        /// <summary>
        /// xml string 反序列化成对象
        /// </summary>
        /// <param name="type"></param>
        /// <param name="xmlstring"></param>
        /// <returns></returns>
        public static object Deserialize(Type type, string xmlstring)
        {
            XmlSerializer xmlserializer = new XmlSerializer(type);
            using (MemoryStream xmlstream = new MemoryStream(Encoding.UTF8.GetBytes(xmlstring)))
            {
                using (XmlReader xmlreader = XmlReader.Create(xmlstream))
                {
                    return xmlserializer.Deserialize(xmlreader);
                }
            }
        }

        #endregion


        public static string ToXml<T>(T obj)
        {
            return Serialize(obj);
        }

        public static T ToObject<T>(string str)
        {
            return Deserialize<T>(str);
        }
    }
}
