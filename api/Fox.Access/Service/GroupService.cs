using System;
using DB.Fox;
using Fox.Access.Model;
using Fox.Access.Repository;

namespace Fox.Access.Service;

public class GroupService : IService
{
	private StampService StampService { get; set; }
	private GroupRepository GroupRepository { get; set; }

	public GroupService(GroupRepository groupRepository, StampService stampService)
	{
		GroupRepository = groupRepository;
		StampService = stampService;
	}

	public IEnumerable<Group> GetAllGroups()
	{
		return GroupRepository.GetAllGroups();
	}

	public Group? GetGroup(Guid id)
	{
		return GroupRepository.GetGroup(id);
	}

	public Group CreateGroup(Group group)
	{
		CheckGroupFields(group);
		return GroupRepository.CreateGroup(group);
	}

	public void UpdateGroup(Group group)
	{
        CheckGroupFields(group);
		GroupRepository.UpdateGroup(group);
	}

	public void DeleteGroup(Guid id)
	{
		GroupRepository.DeleteGroup(id);
	}

    public void AddUserToGroup(Guid groupId, Guid[] userIds)
	{
		int stampId = StampService.CreateStamp();
		GroupRepository.AddUserToGroup(groupId, userIds, stampId);
	}

    public void DelUserFromGroup(Guid groupId, Guid[] userIds)
    {
		GroupRepository.DelUserFromGroup(groupId, userIds);
    }

	public IEnumerable<User> GetUsersFromGroup(Guid id)
	{
		return GroupRepository.GetUsersFromGroup(id);
	}

    private void CheckGroupFields(Group group)
    {
        if (string.IsNullOrWhiteSpace(group.Name))
            throw new ArgumentNullException(nameof(Group.Name));
    }
}

