namespace EasyClaimsCore.API.Security.Cryptography
{
    public interface ICryptoEngine
    {
        string EncryptXmlPayloadData(string xmlPayloadData, string cipherKey, string mimeType = "text/xml");
        string DecryptXmlPayloadData(string encryptedContent, string cipherKey);
        string DecryptRestPayloadData(string encryptedContent, string cipherKey);
        string DecryptRest2PayloadData(string encryptedContent, string cipherKey);
    }
}