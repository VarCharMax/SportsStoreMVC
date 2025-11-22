using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SportsStore.Models;

namespace SportsStore.Pages
{
  public class CartModel(IStoreRepository repo, Cart cartService) : PageModel
  {
    private readonly IStoreRepository repository = repo;

    public Cart Cart { get; set; } = cartService;

    public string ReturnUrl { get; set; } = "/";

    public void OnGet(string returnUrl)
    {
      ReturnUrl = returnUrl ?? "/";
    }

    public IActionResult OnPost(long productId, string returnUrl)
    {
      Product? product = repository.Products.FirstOrDefault(p => p.ProductID == productId);

      if (product != null)
      {
        Cart.AddItem(product, 1);
      }
      return RedirectToPage(new { returnUrl });
    }
  }
}
