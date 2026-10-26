using System.ComponentModel.DataAnnotations;
using ExcellentTaste.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExcellentTaste.Web.ViewModels;

public class RoosterViewModel
{
    public DateTime WeekStart { get; set; }
    public List<Dienst> Diensten { get; set; } = [];
    public List<DateTime> Dagen { get; set; } = [];
}

public class DienstFormViewModel
{
    public int DienstId { get; set; }

    [Display(Name = "Medewerker")]
    [Range(1, int.MaxValue, ErrorMessage = "Kies een medewerker.")]
    public int MedewerkerId { get; set; }

    [Display(Name = "Begint op")]
    [DataType(DataType.DateTime)]
    public DateTime StartTijd { get; set; } = DateTime.Today.AddHours(16);

    [Display(Name = "Eindigt op")]
    [DataType(DataType.DateTime)]
    public DateTime EindTijd { get; set; } = DateTime.Today.AddHours(22);

    public List<SelectListItem> Medewerkers { get; set; } = [];
}

public class MedewerkerFormViewModel
{
    public int MedewerkerId { get; set; }

    [Required(ErrorMessage = "Vul de naam in.")]
    [StringLength(100)]
    [Display(Name = "Naam")]
    public string Naam { get; set; } = "";

    [StringLength(60)]
    [Display(Name = "Functie")]
    public string Functie { get; set; } = "";

    [Display(Name = "Actief")]
    public bool Actief { get; set; } = true;
}
