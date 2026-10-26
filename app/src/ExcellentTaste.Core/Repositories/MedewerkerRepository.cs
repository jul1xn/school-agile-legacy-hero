using ExcellentTaste.Core.Data;
using ExcellentTaste.Core.Models;

namespace ExcellentTaste.Core.Repositories;

public class MedewerkerRepository : RepositoryBase
{
    private readonly DatabaseConnection _database;

    public MedewerkerRepository(DatabaseConnection database)
    {
        _database = database;
    }

    public List<Medewerker> GetAll()
    {
        using var connection = _database.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT medewerker_id, naam, functie, actief FROM medewerker ORDER BY naam";
        using var reader = command.ExecuteReader();
        var medewerkers = new List<Medewerker>();
        while (reader.Read()) medewerkers.Add(Map(reader));
        return medewerkers;
    }

    public Medewerker? GetById(int id)
    {
        using var connection = _database.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT medewerker_id, naam, functie, actief FROM medewerker WHERE medewerker_id = @id";
        Add(command, "@id", id);
        using var reader = command.ExecuteReader();
        return reader.Read() ? Map(reader) : null;
    }

    public int Add(Medewerker medewerker)
    {
        using var connection = _database.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO medewerker (naam, functie, actief) VALUES (@naam, @functie, @actief); SELECT LAST_INSERT_ID();";
        AddParameters(command, medewerker);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public void Update(int id, Medewerker medewerker)
    {
        using var connection = _database.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE medewerker SET naam = @naam, functie = @functie, actief = @actief WHERE medewerker_id = @id";
        Add(command, "@id", id);
        AddParameters(command, medewerker);
        command.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var connection = _database.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM medewerker WHERE medewerker_id = @id";
        Add(command, "@id", id);
        command.ExecuteNonQuery();
    }

    private static void AddParameters(MySqlConnector.MySqlCommand command, Medewerker medewerker)
    {
        Add(command, "@naam", medewerker.Naam.Trim());
        Add(command, "@functie", medewerker.Functie.Trim());
        Add(command, "@actief", medewerker.Actief);
    }

    private static Medewerker Map(MySqlConnector.MySqlDataReader reader) => new()
    {
        MedewerkerId = Int(reader, "medewerker_id"),
        Naam = Text(reader, "naam"),
        Functie = Text(reader, "functie"),
        Actief = Int(reader, "actief") == 1
    };
}
