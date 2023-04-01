using System;
using DB.Fox;

namespace Fox.Access.Repository;
public class UserRepository : IRepository
{
	DBSettings _dbSettings;

    public UserRepository(DBSettings dbSettings)
	{
		_dbSettings = dbSettings;
	}

	public List<string> GetSystemPermissions(string userId)
	{
		//TODO: go to the actual database
		return new List<string>(new string[] { "teste1", "teste2" });
	}
}

