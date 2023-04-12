using System;
using API.Fox.Settings;
using DB.Fox;

namespace Fox.Access.Repository;

public class StampRepository : IRepository
{
    private DBConnection DB { get; set; }
    private const string PROC_CREATESTAMP = "fox_stamp_create_v1";
    
    public StampRepository(DBConnection dbConnection)
    {
        DB = dbConnection;
    }

    public int CreateStamp(Guid userId, string systemVersion)
    {
        var parameters = new { _userId = userId, _systemVersion = systemVersion };
        return DB.ProcedureFirst<int>(PROC_CREATESTAMP, parameters);
    }
}

