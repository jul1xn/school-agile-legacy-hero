using ExcellentTaste.Core.Models;
using ExcellentTaste.Core.Repositories;

namespace ExcellentTaste.Core.Services;

public class ReserveringService
{
    private readonly ReserveringRepository _reserveringen;

    public ReserveringService(ReserveringRepository reserveringen)
    {
        _reserveringen = reserveringen;
    }

    public List<Reservering> GetAll() => _reserveringen.GetAll();

    public Reservering? GetById(int id) => _reserveringen.GetById(id);

    public (bool Gelukt, string Bericht, int Id) Add(Reservering reservering)
    {
        try
        {
            var id = _reserveringen.Add(reservering);
            return (true, "Reservering opgeslagen.", id);
        }
        catch (Exception ex)
        {
            return (false, ex.Message, -1);
        }
    }

    public (bool Gelukt, string Bericht) Update(int id, Reservering reservering)
    {
        _reserveringen.Update(id, reservering);
        return (true, "Reservering opgeslagen.");
    }

    public void Delete(int id) => _reserveringen.Delete(id);
}
