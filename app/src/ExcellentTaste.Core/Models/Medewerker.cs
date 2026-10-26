namespace ExcellentTaste.Core.Models;

public class Medewerker
{
    public int MedewerkerId { get; set; }
    public string Naam { get; set; } = "";
    public string Functie { get; set; } = "";
    public bool Actief { get; set; } = true;
}
