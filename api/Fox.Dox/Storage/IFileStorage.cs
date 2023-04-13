using System;
namespace Fox.Dox.Storage;

public interface IFileStorage
{
    public Task UploadAsync(Guid fileId, Stream fileToStore);
    public Task DownloadAsync(Guid fileId, Stream streamToCopyFileTo);
    public void DeleteFile(Guid fileId);
}

