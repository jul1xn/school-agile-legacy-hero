namespace ExcellentTaste.Core.Models;

public class Dienst
{
    public int DienstId { get; set; }
    public int MedewerkerId { get; set; }
    public string MedewerkerNaam { get; set; } = "";
    public DateTime StartTijd { get; set; }
    public DateTime EindTijd { get; set; }
    public decimal GewerkteUren => (decimal)(EindTijd - StartTijd).TotalHours;
}
