using System;
using DB.Fox;
using Fox.Access.Model;

namespace Fox.Access.Repository;

public class GroupRepository : IRepository
{
	private StampRepository StampRepo { get; set; }
	private DBConnection DB { get; set; }
    private const string PROC_GETGROUP = "fox_group_read_byid_v1";
    private const string PROC_GETALLGROUPS = "fox_group_read_all_v1";
	private const string PROC_CREATEGROUP = "fox_group_create_v1";
	private const string PROC_UPDATEGROUP = "fox_group_update_v1";
	private const string PROC_DELETEGROUP = "fox_group_delete_v1";
	private const string PROC_ADDTOGROUP = "fox_group_adduser_v1";
	private const string PROC_DELFROMGROUP = "fox_group_deluser_v1";
	private const string PROC_GETUSERS = "fox_group_read_users_v1";

	public GroupRepository(DBConnection dbConnection, StampRepository stampRepo)
	{
		DB = dbConnection;
		StampRepo = stampRepo;
	}

	public IEnumerable<Group> GetAllGroups()
	{
		return DB.Procedure<Group>(PROC_GETALLGROUPS, new { });
	}

	public Group? GetGroup(Guid id)
	{
		var parameters = new { _id = id };
		return DB.ProcedureFirstOrDefault<Group>(PROC_GETGROUP, parameters);
	}

	public Group CreateGroup(Group group)
	{
		CheckGroupFields(group);
		var parameters = new { _name = group.Name };
		group.Id = DB.ProcedureFirst<Guid>(PROC_CREATEGROUP, parameters);
		return group;
	}

	public void UpdateGroup(Group group)
	{
        CheckGroupFields(group);
        var parameters = new { _id = group.Id, _name = group.Name };
		DB.ProcedureExecute(PROC_UPDATEGROUP, parameters);
	}

	public void DeleteGroup(Guid id)
	{
		var parameters = new { _id = id };
		DB.ProcedureExecute(PROC_DELETEGROUP, parameters);
	}

    private void CheckGroupFields(Group group)
    {
		if (string.IsNullOrWhiteSpace(group.Name))
			throw new ArgumentNullException(nameof(Group.Name));
    }

	public void AddUserToGroup(Guid groupId, Guid[] userIds)
	{
		int stampId = StampRepo.CreateStamp();
		var parameters = new {
			_stampId = stampId,
			_groupId = groupId,
			_userIdsToAdd = userIds
		};
		DB.ProcedureExecute(PROC_ADDTOGROUP, parameters);
	}

    public void DelUserFromGroup(Guid groupId, Guid[] userIds)
    {
        var parameters = new
        {
            _groupId = groupId,
            _userIds = userIds
        };
        DB.ProcedureExecute(PROC_DELFROMGROUP, parameters);
    }

	public IEnumerable<User> GetUsersFromGroup(Guid id)
	{
		var parameters = new { _groupId = id };
		return DB.Procedure<User>(PROC_GETUSERS, parameters);
	}
}

