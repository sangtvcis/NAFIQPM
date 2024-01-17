using System;
using System.Collections.Generic;
using System.Linq; 
using System.Xml.Serialization;
using System.Xml;

namespace Web.Core
{
    public class HelperXml
    {
        /// <summary>
        /// Convert một Obj sang chuỗi string Xml
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeObj2Xml<T>(T obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            try
            {
                var xmlserializer = new XmlSerializer(typeof(T));
                var stringWriter = new StringWriter();
                using (var writer = XmlWriter.Create(stringWriter))
                {
                    xmlserializer.Serialize(writer, obj);
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }
        }
        /// <summary>
        /// Chuyển đổi chuỗi xml sang object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static T DeserializeXml2Obj<T>(string input)
        where T : class
        {
            var ser = new XmlSerializer(typeof(T));
            using (var sr = new StringReader(input))
                return (T)ser.Deserialize(sr);
        }
        /// <summary>
        /// Write một list object sang file Xml
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="filepath"></param>

        public static string SerializeList2Xml<T>(List<T> list)
        {
            //Creating XmlSerializer.
            var serializer = new XmlSerializer(typeof(List<T>));
            var stringWriter = new StringWriter();
            using (var writer = XmlWriter.Create(stringWriter))
            {
                serializer.Serialize(writer, list);
                return stringWriter.ToString();
            }
        }
        /// <summary>
        /// Chuyển đổi 1 file xml sang List Object 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static List<T> Deserialize<T>(string filepath)
            where T : class
        {
            //Creating XmlSerializer for the object
            var serializer = new XmlSerializer(typeof(List<T>));
            //Geting XMl from the file.
            using (var tr = File.OpenText("project.json"))
            {
                //Deserialize back to object from XML
                var b = (List<T>)serializer.Deserialize(tr);
                tr.Close();
                return b;
            }
        }

        public static List<T> DeserializeXml2List<T>(string xml)
            where T : class
        {
            var b = new List<T>();
            if (!string.IsNullOrEmpty(xml))
            {
                var serializer = new XmlSerializer(typeof(List<T>));
                var sr = new StringReader(xml);
                b = (List<T>)serializer.Deserialize(sr);
            }
            return b;
        }
    }
}
