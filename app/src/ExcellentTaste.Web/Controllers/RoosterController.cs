using System.Globalization;
using System.Net.Http.Json;
using ExcellentTaste.Core.Models;
using ExcellentTaste.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExcellentTaste.Web.Controllers;

public class RoosterController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<RoosterController> _logger;

    public RoosterController(IHttpClientFactory httpClientFactory, ILogger<RoosterController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<IActionResult> Index(DateTime? week)
    {
        var gekozenDatum = (week ?? DateTime.Today).Date;
        var start = gekozenDatum.AddDays(-(((int)gekozenDatum.DayOfWeek + 6) % 7));
        var eind = start.AddDays(7);
        var model = new RoosterViewModel
        {
            WeekStart = start,
            Dagen = Enumerable.Range(0, 7).Select(dag => start.AddDays(dag)).ToList()
        };

        try
        {
            var client = CreateApiClient();
            var url = $"api/diensten?start={Uri.EscapeDataString(start.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture))}&einde={Uri.EscapeDataString(eind.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture))}";
            model.Diensten = await client.GetFromJsonAsync<List<Dienst>>(url) ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Het rooster kon niet via de API worden opgehaald.");
            ViewBag.ApiFout = "Het rooster is niet bereikbaar. Neem contact op met de administrator.";
        }

        return View(model);
    }

    public async Task<IActionResult> Create(DateTime? dag)
    {
        var start = (dag ?? DateTime.Today).Date.AddHours(16);
        return View(await BuildForm(new DienstFormViewModel { StartTijd = start, EindTijd = start.AddHours(6) }));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DienstFormViewModel model)
    {
        if (model.EindTijd <= model.StartTijd)
            ModelState.AddModelError(nameof(model.EindTijd), "De eindtijd moet na de begintijd liggen.");
        if (!ModelState.IsValid) return View(await BuildForm(model));

        var response = await CreateApiClient().PostAsJsonAsync("api/diensten", ToDienst(model));
        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "De dienst kon niet worden opgeslagen.");
            return View(await BuildForm(model));
        }
        return RedirectToAction(nameof(Index), new { week = model.StartTijd.Date });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var dienst = await CreateApiClient().GetFromJsonAsync<Dienst>($"api/diensten/{id}");
        if (dienst is null) return NotFound();
        return View(await BuildForm(new DienstFormViewModel
        {
            DienstId = dienst.DienstId,
            MedewerkerId = dienst.MedewerkerId,
            StartTijd = dienst.StartTijd,
            EindTijd = dienst.EindTijd
        }));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, DienstFormViewModel model)
    {
        model.DienstId = id;
        if (model.EindTijd <= model.StartTijd)
            ModelState.AddModelError(nameof(model.EindTijd), "De eindtijd moet na de begintijd liggen.");
        if (!ModelState.IsValid) return View(await BuildForm(model));

        var response = await CreateApiClient().PutAsJsonAsync($"api/diensten/{id}", ToDienst(model));
        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "De dienst kon niet worden opgeslagen.");
            return View(await BuildForm(model));
        }
        return RedirectToAction(nameof(Index), new { week = model.StartTijd.Date });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, DateTime week)
    {
        await CreateApiClient().DeleteAsync($"api/diensten/{id}");
        return RedirectToAction(nameof(Index), new { week });
    }

    private HttpClient CreateApiClient() => _httpClientFactory.CreateClient("ExcellentTasteApi");

    private async Task<DienstFormViewModel> BuildForm(DienstFormViewModel model)
    {
        try
        {
            var medewerkers = await CreateApiClient().GetFromJsonAsync<List<Medewerker>>("api/medewerkers") ?? [];
            model.Medewerkers = medewerkers.Where(m => m.Actief)
                .Select(m => new SelectListItem(m.Naam, m.MedewerkerId.ToString(CultureInfo.InvariantCulture), m.MedewerkerId == model.MedewerkerId)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Medewerkers konden niet via de API worden opgehaald.");
            ModelState.AddModelError("", "Medewerkers konden niet worden opgehaald.");
        }
        return model;
    }

    private static Dienst ToDienst(DienstFormViewModel model) => new()
    {
        DienstId = model.DienstId,
        MedewerkerId = model.MedewerkerId,
        StartTijd = model.StartTijd,
        EindTijd = model.EindTijd
    };
}
