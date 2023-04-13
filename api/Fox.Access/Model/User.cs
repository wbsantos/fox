using System;
using System.ComponentModel.DataAnnotations;

namespace Fox.Access.Model;

public class User
{
	public Guid Id { get; set; }
	public string Email { get; set; } = string.Empty;
    [Required]
    public string Login { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

