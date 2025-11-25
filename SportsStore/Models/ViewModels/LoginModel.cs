using System.ComponentModel.DataAnnotations;

namespace SportsStore.Models.ViewModels
{
  public class LoginModel
  {
    [Required]
    public required string Name { get; set; }

    [Required]
    public required string Password { get; set; }

    public string ReturnUrl { get; set; } = "/";
  }
}
