using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Fox.Access.Repository;
using Fox.Dox.Repository;
using Fox.Access.Model;
using Fox.Dox.Model;
using FoxUser = Fox.Access.Model.User;

namespace Web.Fox.Pages.Menu;

public class UpdateDocumentModel : PageModel
{
    public LoggedUser LoggedUser { get; set; }

    DocumentRepository _docRepo;
    public UpdateDocumentModel(DocumentRepository documentRepo, LoggedUser loggedUser)
    {
        _docRepo = documentRepo;
        LoggedUser = loggedUser;
        DocumentInformation = new DocumentInformation();
    }

    public bool SuccessOnPost { get; set; } = true;
    public string MsgInformation { get; set; } = string.Empty;

    [FromQuery(Name = "documentId")]
    public Guid DocumentIdQuery { get; set; }
    [FromQuery(Name = "documentName")]
    public string DocumentNameQuery { get; set; } = string.Empty;

    [BindProperty]
    public string DocumentName { get; set; } = string.Empty;
    [BindProperty]
    public Guid DocumentId { get; set; }

    public DocumentInformation DocumentInformation { get; set; }

    public void OnGet()
    {
        DocumentName = DocumentNameQuery;
        DocumentId = DocumentIdQuery;
        DocumentInformation = _docRepo.GetDocumentInformation(DocumentId) ?? new DocumentInformation();
    }

    public IActionResult OnPostUpdateDocument(List<MetadataModel> metadata)
    {
        try
        {
            var document = new DocumentInformation()
            {
                Id = DocumentId,
                Name = DocumentName,
                Metadata = new Dictionary<string, string>()
            };
            foreach (var item in metadata.Where(m => !string.IsNullOrWhiteSpace(m.Key)))
                document.Metadata.Add(item.Key, item.Value);

            _docRepo.UpdateDocument(document);
            var currentDocument = _docRepo.GetDocumentInformation(DocumentId) ?? new DocumentInformation();
            _docRepo.DeleteMetadata(DocumentId, currentDocument.Metadata.Keys.ToArray());
            _docRepo.AddMetadata(DocumentId, document.Metadata);

            SuccessOnPost = true;
            MsgInformation = "Document updated!";
            return RedirectToPage("DownloadDocument");
        }
        catch(Exception argEx)
        {
            MsgInformation = argEx.Message;
            SuccessOnPost = false;
            return Page();
        }
    }
}