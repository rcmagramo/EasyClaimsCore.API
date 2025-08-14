using EasyClaimsCore.API.Security.Cryptography.DataContracts;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace EasyClaimsCore.API.Security.Cryptography
{
    public class CryptoEngine : ICryptoEngine
    {
        const int PASSWORD1_LEN = 16;
        const int PASSWORD2_LEN = 16;
        const int DOC_IV_LEN = 16;
        const int PASSWORD_LEN = PASSWORD1_LEN + PASSWORD2_LEN;
        const int XML_IV_LEN = 16;

        private readonly ILogger<CryptoEngine> _logger;

        public CryptoEngine(ILogger<CryptoEngine> logger)
        {
            _logger = logger;
        }

        //public string EncryptXmlPayloadData(string xmlPayloadData, string cipherKey, string mimeType = "text/xml")
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(xmlPayloadData))
        //            throw new ArgumentException("XML payload cannot be null or empty string!");

        //        var data = Encoding.UTF8.GetBytes(xmlPayloadData);
        //        var password = GeneratePassword(cipherKey);
        //        var iv = GetRandomBytes(XML_IV_LEN);
        //        var encryptedData = EncryptUsingAES(data, password, iv);

        //        if (encryptedData.Length > 0)
        //        {
        //            var ivBase64 = Convert.ToBase64String(iv);
        //            var sha256Hash = ComputeHashUsingSHA256(data);
        //            var encryptedDataBase64 = Convert.ToBase64String(encryptedData);

        //            var payload = new Payload
        //            {
        //                DocMimeType = mimeType,
        //                Hash = sha256Hash,
        //                Key1 = "",
        //                Key2 = "",
        //                IV = ivBase64,
        //                Doc = encryptedDataBase64
        //            };

        //            return JsonConvert.SerializeObject(payload);
        //        }

        //        return "";
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Cryptography exception occurred during encryption");
        //        throw new ApplicationException($"Cryptography exception occurred! {ex.Message}");
        //    }
        //}

        public string EncryptXmlPayloadData(string xmlPayloadData, string cipherKey, string mimeType = "text/xml")
        {
            try
            {
                if (string.IsNullOrEmpty(xmlPayloadData)) throw new Exception("XML payload cannot be null or empty string!");

                //Log("Encryption process started.");
                var encryptedContent = "";

                //Log("Translating XML payload to bytes...");
                var data = Encoding.UTF8.GetBytes(xmlPayloadData);

                var password = GeneratePassword(cipherKey);

                //Log($"Generating {XML_IV_LEN} random bytes for initialization vector for AES encryption...");
                var iv = GetRandomBytes(XML_IV_LEN);

                //Log("Encrypting XML Payload using 'AES-256-CBC'...");
                var encryptedData = EncryptUsingAES(data, password, iv);

                if (encryptedData.Length > 0)
                {
                    //Log("Encoding the initialization vector to base-64...");
                    var ivBase64 = Convert.ToBase64String(iv);

                    //Log("Computing the hash of the XML Payload using SHA256...");
                    var sha256Hash = ComputeHashUsingSHA256(data);

                    //Log("Encoding the encrypted XML Payload to base-64");
                    var encryptedDataBase64 = Convert.ToBase64String(encryptedData);

                    //Log("Serializing encrypted XML Payload...");
                    var payload = new Payload
                    {
                        docMimeType = mimeType,
                        hash = sha256Hash,
                        key1 = "",
                        key2 = "",
                        iv = ivBase64,
                        doc = encryptedDataBase64
                    };

                    encryptedContent = Serialize(payload);
                }

                //Log("Encryption process finished.");
                return encryptedContent;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Cryptography exception occurred! {ex.Message}");
            }
        }
        //public string DecryptXmlPayloadData(string encryptedContent, string cipherKey)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(encryptedContent))
        //            throw new ArgumentException("Encrypted content cannot be null or empty string!");

        //        var payload = JsonConvert.DeserializeObject<Payload>(encryptedContent);
        //        if (payload == null)
        //            throw new ArgumentException("Invalid payload format");

        //        var data = Convert.FromBase64String(payload.Doc);
        //        var password = GeneratePassword(cipherKey);
        //        var iv = Convert.FromBase64String(payload.IV);

        //        return DecryptUsingAES(data, password, iv);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Cryptography exception occurred during decryption");
        //        throw new ApplicationException($"Cryptography exception occurred! {ex.Message}");
        //    }
        //}

        //public string DecryptXmlPayloadData(string encryptedContent, string cipherKey)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(encryptedContent)) throw new Exception("XML payload cannot be null or empty string!");

        //        //Log("Decryption process started.");
        //        var xmlPayloadData = "";

        //        //Log("Parsing encrypted content...");
        //        var payload = Deserialize<Payload>(encryptedContent);

        //        //Log("Parsing XML Payload from base-64...");
        //        var data = Convert.FromBase64String(payload.doc);

        //        var password = GeneratePassword(cipherKey);

        //        //Log("Parsing initialization vector from base-64...");
        //        var iv = Convert.FromBase64String(payload.iv);

        //        //Log("Decrypting XML Payload using 'AES-256-CBC'...");
        //        xmlPayloadData = DecryptUsingAES(data, password, iv);

        //        //Log("Decryption process finished.");
        //        return xmlPayloadData;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ApplicationException($"Cryptography exception occurred! {ex.Message}");
        //    }
        //}

        public string DecryptXmlPayloadData(string encryptedContent, string cipherKey)
        {
            try
            {
                if (string.IsNullOrEmpty(encryptedContent)) throw new Exception("XML payload cannot be null or empty string!");

                //Log("Decryption process started.");
                var xmlPayloadData = "";

                //Log("Parsing encrypted content...");
                var payload = Deserialize<Payload>(encryptedContent);

                //Log("Parsing XML Payload from base-64...");
                var data = Convert.FromBase64String(payload.doc);

                var password = GeneratePassword(cipherKey);

                //Log("Parsing initialization vector from base-64...");
                var iv = Convert.FromBase64String(payload.iv);

                //Log("Decrypting XML Payload using 'AES-256-CBC'...");
                xmlPayloadData = DecryptUsingAES(data, password, iv);

                //Log("Decryption process finished.");
                return xmlPayloadData;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Cryptography exception occurred! {ex.Message}");
            }
        }

        public string DecryptRestPayloadData(string encryptedContent, string cipherKey)
        {
            try
            {
                if (string.IsNullOrEmpty(encryptedContent))
                    throw new ArgumentException("Encrypted content cannot be null or empty string!");

                var payload = JsonConvert.DeserializeObject<dynamic>(encryptedContent);
                if (payload == null)
                    throw new ArgumentException("Invalid payload format");

                var doc = payload.result.doc.ToString();
                var iv = payload.result.iv.ToString();

                var decodedDoc = Convert.FromBase64String(doc);
                var ivBytes = Convert.FromBase64String(iv);
                var password = GeneratePassword(cipherKey);

                return DecryptUsingAES(decodedDoc, password, ivBytes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cryptography exception occurred during REST payload decryption");
                throw new ApplicationException($"Cryptography exception occurred! {ex.Message}");
            }
        }

        private byte[] GeneratePassword(string cipherKey)
        {
            var keyHashBytes = GetSHA256HashAsBytes(Encoding.UTF8.GetBytes(cipherKey));
            var paddedKey = new byte[PASSWORD_LEN];
            Buffer.BlockCopy(keyHashBytes, 0, paddedKey, 0, Math.Min(keyHashBytes.Length, PASSWORD_LEN));
            return paddedKey;
        }

        private byte[] EncryptUsingAES(byte[] data, byte[] password, byte[] iv)
        {
            using (var memoryStream = new MemoryStream())
            {
                var symmetricKey = new AesManaged
                {
                    Padding = PaddingMode.Zeros,
                    KeySize = 256,
                    BlockSize = 128,
                    Mode = CipherMode.CBC,
                    Key = password,
                    IV = iv
                };

                var encryptor = symmetricKey.CreateEncryptor(password, iv);

                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(data, 0, data.Length);
                }

                return memoryStream.ToArray();
            }
        }
        //private byte[] EncryptUsingAES(byte[] data, byte[] password, byte[] iv)
        //{
        //    using var aes = Aes.Create();
        //    aes.Padding = PaddingMode.Zeros;
        //    aes.KeySize = 256;
        //    aes.BlockSize = 128;
        //    aes.Mode = CipherMode.CBC;
        //    aes.Key = password;
        //    aes.IV = iv;

        //    using var memoryStream = new MemoryStream();
        //    using var encryptor = aes.CreateEncryptor(password, iv);
        //    using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

        //    cryptoStream.Write(data, 0, data.Length);
        //    cryptoStream.FlushFinalBlock();

        //    return memoryStream.ToArray();
        //}


        private string DecryptUsingAES(byte[] data, byte[] password, byte[] iv)
        {
            using (var memoryStream = new MemoryStream(data))
            {
                var symmetricKey = new AesManaged
                {
                    Padding = PaddingMode.Zeros,
                    KeySize = 256,
                    BlockSize = 128,
                    Mode = CipherMode.CBC,
                    Key = password,
                    IV = iv
                };

                var decryptor = symmetricKey.CreateDecryptor(password, iv);
                var output = new byte[data.Length];

                using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    var readBytes = cryptoStream.Read(output, 0, output.Length);
                    return Encoding.UTF8.GetString(output, 0, readBytes);
                }
            }
        }
        //private string DecryptUsingAES(byte[] data, byte[] password, byte[] iv)
        //{
        //    using var aes = Aes.Create();
        //    aes.Padding = PaddingMode.Zeros;
        //    aes.KeySize = 256;
        //    aes.BlockSize = 128;
        //    aes.Mode = CipherMode.CBC;
        //    aes.Key = password;
        //    aes.IV = iv;

        //    using var memoryStream = new MemoryStream(data);
        //    using var decryptor = aes.CreateDecryptor(password, iv);
        //    using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

        //    var output = new byte[data.Length];
        //    var readBytes = cryptoStream.Read(output, 0, output.Length);
        //    return Encoding.UTF8.GetString(output, 0, readBytes).TrimEnd('\0');
        //}

        private string ComputeHashUsingSHA256(byte[] data)
        {
            var crypt = SHA256.Create();

            //Log("Data to compute hash for:");
            //Log(ToHexString(data));

            var hash = crypt.ComputeHash(data);
            var hex = new StringBuilder();

            foreach (byte byt in hash)
                hex.Append(byt.ToString("x2"));

            return hex.ToString();
        }
       
        
        //private string ComputeHashUsingSHA256(byte[] data)
        //{
        //    using var sha256 = SHA256.Create();
        //    var hash = sha256.ComputeHash(data);
        //    var hex = new StringBuilder();

        //    foreach (byte b in hash)
        //        hex.Append(b.ToString("x2"));

        //    return hex.ToString();
        //}

        private byte[] GetSHA256HashAsBytes(byte[] data)
        {
            using var sha256 = SHA256.Create();
            return sha256.ComputeHash(data);
        }

        private byte[] GetRandomBytes(int count)
        {
            Random random = new Random();
            byte[] data = new byte[count];
            random.NextBytes(data);

            return data;
        }

        private string Serialize(object data)
        {
            return JsonConvert.SerializeObject(data);
        }

        private T Deserialize<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }
    }
}