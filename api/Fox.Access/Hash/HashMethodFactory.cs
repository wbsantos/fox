using Fox.Access.Model;
using System.Reflection;
using System.Text.Json;

namespace Fox.Access.Hash;


internal static class HashMethodFactory
{
    internal const HashMethod HASH_METHOD_DEFAULT = HashMethod.SHA512_S12_P12;

    internal static IHashMethod Create()
    {
        HashMethod hashMethod = HashMethod.SHA512_S12_P12; //Default hash method for new users
        var serverParameters = GetServerParameters(hashMethod.ToString());
        return Create(hashMethod, serverParameters, null);
    }

    internal static IHashMethod Create(UserSecret userSecret)
    {
        HashMethod hashMethod = userSecret.HashMethod;
        var serverParameters = GetServerParameters(hashMethod.ToString());
        return Create(hashMethod, serverParameters, userSecret); 
    }

    private static IHashMethod Create(HashMethod hashMethod,
                                      ServerSettings serverParameters,
                                      UserSecret? userSecret)
    {
        if(hashMethod == HashMethod.SHA512_S12_P12)
            return new HashSHA512S12P12(serverParameters, userSecret);
        else
            throw new NotImplementedException($"The hash method '{hashMethod}' is not implemented");
    }

    internal static ServerSettings GetServerParameters(string hashMethod)
    {
        string binFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? string.Empty;
        string jsonPath = Path.Combine(binFolder, "Hash", $"Hash{hashMethod}.json");
        string json = File.ReadAllText(jsonPath);
        return JsonSerializer.Deserialize<ServerSettings>(json) ?? new ServerSettings();
    }
}
