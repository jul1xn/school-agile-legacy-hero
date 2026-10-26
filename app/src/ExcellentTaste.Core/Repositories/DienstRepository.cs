using ExcellentTaste.Core.Data;
using ExcellentTaste.Core.Models;

namespace ExcellentTaste.Core.Repositories;

public class DienstRepository : RepositoryBase
{
    private readonly DatabaseConnection _database;

    public DienstRepository(DatabaseConnection database)
    {
        _database = database;
    }

    public List<Dienst> GetByWeek(DateTime start, DateTime einde)
    {
        using var connection = _database.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
SELECT d.dienst_id, d.medewerker_id, m.naam AS medewerker_naam, d.start_tijd, d.eind_tijd
FROM dienst d JOIN medewerker m ON d.medewerker_id = m.medewerker_id
WHERE d.start_tijd >= @start AND d.start_tijd < @einde
ORDER BY d.start_tijd, m.naam
""";
        Add(command, "@start", start);
        Add(command, "@einde", einde);
        using var reader = command.ExecuteReader();
        var diensten = new List<Dienst>();
        while (reader.Read()) diensten.Add(Map(reader));
        return diensten;
    }

    public Dienst? GetById(int id)
    {
        using var connection = _database.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
SELECT d.dienst_id, d.medewerker_id, m.naam AS medewerker_naam, d.start_tijd, d.eind_tijd
FROM dienst d JOIN medewerker m ON d.medewerker_id = m.medewerker_id
WHERE d.dienst_id = @id
""";
        Add(command, "@id", id);
        using var reader = command.ExecuteReader();
        return reader.Read() ? Map(reader) : null;
    }

    public int Add(Dienst dienst)
    {
        using var connection = _database.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO dienst (medewerker_id, start_tijd, eind_tijd) VALUES (@medewerkerId, @start, @einde); SELECT LAST_INSERT_ID();";
        AddParameters(command, dienst);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public void Update(int id, Dienst dienst)
    {
        using var connection = _database.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE dienst SET medewerker_id = @medewerkerId, start_tijd = @start, eind_tijd = @einde WHERE dienst_id = @id";
        Add(command, "@id", id);
        AddParameters(command, dienst);
        command.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var connection = _database.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM dienst WHERE dienst_id = @id";
        Add(command, "@id", id);
        command.ExecuteNonQuery();
    }

    private static void AddParameters(MySqlConnector.MySqlCommand command, Dienst dienst)
    {
        Add(command, "@medewerkerId", dienst.MedewerkerId);
        Add(command, "@start", dienst.StartTijd);
        Add(command, "@einde", dienst.EindTijd);
    }

    private static Dienst Map(MySqlConnector.MySqlDataReader reader) => new()
    {
        DienstId = Int(reader, "dienst_id"),
        MedewerkerId = Int(reader, "medewerker_id"),
        MedewerkerNaam = Text(reader, "medewerker_naam"),
        StartTijd = Convert.ToDateTime(reader["start_tijd"]),
        EindTijd = Convert.ToDateTime(reader["eind_tijd"])
    };
}
