using System;
using DB.Fox;

namespace Fox.Access.Repository;

public class PermissionRepository : IRepository
{
    private StampRepository StampRepo { get; set; }
    private DBConnection DB { get; set; }
    private const string PROC_ADDPERMISSIONS = "fox_system_addpermission_v1";
    
    public PermissionRepository(DBConnection dbConnection, StampRepository stampRepo)
    {
        DB = dbConnection;
        StampRepo = stampRepo;
    }

    public void AddPermission(Guid permissionHolderId, Guid permissionGiverId, string permission)
    {
        int stampId = StampRepo.CreateStamp(permissionGiverId);
        var parameters = new {
            _stampId = stampId,
            _holderId = permissionHolderId,
            _permission = permission
        };
        DB.ProcedureExecute(PROC_ADDPERMISSIONS, parameters);
    }
}

