using System;
using System.Data;
using DB.Fox;
using Fox.Access.Model;
using Fox.Access.Hash;
using Fox.Access.Repository;

namespace Fox.Access.Service;
public class UserService : IService
{
    private PermissionService PermissionService { get; set; }
    private UserRepository UserRepository { get; set; }

    public UserService(UserRepository userRepo, PermissionService permissionService)
	{
        PermissionService = permissionService;
        UserRepository = userRepo;

    }

    public IEnumerable<User> GetAllUsers()
    {
        return UserRepository.GetAllUsers();
    }

    public User? GetUser(string login)
    {
        return UserRepository.GetUser(login);
    }

    public User? GetUser(Guid userId)
    {
        return UserRepository.GetUser(userId);
    }

    public IEnumerable<string> GetSystemPermissions(Guid userId)
    {
        return UserRepository.GetSystemPermissions(userId);
    }

    public bool ValidateUserPassword(string login, string password)
	{
        UserSecret? userSecret = UserRepository.GetUserSecret(login, password);
        if (userSecret == null)
			return false;
        IHashMethod hashMethod = HashMethodFactory.Create(userSecret);
		byte[] passwordInformed = hashMethod.ComputeHash(password);
		return hashMethod.IsSameHash(passwordInformed, userSecret.Password);
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
        user = UserRepository.CreateUser(user, secret);

        if (selfCreation)
            PermissionService.AddPermission(user.Id, user.Id, "USER_SELF_MANAGEMENT");
        else
            PermissionService.AddPermission(user.Id, "USER_SELF_MANAGEMENT");
        return user;
    }

    public void UpdateUser(User user)
	{
		CheckUserFields(user);
        UserRepository.UpdateUser(user);
	}

    public void UpdatePassword(Guid id, string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentNullException(nameof(password));

        IHashMethod hashMethod = HashMethodFactory.Create();
        hashMethod.ComputeHash(password);
        UserSecret secret = hashMethod.GetSecret();

        UserRepository.UpdatePassword(id, secret);
    }

    public void DeleteUser(Guid id)
	{
        UserRepository.DeleteUser(id);
	}

    public IEnumerable<Group> GetUserGroups(Guid userId)
    {
        return UserRepository.GetUserGroups(userId);
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
}

