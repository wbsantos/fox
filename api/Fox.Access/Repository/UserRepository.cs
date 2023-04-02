using System;
using System.Data;
using DB.Fox;
using Fox.Access.Model;
using Fox.Access.Hash;

namespace Fox.Access.Repository;
public class UserRepository : IRepository
{
	private DBConnection DB { get; set; }
    private const string PROC_GETPERMISSIONS = "fox_user_read_permission_v1";
    private const string PROC_GETSECRET = "fox_user_read_secret_v1";

    public UserRepository(DBConnection dbSettings)
	{
		DB = dbSettings;
	}

	public bool ValidateUserPassword(Guid userId, string password)
	{
        var parameters = new { _userId = userId };
		UserSecret userSecret = DB.ProcedureFirst<UserSecret>(PROC_GETPERMISSIONS, parameters);

		IHashMethod hashMethod = HashMethodFactory.Create(userSecret);
		byte[] passwordInformed = hashMethod.ComputeHash(password);
		return hashMethod.IsSameHash(passwordInformed, userSecret.Password);
	}

	public IEnumerable<string> GetSystemPermissions(Guid userId)
	{
		var parameters = new { _userId = userId };
        return DB.Procedure<string>(PROC_GETPERMISSIONS, parameters);		
	}
}

