using System;
using System.IO;
namespace Fox.Dox.Storage;

public class FileStorage : IFileStorage
{
    private FileStorageSettings _settings;
	public FileStorage(FileStorageSettings settings)
	{
        _settings = settings;
	}

    public async Task DownloadAsync(Guid fileId, Stream streamToCopyFileTo)
    {
        if (!Directory.Exists(_settings.Path))
            throw new DirectoryNotFoundException();
        if (!File.Exists(GetFullPath(fileId)))
            throw new FileNotFoundException();

        using (var fileStored = new FileStream(GetFullPath(fileId), FileMode.Open, FileAccess.Read))
        {
            await fileStored.CopyToAsync(streamToCopyFileTo);
        }
        streamToCopyFileTo.Seek(0, SeekOrigin.Begin);
    }

    public async Task UploadAsync(Guid fileId, Stream fileToStore)
    {
        if (!Directory.Exists(_settings.Path))
            throw new DirectoryNotFoundException();

        fileToStore.Seek(0, SeekOrigin.Begin);
        using (var newStorage = File.Create(GetFullPath(fileId)))
        {
            await fileToStore.CopyToAsync(newStorage);
        }
    }

    public void DeleteFile(Guid fileId)
    {
        if (!File.Exists(GetFullPath(fileId)))
            throw new FileNotFoundException();
        File.Delete(GetFullPath(fileId));
    }

    private string GetFullPath(Guid fileId)
    {
        return Path.Combine(_settings.Path, fileId.ToString().ToUpper());
    }
}

