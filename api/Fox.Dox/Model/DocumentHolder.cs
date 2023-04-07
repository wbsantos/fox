using System;
namespace Fox.Dox.Model;

public class DocumentHolder
{
	public Guid HolderId { get; set; }
	public string HolderLogin { get; set; } = string.Empty;
	public string HolderName { get; set; } = string.Empty;
	public DocumentPermission Permission { get; set; }
	public string HolderType { get; set; } = string.Empty;
}

