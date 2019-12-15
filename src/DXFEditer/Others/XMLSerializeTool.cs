using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace DXFViewer
{
    /// <summary>
    ///     XML序列化工具
    /// </summary>
    public class XMLSerializeTool<T>
    {
        public static XMLSerializeTool<T> Instance = new XMLSerializeTool<T>();

        private readonly XmlSerializer serializer;

        private XMLSerializeTool()
        {
            serializer = new XmlSerializer(typeof(T));
        }

        /// <summary>
        ///     从文件加载
        /// </summary>
        /// <param name="fileName">包含绝对路径的文件名</param>
        public T Load(string fileName)
        {
            if(!File.Exists(fileName))
            {
                string path = fileName;
                XmlDocument xmlDocument = new XmlDocument();
                XmlDeclaration xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
                xmlDocument.AppendChild(xmlDeclaration);
                XmlElement rootElement = xmlDocument.CreateElement(typeof(T).Name);
                xmlDocument.AppendChild(rootElement);
                xmlDocument.Save(path);
            }
            using (var fileStream = new FileStream(fileName, FileMode.Open))
            {
                return Load(fileStream);
            }
        }

        /// <summary>
        ///     从流加载
        /// </summary>
        /// <param name="stream">数据流</param>
        public T Load(Stream stream)
        {
            var reader = new XmlTextReader(stream);
            return Load(reader);
        }

        /// <summary>
        ///     从 XmlTextReader 加载
        /// </summary>
        /// <param name="reader">XmlTextReader的实例</param>
        public T Load(XmlTextReader reader)
        {
            return (T)serializer.Deserialize(reader);
        }

        /// <summary>
        ///     序列化对象到文件
        /// </summary>
        /// <param name="fileName">包含绝对路径的文件名</param>
        /// <param name="configObject">需保存的对象</param>
        public void Save(string fileName, T configObject)
        {
            using (var fileStream = new FileStream(fileName, FileMode.Create))
            {
                Save(fileStream, configObject);
            }
        }

        /// <summary>
        ///     序列化对象到数据流
        /// </summary>
        /// <param name="stream">数据流</param>
        /// <param name="configObject">需保存的对象</param>
        public void Save(Stream stream, T configObject)
        {
            // UTF8编码 换行符
            var writer = new XmlTextWriter(stream, Encoding.UTF8) { Formatting = Formatting.Indented };
            Save(writer, configObject);
        }

        /// <summary>
        ///     序列化对象到 XmlTextWriter
        /// </summary>
        /// <param name="writer">XmlTextWriter的实例</param>
        /// <param name="configObject">需保存的对象</param>
        public void Save(XmlTextWriter writer, T configObject)
        {
            var xmlns = new XmlSerializerNamespaces();
            xmlns.Add(String.Empty, string.Empty);
            serializer.Serialize(writer, configObject, xmlns);
        }
    }
}