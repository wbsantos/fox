using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Fox.Pages.Menu;

public class ProfileModel : PageModel, INavBarItem
{
    public string MenuDescription => "Profile";
    public string PagePath => "/Menu/Profile";

    public void OnGet()
    {
    }
}
