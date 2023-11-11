using System.Net;
using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// treba da promenis namespace MagicVilla_VillaAPI.Controllers.v2
namespace MagicVilla_VillaAPI.Controllers
{

  [Route("api/v{version:apiVersion}/VillaNumberAPI")]
  [ApiController]
  [ApiVersion("2.0")]
  public class VillaNumberAPIv2Controller : ControllerBase
  {

    private readonly IVillaNumberRepository _dbVillaNumber;
    private readonly IVillaRepository _dbVilla;
    private readonly IMapper _mapper;
    protected APIResponse _response;
    public VillaNumberAPIv2Controller(IVillaNumberRepository dbVillaNumber, IMapper mapper, IVillaRepository dbVilla)
    {
      _dbVillaNumber = dbVillaNumber;
      _mapper = mapper;
      this._response = new APIResponse();
      _dbVilla = dbVilla;
    }



      // [MapToApiVersion("2.0")]
      [HttpGet("GetString")]
      public IEnumerable<string> Get()
      {
          return new string[] { "value1", "value2" };
      }



  }
}