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
using System.Text.Json;

// treba da promenis namespace MagicVilla_VillaAPI.Controllers.v1
namespace MagicVilla_VillaAPI.Controllers
{

  //[Route("api/[controller]")]
  [Route("api/v{version:apiVersion}/VillaAPI")]
  [ApiController]
  [ApiVersion("1.0")]
  public class VillaAPIController : ControllerBase
  {

    private readonly IVillaRepository _dbVilla;
    private readonly IMapper _mapper;
    protected APIResponse _response;
    public VillaAPIController(IVillaRepository dbVilla, IMapper mapper)
    {
      _dbVilla = dbVilla;
      _mapper = mapper;
      this._response = new APIResponse();
    }


    [HttpGet]
    [ResponseCache(CacheProfileName = "Default30")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<APIResponse>> GetVillas([FromQuery(Name ="filterOccupancy")]int? occupancy,
    [FromQuery]  string? search, int pageSize = 0, int pageNumber = 1)
    {
      try
      {
        IEnumerable<Villa> villaList;

        if (occupancy > 0)
        {
          villaList = await _dbVilla.GetAllAsync(u => u.Occupancy == occupancy, pageSize:pageSize, pageNumber:pageNumber);
        }
        else
        {
          villaList = await _dbVilla.GetAllAsync(pageSize:pageSize, pageNumber:pageNumber);
        }

        if (!string.IsNullOrEmpty(search))
        {
            villaList = villaList.Where(u=>u.Name.ToLower().Contains(search));
        }
        Pagination pagination = new() { PageNumber = pageNumber, PageSize = pageSize };

        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));

        _response.Result = _mapper.Map<List<VillaDTO>>(villaList);
        _response.StatusCode = HttpStatusCode.OK;
        return Ok(_response);
      }
      catch (Exception ex)
      {
        _response.IsSuccess = false;
        _response.ErrorMessages = new List<string>() { ex.ToString() };
      }
      return _response;

    }


    [HttpGet("{id:int}", Name = "GetVilla")]
    // document what possible response type this endpoint will return
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    // ako ovo podesis svaki put ce da ode do database i da retrieve podatke 
    // [ResponseCache(Location = ResponseCacheLocation.None, NoStore =true)]

  
    public async Task<ActionResult<APIResponse>> GetVilla(int id)
    {
      try
      {
        if (id == 0)
        {
          _response.StatusCode = HttpStatusCode.BadRequest;
          return BadRequest(_response);
        }
        var villa = await _dbVilla.GetAsync(u => u.Id == id);
        if (villa == null)
        {
          _response.StatusCode = HttpStatusCode.NotFound;
          return NotFound(_response);
        }
        _response.Result = _mapper.Map<VillaDTO>(villa);
        _response.StatusCode = HttpStatusCode.OK;
        return Ok(_response);
      }
      catch (Exception ex)
      {
        _response.IsSuccess = false;
        _response.ErrorMessages = new List<string>() { ex.ToString() };
      }
      return _response;

    }


    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<APIResponse>> CreateVilla([FromBody] VillaCreateDTO createDTO)
    {
      // if (!ModelState.IsValid) {
      //   return BadRequest(ModelState);
      // }
      try
      {
        if (await _dbVilla.GetAsync(u => u.Name.ToLower() == createDTO.Name.ToLower()) != null)
        {
          ModelState.AddModelError("ErrorMessages", "Villa already Exists!");
          return BadRequest(ModelState);
        }
        if (createDTO == null)
        {
          return BadRequest(createDTO);
        }
        Villa villa = _mapper.Map<Villa>(createDTO);

        await _dbVilla.CreateAsync(villa);
        // mozes da dodas jos 2 modela i onda save changes
        // imas jedan database poziv umesto 3 separate
        // kad ga addujem ovaj model, vratice mi model sa id populate
        _response.Result = _mapper.Map<VillaDTO>(villa);
        _response.StatusCode = HttpStatusCode.Created;
        return CreatedAtRoute("GetVilla", new { id = villa.Id }, _response);
      }
      catch (Exception ex)
      {
        _response.IsSuccess = false;
        _response.ErrorMessages = new List<string>() { ex.ToString() };
      }
      return _response;
    }

    [HttpDelete("{id:int}", Name = "DeleteVilla")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
    {
      try
      {
        if (id == 0)
        {
          return BadRequest();
        }

        var villa = await _dbVilla.GetAsync(u => u.Id == id);
        if (villa == null)
        {
          return NotFound();
        }
        await _dbVilla.RemoveAsync(villa);
        _response.StatusCode = HttpStatusCode.NoContent;
        _response.IsSuccess = true;
        return Ok(_response);
      }
      catch (Exception ex)
      {
        _response.IsSuccess = false;
        _response.ErrorMessages = new List<string>() { ex.ToString() };
      }
      return _response;
    }


    [HttpPut("{id:int}", Name = "UpdateVilla")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody] VillaUpdateDTO updateDTO)
    {
      try
      {
        if (updateDTO == null || id != updateDTO.Id)
        {
          return BadRequest();
        }

        Villa model = _mapper.Map<Villa>(updateDTO);

        await _dbVilla.UpdateAsync(model);
        _response.StatusCode = HttpStatusCode.NoContent;
        _response.IsSuccess = true;
        return Ok(_response);
      }
      catch (Exception ex)
      {
        _response.IsSuccess = false;
        _response.ErrorMessages = new List<string>() { ex.ToString() };
      }
      return _response;

    }


    [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
    {
      if (patchDTO == null || id == 0)
      {
        return BadRequest();
      }
      var villa = await _dbVilla.GetAsync(u => u.Id == id, tracked: false);

      VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);

      if (villa == null)
      {
        return BadRequest();
      }
      patchDTO.ApplyTo(villaDTO, ModelState);

      Villa model = _mapper.Map<Villa>(villaDTO);

      await _dbVilla.UpdateAsync(model);

      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }
      return NoContent();
    }

  }
}