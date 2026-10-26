using MySqlConnector;

namespace ExcellentTaste.Core.Data;

public class DatabaseConnection
{
    private readonly string _connectionString;

    public DatabaseConnection(string? connectionString)
    {
        _connectionString = connectionString ?? "";
    }

    public MySqlConnection OpenConnection()
    {
        if (string.IsNullOrWhiteSpace(_connectionString))
        {
            throw new DatabaseUnavailableException(
                "De database is niet bereikbaar. Neem contact op met de administrator.",
                new InvalidOperationException("Connection string 'ExcellentTaste' ontbreekt. Geef een MySQL-verbinding mee."));
        }

        var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();
            return connection;
        }
        catch (Exception ex) when (ex is MySqlException or TimeoutException)
        {
            connection.Dispose();
            throw new DatabaseUnavailableException("De database is niet bereikbaar. Neem contact op met de administrator.", ex);
        }
    }

    public void Initialize()
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
CREATE TABLE IF NOT EXISTS medewerker (
    medewerker_id int(11) NOT NULL AUTO_INCREMENT,
    naam varchar(100) NOT NULL,
    functie varchar(60) NOT NULL DEFAULT '',
    actief tinyint(1) NOT NULL DEFAULT 1,
    PRIMARY KEY (medewerker_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8
""";
        command.ExecuteNonQuery();

        command.CommandText = """
CREATE TABLE IF NOT EXISTS dienst (
    dienst_id int(11) NOT NULL AUTO_INCREMENT,
    medewerker_id int(11) NOT NULL,
    start_tijd datetime NOT NULL,
    eind_tijd datetime NOT NULL,
    PRIMARY KEY (dienst_id),
    KEY ix_dienst_start_tijd (start_tijd),
    KEY ix_dienst_medewerker (medewerker_id),
    CONSTRAINT fk_dienst_medewerker FOREIGN KEY (medewerker_id)
        REFERENCES medewerker (medewerker_id) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8
""";
        command.ExecuteNonQuery();

        command.CommandText = "SELECT COUNT(*) FROM klant";
        command.ExecuteScalar();
    }
}
