using System;
using API.Fox.Settings;
using DB.Fox;

namespace Fox.Access.Service;

public class StampService : IService
{
    private Model.LoggedUser User { get; set; }
    private DBConnection DB { get; set; }
    private AppInfo AppInfo { get; set; }
    private const string PROC_CREATESTAMP = "fox_stamp_create_v1";
    
    public StampService(DBConnection dbConnection, AppInfo appInfo, Model.LoggedUser user)
    {
        DB = dbConnection;
        AppInfo = appInfo;
        User = user;
    }

    public int CreateStamp(Guid userId)
    {
        var parameters = new { _userId = userId, _systemVersion = AppInfo.Version };
        return DB.ProcedureFirst<int>(PROC_CREATESTAMP, parameters);
    }

    public int CreateStamp()
    {
        return CreateStamp(User.Id);
    }
}

