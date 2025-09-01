using Newtonsoft.Json;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Formatting = Newtonsoft.Json.Formatting;

namespace EasyClaimsCore.API.Serialization
{
    public class SerializerEngine : ISerializerEngine
    {
        private readonly ILogger<SerializerEngine> _logger;

        public bool Success { get; private set; } = true;
        public string Message { get; private set; } = string.Empty;

        public SerializerEngine(ILogger<SerializerEngine> logger)
        {
            _logger = logger;
        }

        public string SerializeAsXml<T>(T obj)
        {
            try
            {
                Success = true;
                Message = string.Empty;

                if (obj == null)
                    throw new ArgumentNullException(nameof(obj));

                var serializer = new XmlSerializer(typeof(T));
                var settings = new XmlWriterSettings
                {
                    OmitXmlDeclaration = false,
                    Indent = true,
                    Encoding = Encoding.UTF8
                };

                using var stringWriter = new StringWriter();
                using var xmlWriter = XmlWriter.Create(stringWriter, settings);

                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add("", "");

                serializer.Serialize(xmlWriter, obj, namespaces);
                return stringWriter.ToString();
            }
            catch (Exception ex)
            {
                Success = false;
                Message = ex.Message;
                _logger.LogError(ex, "Error occurred during XML serialization");
                throw;
            }
        }

        public T Deserialize<T>(string xmlString)
        {
            try
            {
                Success = true;
                Message = string.Empty;

                if (string.IsNullOrEmpty(xmlString))
                    throw new ArgumentException("XML string cannot be null or empty", nameof(xmlString));

                var serializer = new XmlSerializer(typeof(T));
                using var stringReader = new StringReader(xmlString);
                var result = (T?)serializer.Deserialize(stringReader);

                if (result == null)
                    throw new InvalidOperationException("Deserialization returned null");

                return result;
            }
            catch (Exception ex)
            {
                Success = false;
                Message = ex.Message;
                _logger.LogError(ex, "Error occurred during XML deserialization");
                throw;
            }
        }

        public string FormatXml(string xml)
        {
            try
            {
                Success = true;
                Message = string.Empty;

                if (string.IsNullOrEmpty(xml))
                    return xml;

                var doc = XDocument.Parse(xml);
                return doc.ToString();
            }
            catch (Exception ex)
            {
                Success = false;
                Message = ex.Message;
                _logger.LogError(ex, "Error occurred during XML formatting");
                return xml;
            }
        }

        public string ConvertXmlToJson<T>(string xml, string rootElement)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(rootElement));
            using (StringReader reader = new StringReader(xml))
            {
                var obj = (T)serializer.Deserialize(reader);
                return JsonConvert.SerializeObject(obj, Formatting.Indented);
            }
        }
    }
}