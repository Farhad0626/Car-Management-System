using System;
using Npgsql;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarApp.Data;

public interface IDatabase
{
    NpgsqlConnection GetConnection();
}
