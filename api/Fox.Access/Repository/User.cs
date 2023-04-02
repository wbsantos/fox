using System;
using System.Data;
using DB.Fox;

namespace Fox.Access.Repository;
public class UserRepository : IRepository
{
	private DBConnection DB { get; set; }
	private const string PROC_GETPERMISSIONS = "fox_user_read_permission_v1";

    public UserRepository(DBConnection dbSettings)
	{
		DB = dbSettings;
	}

	public IEnumerable<string> GetSystemPermissions(Guid userId)
	{
		var parameters = new { _userId = userId };
        return DB.Procedure<string>(PROC_GETPERMISSIONS, parameters);		
	}
}

