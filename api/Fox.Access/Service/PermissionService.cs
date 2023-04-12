using System;
using System.Security;
using DB.Fox;
using Fox.Access.Model;
using Fox.Access.Repository;

namespace Fox.Access.Service;

public class PermissionService : IService
{
    private StampService StampService { get; set; }
    private PermissionRepository PermissionRepository { get; set; }
    public PermissionService(PermissionRepository permissionRepository, StampService stampService)
    {
        PermissionRepository = permissionRepository;
        StampService = stampService;
    }

    public void AddPermission(Guid permissionHolderId, Guid permissionGiverId, string permission)
    {
        if (string.IsNullOrWhiteSpace(permission))
            throw new ArgumentNullException(nameof(permission));
        int stampId = StampService.CreateStamp(permissionGiverId);
        PermissionRepository.AddPermission(stampId, permissionHolderId, permission);
    }

    public void AddPermission(Guid permissionHolderId, string permission)
    {
        if (string.IsNullOrWhiteSpace(permission))
            throw new ArgumentNullException(nameof(permission));
        int stampId = StampService.CreateStamp();
        PermissionRepository.AddPermission(stampId, permissionHolderId, permission);
    }

    public void DeletePermission(Guid permissionHolderId, string permission)
    {
        PermissionRepository.DeletePermission(permissionHolderId, permission);
    }

    public IEnumerable<string> GetPermissions(Guid holderId)
    {
        return PermissionRepository.GetPermissions(holderId);
    }
}

