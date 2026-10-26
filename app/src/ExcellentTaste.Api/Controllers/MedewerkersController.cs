using ExcellentTaste.Core.Models;
using ExcellentTaste.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExcellentTaste.Api.Controllers;

[ApiController]
[Route("api/medewerkers")]
public class MedewerkersController : ControllerBase
{
    private readonly MedewerkerService _service;

    public MedewerkersController(MedewerkerService service) => _service = service;

    [HttpGet]
    public ActionResult<List<Medewerker>> GetAll() => Ok(_service.GetAll());

    [HttpGet("{id:int}")]
    public ActionResult<Medewerker> GetById(int id)
    {
        var medewerker = _service.GetById(id);
        return medewerker is null ? NotFound() : Ok(medewerker);
    }

    [HttpPost]
    public ActionResult<object> Add(Medewerker medewerker)
    {
        if (string.IsNullOrWhiteSpace(medewerker.Naam)) return BadRequest(new { bericht = "Vul een naam in." });
        var id = _service.Add(medewerker);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, Medewerker medewerker)
    {
        if (_service.GetById(id) is null) return NotFound();
        if (string.IsNullOrWhiteSpace(medewerker.Naam)) return BadRequest(new { bericht = "Vul een naam in." });
        _service.Update(id, medewerker);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        if (_service.GetById(id) is null) return NotFound();
        _service.Delete(id);
        return NoContent();
    }
}
