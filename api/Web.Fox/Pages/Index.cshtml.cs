﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Fox.Access.Model;

namespace Web.Fox.Pages;

[Authorize]
public class IndexModel : PageModel
{
    public LoggedUser LoggedUser { get; }

    public IndexModel(LoggedUser loggedUser)
    {
        LoggedUser = loggedUser;
    }

    public void OnGet()
    {
    }
}
