using System;
using System.ComponentModel.DataAnnotations;

namespace Fox.Access.Model;

public class Group
{
	public Guid Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
}

