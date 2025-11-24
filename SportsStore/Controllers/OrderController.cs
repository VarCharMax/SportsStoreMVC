using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;

namespace SportsStore.Controllers
{
  public class OrderController(IOrderRepository repo, Cart cartService) : Controller
  {
    public ViewResult Checkout() => View(new Order());

    [HttpPost]
    public IActionResult Checkout(Order order)
    {
      if (cartService.Lines.Count == 0)
      {
        ModelState.AddModelError("", "Sorry, your cart is empty!");
      }
      if (ModelState.IsValid)
      {
        order.Lines = [.. cartService.Lines];

        repo.SaveOrder(order);
        cartService.Clear();

        return RedirectToPage("/Completed", new { orderId = order.OrderID });
      }
      else
      {
        return View();
      }
    }
  }
}
