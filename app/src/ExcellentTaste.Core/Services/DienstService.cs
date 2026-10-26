using ExcellentTaste.Core.Models;
using ExcellentTaste.Core.Repositories;

namespace ExcellentTaste.Core.Services;

public class DienstService
{
    private readonly DienstRepository _diensten;

    public DienstService(DienstRepository diensten) => _diensten = diensten;

    public List<Dienst> GetByWeek(DateTime start, DateTime einde) => _diensten.GetByWeek(start, einde);
    public Dienst? GetById(int id) => _diensten.GetById(id);

    public (bool Gelukt, string Bericht, int Id) Add(Dienst dienst)
    {
        if (dienst.EindTijd <= dienst.StartTijd)
            return (false, "De eindtijd moet na de begintijd liggen.", 0);
        return (true, "Dienst opgeslagen.", _diensten.Add(dienst));
    }

    public (bool Gelukt, string Bericht) Update(int id, Dienst dienst)
    {
        if (dienst.EindTijd <= dienst.StartTijd)
            return (false, "De eindtijd moet na de begintijd liggen.");
        _diensten.Update(id, dienst);
        return (true, "Dienst opgeslagen.");
    }

    public void Delete(int id) => _diensten.Delete(id);
}
