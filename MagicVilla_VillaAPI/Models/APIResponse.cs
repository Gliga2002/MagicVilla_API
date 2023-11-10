using System.Net;

namespace MagicVilla_VillaAPI.Models
{
  public class APIResponse
  {

    // public APIResponse() {
    //   ErrorMessages = new List<string>();
    // }
    public HttpStatusCode StatusCode { get; set; }
    public bool IsSuccess { get; set; } = true;

    // ovo je isto kao ovo gore, smao sto je ovo properties init a ono gore constructor init
    public List<string> ErrorMessages { get; set; } = new List<string>();

    public object Result { get; set; }
  }
}