using System;
using System.Security;
using DB.Fox;
using Fox.Access.Model;

namespace Fox.Access.Repository;

public class PermissionRepository : IRepository
{
    private StampRepository StampRepo { get; set; }
    private DBConnection DB { get; set; }
    private const string PROC_ADDPERMISSION = "fox_system_addpermission_v1";
    private const string PROC_DELPERMISSION = "fox_system_delpermission_v1";
    private const string PROC_GETPERMISSIONS = "fox_system_read_permission_v1";

    public PermissionRepository(DBConnection dbConnection, StampRepository stampRepo)
    {
        DB = dbConnection;
        StampRepo = stampRepo;
    }

    public void AddPermission(Guid permissionHolderId, Guid permissionGiverId, string permission)
    {
        int stampId = StampRepo.CreateStamp(permissionGiverId);
        AddPermission(stampId, permissionHolderId, permission);
    }

    public void AddPermission(Guid permissionHolderId, string permission)
    {
        if (string.IsNullOrWhiteSpace(permission))
            throw new ArgumentNullException(nameof(permission));
        int stampId = StampRepo.CreateStamp();
        AddPermission(stampId, permissionHolderId, permission);
    }

    private void AddPermission(int stampId, Guid permissionHolderId, string permission)
    {
        var parameters = new
        {
            _stampId = stampId,
            _holderId = permissionHolderId,
            _permission = permission
        };
        DB.ProcedureExecute(PROC_ADDPERMISSION, parameters);
    }

    public void DeletePermission(Guid permissionHolderId, string permission)
    {
        var parameters = new { _holderId = permissionHolderId, _permission = permission };
        DB.ProcedureExecute(PROC_DELPERMISSION, parameters);
    }

    public IEnumerable<string> GetPermissions(Guid holderId)
    {
        var parameters = new { _holderId = holderId};
        return DB.Procedure<string>(PROC_GETPERMISSIONS, parameters);
    }
}

