using System;
using API.Fox.Settings;
using DB.Fox;

namespace Fox.Access.Repository;

public class StampRepository : IRepository
{
    private DBConnection DB { get; set; }
    private AppInfo AppInfo { get; set; }
    private const string PROC_CREATESTAMP = "fox_stamp_create_v1";
    
    public StampRepository(DBConnection dbConnection, AppInfo appInfo)
    {
        DB = dbConnection;
        AppInfo = appInfo;
    }

    public int CreateStamp(Guid userId)
    {
        var parameters = new { _userId = userId, _systemVersion = AppInfo.Version };
        return DB.ProcedureFirst<int>(PROC_CREATESTAMP, parameters);
    }
}

