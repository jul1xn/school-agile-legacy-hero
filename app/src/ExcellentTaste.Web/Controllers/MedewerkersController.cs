using System.Net.Http.Json;
using ExcellentTaste.Core.Models;
using ExcellentTaste.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ExcellentTaste.Web.Controllers;

public class MedewerkersController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<MedewerkersController> _logger;

    public MedewerkersController(IHttpClientFactory httpClientFactory, ILogger<MedewerkersController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            return View(await Client().GetFromJsonAsync<List<Medewerker>>("api/medewerkers") ?? []);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Medewerkers konden niet via de API worden opgehaald.");
            ViewBag.ApiFout = "Medewerkers konden niet worden opgehaald.";
            return View(new List<Medewerker>());
        }
    }

    public IActionResult Create() => View(new MedewerkerFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MedewerkerFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var response = await Client().PostAsJsonAsync("api/medewerkers", ToMedewerker(model));
        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "De medewerker kon niet worden opgeslagen.");
            return View(model);
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var medewerker = await Client().GetFromJsonAsync<Medewerker>($"api/medewerkers/{id}");
        if (medewerker is null) return NotFound();
        return View(ToForm(medewerker));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, MedewerkerFormViewModel model)
    {
        model.MedewerkerId = id;
        if (!ModelState.IsValid) return View(model);
        var response = await Client().PutAsJsonAsync($"api/medewerkers/{id}", ToMedewerker(model));
        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "De medewerker kon niet worden opgeslagen.");
            return View(model);
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await Client().DeleteAsync($"api/medewerkers/{id}");
        if (!response.IsSuccessStatusCode) TempData["Fout"] = "Deze medewerker kan niet worden verwijderd. Verwijder eerst de ingeplande diensten.";
        return RedirectToAction(nameof(Index));
    }

    private HttpClient Client() => _httpClientFactory.CreateClient("ExcellentTasteApi");

    private static Medewerker ToMedewerker(MedewerkerFormViewModel model) => new()
    {
        MedewerkerId = model.MedewerkerId,
        Naam = model.Naam,
        Functie = model.Functie,
        Actief = model.Actief
    };

    private static MedewerkerFormViewModel ToForm(Medewerker medewerker) => new()
    {
        MedewerkerId = medewerker.MedewerkerId,
        Naam = medewerker.Naam,
        Functie = medewerker.Functie,
        Actief = medewerker.Actief
    };
}
