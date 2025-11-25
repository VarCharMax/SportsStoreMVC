using Moq;
using SportsStore.Models;
using SportsStore.Pages;

namespace SportsStore.Tests
{
  public class CartPageTests
  {
    [Fact]
    public void Can_Load_Cart()
    {
      // Arrange
      Product p1 = new() { ProductID = 1, Name = "P1" };
      Product p2 = new() { ProductID = 2, Name = "P2" };
      Mock<IStoreRepository> mockRepo = new();

      mockRepo.Setup(m => m.Products).Returns((new Product[] { p1, p2 }).AsQueryable());

      // - create a cart 
      Cart testCart = new();
      testCart.AddItem(p1, 2);
      testCart.AddItem(p2, 1);

      // Action
      CartModel cartModel = new(mockRepo.Object, testCart);
      cartModel.OnGet("myUrl");

      //Assert
      Assert.Equal(2, cartModel.Cart?.Lines.Count);
      Assert.Equal("myUrl", cartModel.ReturnUrl);
    }

    [Fact]
    public void Can_Update_Cart()
    {
      // Arrange
      // - create a mock repository
      Mock<IStoreRepository> mockRepo = new();

      mockRepo.Setup(m => m.Products).Returns((new Product[] { new() { ProductID = 1, Name = "P1" } }).AsQueryable());

      Cart testCart = new();

      // Action
      CartModel cartModel = new(mockRepo.Object, testCart);
      
      cartModel.OnPost(1, "myUrl");

      //Assert
      Assert.Single(testCart.Lines);
      Assert.Equal("P1", testCart.Lines.First().Product.Name);
      Assert.Equal(1, testCart.Lines.First().Quantity);
    }
  }
}

