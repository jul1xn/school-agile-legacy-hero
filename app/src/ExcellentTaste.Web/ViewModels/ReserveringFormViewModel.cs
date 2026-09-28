using System.ComponentModel.DataAnnotations;
using ExcellentTaste.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExcellentTaste.Web.ViewModels;

public class ReserveringFormViewModel
{
    public int ReserveringId { get; set; }

    [Display(Name = "Klant")]
    public int KlantId { get; set; } = -1;

    [Display(Name = "Klantnaam")]
    public string? KlantNaam { get; set; }

    [Display(Name = "E-mailadres klant")]
    public string? Email { get; set; }

    [Display(Name = "Telefoonnummer klant")]
    public string? Telefoon { get; set; }

    [Display(Name = "Reserveringsdatum (dd-mm-jjjj)")]
    public DateOnly Datum { get; set; } = new DateOnly();

    [Display(Name = "Reserveringstijd (uu:mm)")]
    public TimeOnly Tijd { get; set; } = new TimeOnly();

    [Range(1, 10, ErrorMessage = "Kies een tafelnummer van 1 tot en met 10.")]
    [Display(Name = "Tafelnummer")]
    public int Tafel { get; set; }

    [Range(1, 100, ErrorMessage = "Vul minimaal 1 persoon in.")]
    [Display(Name = "Aantal personen")]
    public int Aantal { get; set; }

    public List<SelectListItem> Klanten { get; set; } = [];

    public Reservering ToReservering(int klantId)
    {
        return new Reservering
        {
            ReserveringId = ReserveringId,
            KlantId = klantId,
            Datum = Datum,
            Tijd = Tijd,
            Tafel = Tafel,
            Aantal = Aantal,
            Status = 1
        };
    }
}
