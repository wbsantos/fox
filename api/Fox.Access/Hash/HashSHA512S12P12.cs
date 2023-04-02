using System.Security.Cryptography;
using System.Text;
using System.Linq;
using Fox.Access.Model;

namespace Fox.Access.Hash;

internal class HashSHA512S12P12 : IHashMethod
{
    public HashMethod HashMethod => HashMethod.SHA512_S12_P12;

    private const byte SALT_SIZE = 12;
    private byte[] Pepper { get; set; }
    private byte[] Salt { get; set; }
    private byte[] LastComputedHash { get; set; } = Array.Empty<byte>();

    internal HashSHA512S12P12(ServerSettings serverParameters,
                              UserSecret? userSecret)
    {
        Pepper = Encoding.UTF8.GetBytes(serverParameters.Pepper);
        if (userSecret != null)
            Salt = userSecret.Salt;
        else
            Salt = CreateSalt(SALT_SIZE);
    }

    public byte[] ComputeHash(string password)
    {
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var passwordWithSeason = Salt.Concat(passwordBytes).Concat(Pepper).ToArray();
        using(var sha = SHA512.Create())
        {
            return LastComputedHash = sha.ComputeHash(passwordWithSeason);
        }
    }
 
    public UserSecret GetSecret()
    {
        if (LastComputedHash.Length == 0)
            throw new Exception("No hash was computed before trying to build user secret");
        return new UserSecret()
        {
            Password = LastComputedHash,
            Salt = Salt,
            HashMethod = HashMethod
        };
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

    public bool IsSameHash(byte[] a, byte[] b)
    {
        return a.SequenceEqual(b);
    }
}
