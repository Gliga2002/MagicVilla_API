using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Models.Dto
{
  public class VillaCreateDTO
  {
    [Required]
    [MaxLength(30)]
    public string? Name { get; set; }
    public string Details { get; set; }
    public double Rate { get; set; }
    [Required]
    public int Occupancy { get; set; }
    public int Sqft { get; set; }
    public string ImageUrl { get; set; }
    public string Amenity { get; set; }


  }
}