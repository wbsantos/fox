using System;
namespace Fox.Dox.Repository.DBCustomTypes;

public class MetadataDictionary : DB.Fox.IDBCustomType
{
	public string Key { get; set; } = string.Empty;
	public string Value { get; set; } = string.Empty;
}

