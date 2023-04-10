using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fox.Dox.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Fox.Dox.Model;
using Microsoft.AspNetCore.Authorization;

namespace Web.Fox.Pages.Menu;

[Authorize(Policy = "DOCUMENT_READ")]
public class DownloadDocumentModel : PageModel, INavBarItem
{
    public string MenuDescription => "Download";
    public string PagePath => "/Menu/DownloadDocument";
    public string MenuCategory => "DOCUMENT";

    public string Msg { get; set; } = string.Empty;
    public IEnumerable<DocumentInformation> Documents { get; set; } = Array.Empty<DocumentInformation>();

    public IEnumerable<DocumentHolder> DocumentHolders { get; set; } = Array.Empty<DocumentHolder>();

    [BindProperty]
    public Guid? HoldersFor { get; set; }
    
    private DocumentRepository _docRepo;
    public DownloadDocumentModel(DocumentRepository documentRepo)
    {
        _docRepo = documentRepo;
    }

    private void FillDocuments()
    {
        Documents = _docRepo.GetAllDocuments();
        foreach (var item in Documents)
        {
            DocumentInformation? docWithMetadata = _docRepo.GetDocumentInformation(item.Id);
            if(docWithMetadata != null)
                item.Metadata = docWithMetadata.Metadata;
        }
        Documents = Documents.OrderBy(d => d.Name);
    }

    public void OnGet()
    {
        FillDocuments();
    }

    public void OnPostDeleteDocument(Guid documentId)
    {
        try
        {
            HttpContext.HasPermission("DOCUMENT_DELETION");
            _docRepo.DeleteDocument(documentId);
        }
        catch (ArgumentException argEx)
        {
            Msg = argEx.Message;
        }
        FillDocuments();
    }

    public void OnPostHolders(Guid documentId)
    {
        try
        {
            HttpContext.HasPermission("DOCUMENT_PERMISSION_READ");
            DocumentHolders = _docRepo.GetPermissionByDocument(documentId);
            HoldersFor = documentId;
        }
        catch (Exception argEx)
        {
            Msg = argEx.Message;
        }
        FillDocuments();
    }

    public IActionResult OnPostDownload(Guid documentId, string documentName)
    {
        try
        {
            HttpContext.HasPermission("DOCUMENT_READ");
            var docFile = _docRepo.GetDocumentBinary(documentId);
            FillDocuments();
            return File(docFile, "application/octet-stream", documentName);
        }
        catch (ArgumentException argEx)
        {
            Msg = argEx.Message;
            FillDocuments();
            return Page();
        }
    }
}
