namespace ExcellentTaste.Core.Models;

public class Dienst
{
    public int DienstId { get; set; }
    public int MedewerkerId { get; set; }
    public string MedewerkerNaam { get; set; } = "";
    public DateTime StartTijd { get; set; }
    public DateTime EindTijd { get; set; }
    public decimal GewerkteUren => (decimal)(EindTijd - StartTijd).TotalHours;

    public decimal UrenInPeriode(DateTime start, DateTime einde)
    {
        var beginBinnenPeriode = StartTijd > start ? StartTijd : start;
        var eindeBinnenPeriode = EindTijd < einde ? EindTijd : einde;
        return eindeBinnenPeriode > beginBinnenPeriode
            ? (decimal)(eindeBinnenPeriode - beginBinnenPeriode).TotalHours
            : 0;
    }
}
