using Npgsql;
using Microsoft.Extensions.Options;

namespace CarApp.Data;

public class PostgresConnectionFactory(IOptions<DatabaseOptions> options) : IDatabaseConnectionFactory, IDisposable
{
    // NpgsqlDataSource is Npgsql's connection factory: it owns the connection
    // pool for a given connection string and should be created once and reused,
    // handing out a new NpgsqlConnection per GetConnection() call.
    private readonly NpgsqlDataSource _dataSource = NpgsqlDataSource.Create(options.Value.ConnectionString);

    public NpgsqlConnection GetConnection()
    {
        return _dataSource.CreateConnection();
    }

    public void Dispose()
    {
        _dataSource.Dispose();
    }
}
