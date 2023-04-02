using System;
using Fox.Access.Hash;

namespace Fox.Access.Model;

internal class UserSecret
{
	public Guid Id { get; set; }
    public byte[] Password { get; set; } = Array.Empty<byte>();
    public byte[] Salt { get; set; } = Array.Empty<byte>();
	public HashMethod HashMethod { get; set; }
}

