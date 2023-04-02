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
    private const string PROC_GETUSER = "fox_user_read_v1";
	private const string PROC_CREATEUSER = "fox_user_create_v1";

    public UserRepository(DBConnection dbConnection)
	{
		DB = dbConnection;
	}

	public User? GetUser(string login)
	{
		var parameters = new { _userLogin = login };
		return DB.ProcedureFirstOrDefault<User?>(PROC_GETUSER, parameters);
	}

	public bool ValidateUserPassword(string login, string password)
	{
        var parameters = new { _userLogin = login };
		UserSecret? userSecret = DB.ProcedureFirstOrDefault<UserSecret?>(PROC_GETSECRET, parameters);
		if (userSecret == null)
			return false;

		IHashMethod hashMethod = HashMethodFactory.Create(userSecret);
		byte[] passwordInformed = hashMethod.ComputeHash(password);
		return hashMethod.IsSameHash(passwordInformed, userSecret.Password);
	}

	public IEnumerable<string> GetSystemPermissions(Guid userId)
	{
		var parameters = new { _userId = userId };
        return DB.Procedure<string>(PROC_GETPERMISSIONS, parameters);		
	}

	public User CreateUser(User user, string password)
	{
		IHashMethod hashMethod = HashMethodFactory.Create();
		hashMethod.ComputeHash(password);
		UserSecret secret = hashMethod.GetSecret();

		var parameters = new {
			_email = user.Email,
			_login = user.Login,
			_password = secret.Password,
			_salt = secret.Salt,
			_hashMethod = (int)secret.HashMethod,
			_name = user.Name
		};

        user.Id = DB.ProcedureFirst<Guid>(PROC_CREATEUSER, parameters);
		return user;
	}
}

