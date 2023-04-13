using System;
using System.Xml.Linq;
using DB.Fox;
using Fox.Access.Model;
using Fox.Access.Service;
using Fox.Dox.Model;
using Fox.Dox.Repository;
using Fox.Dox.Storage;

namespace Fox.Dox.Service;

public class DocumentService : IService
{
	private LoggedUser LoggedUser { get; set; }
    private UserService UserService { get; set; }
    private StampService StampService { get; set; }
	private DocumentRepository DocumentRepository { get; set; }
    private IFileStorage FileStorage { get; set; }

    public DocumentService(DocumentRepository documentRepository,
                           LoggedUser loggedUser,
                           StampService stampService,
                           UserService userService,
                           IFileStorage fileStorage)
	{
        DocumentRepository = documentRepository;
		LoggedUser = loggedUser;
        StampService = stampService;
        UserService = userService;
        FileStorage = fileStorage;
	}

    public async Task<DocumentInformation> CreateDocumentAsync(DocumentInformation document, Stream fileToStore)
    {
        CheckDocumentFields(document, fileToStore);

        document.FileSizeBytes = fileToStore.Length;
        int stampId = StampService.CreateStamp();
        document = DocumentRepository.CreateDocument(document, stampId);
        await FileStorage.UploadAsync(document.Id, fileToStore);

        AddMetadata(document.Id, document.Metadata);
        AddPermission(document.Id, LoggedUser.Id, DocumentPermission.Manage);
        return new DocumentInformation()
        {
            Id = document.Id,
            Name = document.Name,
            FileSizeBytes = document.FileSizeBytes,
            Metadata = document.Metadata
        };
    }

    public bool DeleteDocument(Guid id)
    {
        if (!HasPermission(id, DocumentPermission.Manage))
            return false;
        DocumentRepository.DeleteDocument(id);
        FileStorage.DeleteFile(id);
        return true;
    }

    public void AddMetadata(Guid documentId, Dictionary<string, string> metadata)
    {
        if (metadata.Count == 0)
            return;
        DocumentRepository.AddMetadata(documentId, metadata);
    }

    public void DeleteMetadata(Guid documentId, string[] metadataKeys)
    {
        if (metadataKeys.Length == 0)
            return;
        DocumentRepository.DeleteMetadata(documentId, metadataKeys);
    }

    public bool HasPermission(Guid documentId, DocumentPermission permission)
    {
        IEnumerable<string> permissions = GetPermissionByHolder(documentId);
        string permissionToTest = permission.ToString().ToUpper();
        string permissionToManage = DocumentPermission.Manage.ToString().ToUpper();
        bool hasPermission = permissions.Any(p => p.ToUpper() == permissionToTest || p.ToUpper() == permissionToManage);

        hasPermission = hasPermission || UserService.IsAdmin(LoggedUser.Id);
        return hasPermission;
    }

    public IEnumerable<string> GetPermissionByHolder(Guid documentId)
    {
        return GetPermissionByHolder(documentId, LoggedUser.Id);
    }

    public IEnumerable<string> GetPermissionByHolder(Guid documentId, Guid holderId)
    {
        return DocumentRepository.GetPermissionByHolder(documentId, holderId);
    }

    public IEnumerable<DocumentHolder> GetPermissionByDocument(Guid documentId)
    {
        if (!HasPermission(documentId, DocumentPermission.Download))
            throw new UnauthorizedAccessException();
        return DocumentRepository.GetPermissionByDocument(documentId);
    }

    public DocumentInformation? GetDocumentInformation(Guid id)
    {
        if (!HasPermission(id, DocumentPermission.Download))
            throw new UnauthorizedAccessException();
        return DocumentRepository.GetDocumentInformation(id);
    }

    public async Task<Stream> GetDocumentBinaryAsync(Guid id)
    {
        if (!HasPermission(id, DocumentPermission.Download))
            throw new UnauthorizedAccessException();

        MemoryStream memory = new MemoryStream();
        await FileStorage.DownloadAsync(id, memory);
        return memory;
    }

    public void AddPermission(Guid documentId, Guid holderId, DocumentPermission permission)
    {
        if (!HasPermission(documentId, DocumentPermission.Manage))
            throw new UnauthorizedAccessException();
        int stampId = StampService.CreateStamp();
        DocumentRepository.AddPermission(documentId, holderId, permission, stampId);
    }

    public void DelPermission(Guid documentId, Guid holderId, DocumentPermission permission)
    {
        if (!HasPermission(documentId, DocumentPermission.Manage))
            throw new UnauthorizedAccessException();
        DocumentRepository.DelPermission(documentId, holderId, permission);
    }

    public IEnumerable<DocumentInformation> GetAllDocuments()
    {
        //TODO: the result should be paginated in the procedure (so this method should receive the offset and limit)
        return DocumentRepository.GetAllDocuments(LoggedUser.Id);
    }

    public void UpdateDocument(DocumentInformation document)
    {
        if (!HasPermission(document.Id, DocumentPermission.Manage))
            throw new UnauthorizedAccessException();
        DocumentRepository.UpdateDocument(document);
    }

    private void CheckDocumentFields(DocumentInformation document, Stream fileToCreate)
    {
        if (string.IsNullOrWhiteSpace(document.Name))
            throw new ArgumentNullException(nameof(DocumentInformation.Name));
        if (fileToCreate.Length == 0)
            throw new ArgumentNullException(nameof(fileToCreate));
    }
}

