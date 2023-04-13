using System;
using System.ComponentModel.DataAnnotations;

namespace Fox.Dox.Model;

public class DocumentInformation
{
	public Guid Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
	public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
	public long FileSizeBytes { get; set; }
}

public enum DocumentPermission
{
	Download,
	Manage
}