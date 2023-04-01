using System;
using System.Security.Cryptography;
using System.Text;

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
    }

    private byte[] _keyAes;
    private string EncryptString(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        using (var aesAlg = Aes.Create())
        using (var encryptor = aesAlg.CreateEncryptor(_keyAes, aesAlg.IV))
        using (var msEncrypt = new MemoryStream())
        {
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(text);
            }

            var iv = aesAlg.IV;
            var decryptedContent = msEncrypt.ToArray();
            var result = new byte[iv.Length + decryptedContent.Length];

            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
            Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

            return Convert.ToBase64String(result);
        }
    }

    private string DecryptString(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            return cipherText;
        var fullCipher = Convert.FromBase64String(cipherText);

        var iv = new byte[16];
        var cipher = new byte[16];

        Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, iv.Length);

        string result;
        using (var aesAlg = Aes.Create())
        using (var decryptor = aesAlg.CreateDecryptor(_keyAes, iv))
        using (var msDecrypt = new MemoryStream(cipher))
        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
        using (var srDecrypt = new StreamReader(csDecrypt))
        {
            result = srDecrypt.ReadToEnd();
            return result;
        }
    }

    private static byte[] GetRandomData(int bits)
    {
        var result = new byte[bits / 8];
        RandomNumberGenerator.Create().GetBytes(result);
        return result;
    }

}

