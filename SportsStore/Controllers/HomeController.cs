using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;

namespace SportsStore.Controllers
{
  public class HomeController(IStoreRepository repo) : Controller
  {
    public int PageSize = 4;

    public IActionResult Index(int productPage = 1)
      => View(repo.Products
          .OrderBy(p => p.ProductID)
          .Skip((productPage - 1) * PageSize)
          .Take(PageSize));
  }
}
