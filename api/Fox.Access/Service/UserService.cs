using System;
using System.Data;
using DB.Fox;
using Fox.Access.Model;
using Fox.Access.Hash;

namespace Fox.Access.Service;
public class UserService : IService
{
    private PermissionService PermissionService { get; set; }
	private DBConnection DB { get; set; }
    private const string PROC_GETPERMISSIONS = "fox_user_read_permission_v1";
    private const string PROC_GETSECRET = "fox_user_read_secret_v1";
    private const string PROC_GETUSER = "fox_user_read_v1";
    private const string PROC_GETUSER_BYID = "fox_user_read_byid_v1";
    private const string PROC_GETUSER_ALL = "fox_user_read_all_v1";
    private const string PROC_CREATEUSER = "fox_user_create_v1";
    private const string PROC_UPDATEUSER = "fox_user_update_v1";
    private const string PROC_UPDATEPASSWORD = "fox_user_update_password_v1";
    private const string PROC_DELETEUSER = "fox_user_delete_v1";
    private const string PROC_GETGROUPS = "fox_user_read_groups_v1";

    public UserService(DBConnection dbConnection, PermissionService permissionService)
	{
		DB = dbConnection;
        PermissionService = permissionService;
	}

    public IEnumerable<User> GetAllUsers()
    {
        return DB.Procedure<User>(PROC_GETUSER_ALL, new { });
    }

    public User? GetUser(string login)
	{
		var parameters = new { _userLogin = login };
		return DB.ProcedureFirstOrDefault<User?>(PROC_GETUSER, parameters);
	}

    public User? GetUser(Guid userId)
    {
        var parameters = new { _id = userId};
        return DB.ProcedureFirstOrDefault<User?>(PROC_GETUSER_BYID, parameters);
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
        user = CreateUser(user, password, false);
        return user;
    }

    public User CreateAdminUser(User user, string password)
    {
        user = CreateUser(user, password, true);
        PermissionService.AddPermission(user.Id, user.Id, "admin");
        return user;
    }

    public bool IsAdmin(Guid userId)
    {
        return GetSystemPermissions(userId).Contains("admin");
    }

    private User CreateUser(User user, string password, bool selfCreation)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentNullException(nameof(password));
        CheckUserFields(user);

        IHashMethod hashMethod = HashMethodFactory.Create();
        hashMethod.ComputeHash(password);
        UserSecret secret = hashMethod.GetSecret();

        var parameters = new
        {
            _email = user.Email,
            _login = user.Login,
            _password = secret.Password,
            _salt = secret.Salt,
            _hashMethod = (int)secret.HashMethod,
            _name = user.Name
        };

        user.Id = DB.ProcedureFirst<Guid>(PROC_CREATEUSER, parameters);

        if (selfCreation)
            PermissionService.AddPermission(user.Id, user.Id, "USER_SELF_MANAGEMENT");
        else
            PermissionService.AddPermission(user.Id, "USER_SELF_MANAGEMENT");
        return user;
    }

    public void UpdateUser(User user)
	{
		CheckUserFields(user);

		var parameters = new {
			_id = user.Id,
			_email = user.Email,
			_login = user.Login,
			_name = user.Name
		};

		DB.ProcedureExecute(PROC_UPDATEUSER, parameters);
	}

    public void UpdatePassword(Guid id, string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentNullException(nameof(password));

        IHashMethod hashMethod = HashMethodFactory.Create();
        hashMethod.ComputeHash(password);
        UserSecret secret = hashMethod.GetSecret();

        var parameters = new
        {
            _id = id,
            _password = secret.Password,
            _salt = secret.Salt,
            _hashMethod = (int)secret.HashMethod
        };

        DB.ProcedureExecute(PROC_UPDATEPASSWORD, parameters);
    }

    public void DeleteUser(Guid id)
	{
		var parameters = new { _id = id };
		DB.ProcedureExecute(PROC_DELETEUSER, parameters);
	}

	private void CheckUserFields(User user)
	{
        if (string.IsNullOrWhiteSpace(user.Login))
            throw new ArgumentNullException(nameof(User.Login));

        if (string.IsNullOrWhiteSpace(user.Name))
            user.Name = user.Login;
        if (string.IsNullOrWhiteSpace(user.Email))
            user.Email = string.Empty;
    }

    public IEnumerable<Group> GetUserGroups(Guid userId)
    {
        var parameters = new { _userId = userId };
        return DB.Procedure<Group>(PROC_GETGROUPS, parameters);
    }
}

