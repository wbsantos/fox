using System;
using System.Xml.Linq;
using DB.Fox;
using Fox.Access.Model;
using Fox.Access.Service;
using Fox.Dox.Model;

namespace Fox.Dox.Service;

public class DocumentService : IService
{
	private LoggedUser LoggedUser { get; set; }
    private UserService UserService { get; set; }
    private StampService StampService { get; set; }
	private DBConnection DB { get; set; }
	private const string PROC_CREATEDOC = "fox_document_create_v1";
    private const string PROC_DELETEDOC = "fox_document_delete_v1";
    private const string PROC_ADDMETADATA = "fox_document_addmetadata_v1";
    private const string PROC_DELMETADATA = "fox_document_delmetadata_v1";
    private const string PROC_READPERMISSION_BYHOLDER = "fox_document_read_permissions_byholder_v1";
    private const string PROC_READPERMISSION_BYDOCUMENT = "fox_document_read_permissions_bydoc_v1";
    private const string PROC_ADDPERMISSION = "fox_document_addpermission_v1";
    private const string PROC_DELPERMISSION = "fox_document_delpermission_v1";
    private const string PROC_READ_METADATA = "fox_document_read_metadata_v1";
    private const string PROC_READ_INFORMATION = "fox_document_read_information_v1";
    private const string PROC_READ_BINARY = "fox_document_read_binary_v1";
    private const string PROC_READ_ALL = "fox_document_read_information_all_v1";
    private const string PROC_UPDATE = "fox_document_update_v1";

    public DocumentService(DBConnection dbConnection, LoggedUser loggedUser, StampService stampService, UserService userService)
	{
		DB = dbConnection;
		LoggedUser = loggedUser;
        StampService = stampService;
        UserService = userService;
	}

    public Document CreateDocument(Document document)
    {
        CheckDocumentFields(document);

        int stampId = StampService.CreateStamp();

        var parameters = new
        {
            _stampId = stampId,
            _fileBinary = document.FileBinary,
            _fileName = document.Name,
            _sizeBytes = document.FileBinary.Length
        };

        document.Id = DB.ProcedureFirst<Guid>(PROC_CREATEDOC, parameters);
        AddMetadata(document.Id, document.Metadata);
        AddPermission(document.Id, LoggedUser.Id, DocumentPermission.Manage);
        return document;
    }

    public bool DeleteDocument(Guid id)
    {
        if (!HasPermission(id, DocumentPermission.Manage))
            return false;
        var parameters = new { _id = id };
        DB.ProcedureExecute(PROC_DELETEDOC, parameters);
        return true;
    }

    public void AddMetadata(Guid documentId, Dictionary<string, string> metadata)
    {
        if (metadata.Count == 0)
            return;
        var parameters = new {
            _documentId = documentId,
            _items = metadata.Select(m => new Fox.Dox.Repository.DBCustomTypes.MetadataDictionary() {
                                         Key = m.Key,
                                            Value = m.Value })
                             .ToArray()
        };
        DB.ProcedureExecute(PROC_ADDMETADATA, parameters);
    }

    public void DeleteMetadata(Guid documentId, string[] metadataKeys)
    {
        if (metadataKeys.Length == 0)
            return;
        var parameters = new
        {
            _documentId = documentId,
            _keys = metadataKeys
        };
        DB.ProcedureExecute(PROC_DELMETADATA, parameters);
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
        var parameters = new { _documentId = documentId, _holderId = holderId };
        return DB.Procedure<string>(PROC_READPERMISSION_BYHOLDER, parameters);
    }

    public IEnumerable<DocumentHolder> GetPermissionByDocument(Guid documentId)
    {
        if (!HasPermission(documentId, DocumentPermission.Download))
            throw new UnauthorizedAccessException();
        var parameters = new { _documentId = documentId };
        return DB.Procedure<DocumentHolder>(PROC_READPERMISSION_BYDOCUMENT, parameters);
    }

    public DocumentInformation? GetDocumentInformation(Guid id)
    {
        if (!HasPermission(id, DocumentPermission.Download))
            throw new UnauthorizedAccessException();
        var parameters = new { _documentId = id };
        var document = DB.ProcedureFirstOrDefault<DocumentInformation>(PROC_READ_INFORMATION, parameters);
        if (document == null)
            return null;

        document.Metadata = DB.Procedure(PROC_READ_METADATA, parameters)
                              .ToDictionary(row => (string)row.key, row => (string)row.value);
        return document;
    }

    public byte[] GetDocumentBinary(Guid id)
    {
        if (!HasPermission(id, DocumentPermission.Download))
            throw new UnauthorizedAccessException();
        var parameters = new { _id = id };
        return DB.ProcedureFirst<byte[]>(PROC_READ_BINARY, parameters);
    }

    public void AddPermission(Guid documentId, Guid holderId, DocumentPermission permission)
    {
        if (!HasPermission(documentId, DocumentPermission.Manage))
            throw new UnauthorizedAccessException();
        int stampId = StampService.CreateStamp();
        var parameters = new {
            _stampId = stampId,
            _documentId = documentId,
            _holderId = holderId,
            _permission = permission.ToString()
        };
        DB.ProcedureExecute(PROC_ADDPERMISSION, parameters);
    }

    public void DelPermission(Guid documentId, Guid holderId, DocumentPermission permission)
    {
        if (!HasPermission(documentId, DocumentPermission.Manage))
            throw new UnauthorizedAccessException();
        var parameters = new
        {
            _documentId = documentId,
            _holderId = holderId,
            _permission = permission.ToString()
        };
        DB.ProcedureExecute(PROC_DELPERMISSION, parameters);
    }

    public IEnumerable<DocumentInformation> GetAllDocuments()
    {
        //TODO: the result should be paginated in the procedure (so this method should receive the offset and limit)
        var parameters = new { _userId = LoggedUser.Id };
        return DB.Procedure<DocumentInformation>(PROC_READ_ALL, parameters);
    }

    public void UpdateDocument(DocumentInformation document)
    {
        if (!HasPermission(document.Id, DocumentPermission.Manage))
            throw new UnauthorizedAccessException();
        var parameters = new { _id = document.Id, _name = document.Name };
        DB.ProcedureExecute(PROC_UPDATE, parameters);
    }

    private void CheckDocumentFields(Document document)
    {
        if (string.IsNullOrWhiteSpace(document.Name))
            throw new ArgumentNullException(nameof(Document.Name));
        if (document.FileBinary.Length == 0)
            throw new ArgumentNullException(nameof(Document.FileBinary));
    }
}

