using Fox.Access.Model;

namespace Fox.Access.Hash;

internal interface IHashMethod
{
    HashMethod HashMethod { get; }
    byte[] ComputeHash(string password);
    UserSecret GetSecret();
    bool IsSameHash(byte[] a, byte[] b);
}

internal enum HashMethod
{
    SHA512_S12_P12 = 1,
}
