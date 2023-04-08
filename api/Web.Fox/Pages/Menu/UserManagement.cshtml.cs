using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Fox.Pages.Menu;

public class UserManagementModel : PageModel, INavBarItem
{
    public string MenuDescription => "User Management";
    public string PagePath => "/Menu/UserManagement";

    public void OnGet()
    {
    }
}
