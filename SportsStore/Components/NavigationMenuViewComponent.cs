using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;

namespace SportsStore.Components
{
  public class NavigationMenuViewComponent(IStoreRepository repo) : ViewComponent
  {
    public IViewComponentResult Invoke()
    {

      return View(repo.Products
        .Select(x => x.Category)
        .Distinct()
        .OrderBy(x => x));
    }
  }
}
