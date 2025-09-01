namespace EasyClaimsCore.API.Serialization
{
    public interface ISerializerEngine
    {
        string SerializeAsXml<T>(T obj);
        T Deserialize<T>(string xmlString);
        string FormatXml(string xml);
        bool Success { get; }
        string Message { get; }

        string ConvertXmlToJson<T>(string xml, string rootElement);
    }
}