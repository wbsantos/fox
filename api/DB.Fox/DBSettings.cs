using System;
using System.Security.Cryptography;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace DB.Fox;

public class DBSettings
{
    //store connection string encrypted in memory, so to avoid memory sniffing
    private string _encryptConnectionString = string.Empty;
    public string ConnectionString
    {
        get
        {
            return DecryptString(_encryptConnectionString);
        }
        set
        {
            _encryptConnectionString = EncryptString(value);
        }
    }

    public DBSettings()
    {
        _keyAes = GetRandomData(128);
        _ivAes = GetRandomData(128);
    }

    private byte[] _keyAes;
    private byte[] _ivAes;
    private string EncryptString(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;
        using (Aes aesAlgorithm = Aes.Create())
        {
            ICryptoTransform encryptor = aesAlgorithm.CreateEncryptor(_keyAes, _ivAes);
            byte[] encryptedData;
            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                using (StreamWriter sw = new StreamWriter(cs))
                {
                    sw.Write(text);
                }
                encryptedData = ms.ToArray();
            }
            
            return Convert.ToBase64String(encryptedData);
        }
    }

    private string DecryptString(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            return cipherText;
        using (Aes aesAlgorithm = Aes.Create())
        {
            ICryptoTransform decryptor = aesAlgorithm.CreateDecryptor(_keyAes, _ivAes);
            byte[] cipher = Convert.FromBase64String(cipherText);

            using (MemoryStream ms = new MemoryStream(cipher))
            using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (StreamReader sr = new StreamReader(cs))
                return sr.ReadToEnd();         
        }
    }

    private static byte[] GetRandomData(int bits)
    {
        var result = new byte[bits / 8];
        RandomNumberGenerator.Create().GetBytes(result);
        return result;
    }

}

