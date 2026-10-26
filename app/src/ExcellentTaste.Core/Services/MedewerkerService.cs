using ExcellentTaste.Core.Models;
using ExcellentTaste.Core.Repositories;

namespace ExcellentTaste.Core.Services;

public class MedewerkerService
{
    private readonly MedewerkerRepository _medewerkers;

    public MedewerkerService(MedewerkerRepository medewerkers) => _medewerkers = medewerkers;

    public List<Medewerker> GetAll() => _medewerkers.GetAll();
    public Medewerker? GetById(int id) => _medewerkers.GetById(id);
    public int Add(Medewerker medewerker) => _medewerkers.Add(medewerker);
    public void Update(int id, Medewerker medewerker) => _medewerkers.Update(id, medewerker);
    public void Delete(int id) => _medewerkers.Delete(id);
}
