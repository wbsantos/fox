using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Fox.Access.Service;
using Fox.Dox.Service;
using Fox.Access.Model;
using Fox.Dox.Model;
using FoxUser = Fox.Access.Model.User;
using Microsoft.AspNetCore.Authorization;

namespace Web.Fox.Pages.Menu;

[Authorize(Policy = "DOCUMENT_CREATION")]
public class UploadDocumentModel : PageModel, INavBarItem
{
    public string MenuDescription => "Upload";
    public string PagePath => "/Menu/UploadDocument";
    public string MenuCategory => "DOCUMENT";
    public LoggedUser LoggedUser { get; set; }

    DocumentService _docService;
    public UploadDocumentModel(DocumentService docService, LoggedUser loggedUser)
    {
        _docService = docService;
        LoggedUser = loggedUser;
    }

    public bool SuccessOnPost { get; set; } = true;
    public string MsgInformation { get; set; } = string.Empty;
    [BindProperty]
    public IFormFile? FileBinary { get; set; }

    public void OnGet()
    {
        
    }

    public async Task<IActionResult> OnPostUploadDocumentAsync(List<MetadataModel> metadata)
    {
        try
        {
            if (FileBinary == null)
                throw new ArgumentException("No file was selected");

            var document = new DocumentInformation()
            {
                Name = FileBinary.FileName,
                Metadata = new Dictionary<string, string>()
            };
            foreach (var item in metadata.Where(m => !string.IsNullOrWhiteSpace(m.Key)))
                document.Metadata.Add(item.Key, item.Value);

            using (var ms = new MemoryStream())
            {
                FileBinary.CopyTo(ms);
                await _docService.CreateDocumentAsync(document, ms);
            }

            SuccessOnPost = true;
            MsgInformation = "Document uploaded!";
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

public class MetadataModel
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}