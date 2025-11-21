using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Moq;
using SportsStore.Components;
using SportsStore.Controllers;
using SportsStore.Infrastructure;
using SportsStore.Models;
using SportsStore.Models.ViewModels;
using static System.Net.Mime.MediaTypeNames;

namespace SportsStore.Tests
{
  public class HomeControllerTests
  {
    [Fact]
    public void CanUseRepository()
    {
      var testData = (new Product[]
        {
          new() { ProductID = 1, Name = "P1", Price = 73.10M },
          new() { ProductID = 2, Name = "P2", Price = 120M },
          new() { ProductID = 3, Name = "P3", Price = 1100M },
        }).AsQueryable();

      var mock = new Mock<IStoreRepository>();
      mock.Setup(m => m.Products).Returns(testData);

      // Arrange
      var controller = new HomeController(mock.Object);

      // Act
      ProductsListViewModel result = controller.Index(null)?.ViewData.Model 
        as ProductsListViewModel ?? new();

      //Assert
      Product[] prodArray = result?.Products.ToArray() ?? [];

      Assert.Equal(3, prodArray.Length);
      Assert.Equal("P1", prodArray[0].Name);
      Assert.Equal("P2", prodArray[1].Name);
    }

    [Fact]
    public void Can_Paginate()
    {
      var testData = (new Product[] {
                new() {ProductID = 1, Name = "P1"},
                new() {ProductID = 2, Name = "P2"},
                new() {ProductID = 3, Name = "P3"},
                new() {ProductID = 4, Name = "P4"},
                new() {ProductID = 5, Name = "P5"}
            }).AsQueryable();

      // Arrange
      Mock<IStoreRepository> mock = new();
      mock.Setup(m => m.Products).Returns(testData);

      HomeController controller = new(mock.Object)
      {
        PageSize = 3
      };

      // Act
      ProductsListViewModel result = controller.Index(null, 2)?
        .ViewData.Model as ProductsListViewModel ?? new();

      // Assert
      Product[] prodArray = [.. result.Products];
      Assert.Equal(2, prodArray.Length);
      Assert.Equal("P4", prodArray[0].Name);
      Assert.Equal("P5", prodArray[1].Name);
    }

    [Fact]
    public void Can_Generate_Page_Links()
    {
      // Arrange
      var urlHelper = new Mock<IUrlHelper>();
      urlHelper.SetupSequence(x =>
                x.Action(It.IsAny<UrlActionContext>()))
          .Returns("Test/Page1")
          .Returns("Test/Page2")
          .Returns("Test/Page3");

      var urlHelperFactory = new Mock<IUrlHelperFactory>();
      urlHelperFactory.Setup(f =>
              f.GetUrlHelper(It.IsAny<ActionContext>()))
                  .Returns(urlHelper.Object);

      var viewContext = new Mock<ViewContext>();

      PageLinkTagHelper helper = new(urlHelperFactory.Object)
      {
        PageModel = new PagingInfo
        {
          CurrentPage = 2,
          TotalItems = 28,
          ItemsPerPage = 10
        },
        ViewContext = viewContext.Object,
        PageAction = "Test"
      };

      TagHelperContext ctx = new(
          [],
          new Dictionary<object, object>(),
          ""
      );

      var content = new Mock<TagHelperContent>();

      TagHelperOutput output = new(
        "div",
        [],
        (cache, encoder) => Task.FromResult(content.Object)
      );

      // Act
      helper.Process(ctx, output);

      // Assert
      Assert.Equal(@"<a href=""Test/Page1"">1</a>"
          + @"<a href=""Test/Page2"">2</a>"
          + @"<a href=""Test/Page3"">3</a>",
           output.Content.GetContent());
    }
    [Fact]
    public void Can_Send_Pagination_View_Model()
    {
      // Arrange
      Mock<IStoreRepository> mock = new();
      mock.Setup(m => m.Products).Returns((new Product[] {
        new() {ProductID = 1, Name = "P1"},
        new() {ProductID = 2, Name = "P2"},
        new() {ProductID = 3, Name = "P3"},
        new() {ProductID = 4, Name = "P4"},
        new() {ProductID = 5, Name = "P5"}
      }).AsQueryable());

      // Arrange
      HomeController controller = new(mock.Object) { PageSize = 3 };

      // Act
      ProductsListViewModel result =
          controller.Index(null, 2)?.ViewData.Model as ProductsListViewModel
              ?? new();

      // Assert
      PagingInfo pageInfo = result.PagingInfo;
      Assert.Equal(2, pageInfo.CurrentPage);
      Assert.Equal(3, pageInfo.ItemsPerPage);
      Assert.Equal(5, pageInfo.TotalItems);
      Assert.Equal(2, pageInfo.TotalPages);
    }

    [Fact]
    public void Can_Filter_Products()
    {
      // Arrange
      // - create the mock repository
      Mock<IStoreRepository> mock = new();
      mock.Setup(m => m.Products).Returns((new Product[] {
        new() {ProductID = 1, Name = "P1", Category = "Cat1"},
        new() {ProductID = 2, Name = "P2", Category = "Cat2"},
        new() {ProductID = 3, Name = "P3", Category = "Cat1"},
        new() {ProductID = 4, Name = "P4", Category = "Cat2"},
        new() {ProductID = 5, Name = "P5", Category = "Cat3"}
      }).AsQueryable());

      // Arrange - create a controller and make the page size 3 items
      HomeController controller = new(mock.Object)
      {
        PageSize = 3
      };

      // Action
      Product[] result = [.. (controller.Index("Cat2", 1)?.ViewData.Model
          as ProductsListViewModel ?? new()).Products];

      // Assert
      Assert.Equal(2, result.Length);
      Assert.True(result[0].Name == "P2" && result[0].Category == "Cat2");
      Assert.True(result[1].Name == "P4" && result[1].Category == "Cat2");
    }

    [Fact]
    public void Can_Select_Categories()
    {
      // Arrange
      Mock<IStoreRepository> mock = new();
      mock.Setup(m => m.Products).Returns((new Product[] {
                new() {ProductID = 1, Name = "P1",
                    Category = "Apples"},
                new() {ProductID = 2, Name = "P2",
                    Category = "Apples"},
                new() {ProductID = 3, Name = "P3",
                    Category = "Plums"},
                new() {ProductID = 4, Name = "P4",
                    Category = "Oranges"},
            }).AsQueryable());

      NavigationMenuViewComponent target = new(mock.Object);

      // Act = get the set of categories
      string[] results = [.. (IEnumerable<string>?)(target.Invoke()
         as ViewViewComponentResult)?.ViewData?.Model ?? []];

      // Assert
      Assert.True(Enumerable.SequenceEqual(["Apples", "Oranges", "Plums"], results));
    }
  }
}
