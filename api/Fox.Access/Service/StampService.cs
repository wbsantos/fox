using System;
using API.Fox.Settings;
using DB.Fox;
using Fox.Access.Model;
using Fox.Access.Repository;

namespace Fox.Access.Service;

public class StampService : IService
{
    private Model.LoggedUser User { get; set; }
    private StampRepository StampRepository { get; set; }
    private AppInfo AppInfo { get; set; }
    private const string PROC_CREATESTAMP = "fox_stamp_create_v1";
    
    public StampService(StampRepository stampRepository, AppInfo appInfo, Model.LoggedUser user)
    {
        StampRepository = stampRepository;
        AppInfo = appInfo;
        User = user;
    }

    public int CreateStamp(Guid userId)
    {
        return StampRepository.CreateStamp(userId, AppInfo.Version);
    }

    public int CreateStamp()
    {
        return StampRepository.CreateStamp(User.Id, AppInfo.Version);
    }
}

