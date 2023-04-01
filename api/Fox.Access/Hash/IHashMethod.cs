namespace API.Login.Fox;

internal interface IHashMethod
{
    byte[] ComputeHash(string password);
    Dictionary<string, object> GetSecondaryParameters();
}

internal enum HashMethod
{
    SHA512_S12_P12 = 1,
}
