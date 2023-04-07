using System;
namespace Fox.Dox.Model;

public class DocumentInformation
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
	public int FileSizeBytes { get; set; }
}

public class Document : DocumentInformation
{
    public byte[] FileBinary { get; set; } = Array.Empty<byte>();
}

public enum DocumentPermission
{
	Download,
	Manage
}