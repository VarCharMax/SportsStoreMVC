using Microsoft.EntityFrameworkCore;

namespace SportsStore.Models
{
  public class EFOrderRepository(StoreDbContext ctx) : IOrderRepository
  {
    public IQueryable<Order> Orders => ctx.Orders
                            .Include(o => o.Lines)
                            .ThenInclude(l => l.Product);

    public void SaveOrder(Order order)
    {
      ctx.AttachRange(order.Lines.Select(l => l.Product));

      if (order.OrderID == 0)
      {
        ctx.Orders.Add(order);
      }
      ctx.SaveChanges();
    }
  }
}
