using System;
namespace Web.Fox.Pages.Menu;

public interface INavBarItem
{
    public string MenuDescription { get; }
    public string PagePath { get; }
    public string MenuCategory { get; }
}

