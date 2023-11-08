using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Models.Dto
{
  public class VillaNumberDTO
  {
    [Required]
    public int VillaNo { get; set; }
    [Required]
    public int VillaID { get; set; }
    public string SpecialDetails { get; set; }
    // ovo mi treba samo kada retreive all details
    public VillaDTO Villa { get; set; }
  }
}