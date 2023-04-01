namespace API.Login.Fox;


internal static class HashMethodFactory
{
    internal const HashMethod HASH_METHOD_DEFAULT = HashMethod.SHA512_S12_P12;
    internal const string HASH_METHOD_KEY = "HASH_METHOD";

    internal static IHashMethod Create()
    {
        var serverParameters = GetServerParameters(HASH_METHOD_DEFAULT.ToString());
        return Create(HASH_METHOD_DEFAULT, serverParameters, null);
    }

    internal static IHashMethod Create(string userId)
    {
        var userParameters = GetUserParameters(userId);
        HashMethod hashMethod = (HashMethod)userParameters[HASH_METHOD_KEY];
        var serverParameters = GetServerParameters(hashMethod.ToString());
        return Create(hashMethod, serverParameters, userParameters); 
    }

    private static IHashMethod Create(HashMethod hashMethod, Dictionary<string, object> serverParameters, Dictionary<string, object>? userParameters)
    {
        if(hashMethod == HashMethod.SHA512_S12_P12)
        {
            var sha = new HashSHA512S12P12(serverParameters, userParameters);
            return sha;
        }
        else
            throw new NotImplementedException($"The hash method '{hashMethod}' is not implemented");
    }

    internal static Dictionary<string, object> GetUserParameters(string userId)
    {
        //TODO throw excpetion if the user doesn't exist
        throw new NotImplementedException();
    }

    internal static Dictionary<string, object> GetServerParameters(string hashMethod)
    {
        //throw exception if the config file doesn't exist
        throw new NotImplementedException();
    }
}
