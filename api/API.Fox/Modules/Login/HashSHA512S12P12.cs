using System.Security.Cryptography;
using System.Text;

namespace API.Login.Fox;

internal class HashSHA512S12P12 : IHashMethod
{
    private const byte SALT_SIZE = 12;

    private byte[] Pepper { get; set; }
    private byte[] Salt { get; set; }

    internal HashSHA512S12P12(Dictionary<string, object> serverParameters, Dictionary<string, object>? userParameters)
    {
        Pepper = (byte[])serverParameters[nameof(Pepper)];
        if(userParameters?.ContainsKey(nameof(Salt)) ?? false)
            Salt = (byte[])userParameters[nameof(Salt)];
        else
            Salt = CreateSalt(SALT_SIZE);
    }

    public byte[] ComputeHash(string password)
    {
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var passwordWithSeason = Salt.Concat(passwordBytes).Concat(Pepper).ToArray();
        using(var sha = SHA512.Create())
        {
            return sha.ComputeHash(passwordWithSeason);
        }
    }
 
    public Dictionary<string, object> GetSecondaryParameters()
    {
        var dict = new Dictionary<string, object>();
        dict.Add(nameof(Salt), Salt);
        return dict;
    }

    internal byte[] CreateSalt(byte saltSize)
    {
        var salt = new byte[saltSize];
        using(var rgn = RandomNumberGenerator.Create())
        {
            rgn.GetBytes(salt);
        }
        return salt;
    }

}
