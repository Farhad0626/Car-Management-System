using System;
using Npgsql;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarApp.Data;

public class Postgresdb : IDatabase
{
    private readonly string _connectionString =
        Environment.GetEnvironmentVariable("CARAPP_CONNECTION")
        ?? throw new InvalidOperationException(
             "Please create the 'CARAPP_CONNECTION' environment variable before running the application.");

    public NpgsqlConnection GetConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}
