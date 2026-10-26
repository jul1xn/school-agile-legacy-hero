using ExcellentTaste.Core.Models;
using ExcellentTaste.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExcellentTaste.Api.Controllers;

[ApiController]
[Route("api/diensten")]
public class DienstenController : ControllerBase
{
    private readonly DienstService _service;

    public DienstenController(DienstService service) => _service = service;

    [HttpGet]
    public ActionResult<List<Dienst>> GetByWeek([FromQuery] DateTime start, [FromQuery] DateTime einde)
    {
        if (einde <= start) return BadRequest(new { bericht = "De einddatum moet na de startdatum liggen." });
        return Ok(_service.GetByWeek(start, einde));
    }

    [HttpGet("{id:int}")]
    public ActionResult<Dienst> GetById(int id)
    {
        var dienst = _service.GetById(id);
        return dienst is null ? NotFound() : Ok(dienst);
    }

    [HttpPost]
    public ActionResult<object> Add(Dienst dienst)
    {
        var result = _service.Add(dienst);
        if (!result.Gelukt) return BadRequest(new { bericht = result.Bericht });
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, new { id = result.Id });
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, Dienst dienst)
    {
        if (_service.GetById(id) is null) return NotFound();
        var result = _service.Update(id, dienst);
        return result.Gelukt ? NoContent() : BadRequest(new { bericht = result.Bericht });
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        if (_service.GetById(id) is null) return NotFound();
        _service.Delete(id);
        return NoContent();
    }
}
