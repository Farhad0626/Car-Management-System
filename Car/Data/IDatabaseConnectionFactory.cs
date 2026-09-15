using Npgsql;

namespace CarApp.Data;

public interface IDatabaseConnectionFactory
{
    NpgsqlConnection GetConnection();
}
