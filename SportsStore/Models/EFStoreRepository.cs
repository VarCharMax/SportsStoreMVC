namespace SportsStore.Models
{
  public class EFStoreRepository(StoreDbContext ctx) : IStoreRepository
  {
    private readonly StoreDbContext context = ctx;

    public IQueryable<Product> Products => context.Products;
  }
}
