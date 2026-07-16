using CarApp.Data;
using CarApp.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace CarApp.Repository;

public class CarRepository : ICarRepository
{
    private readonly IDatabase _database;

    public CarRepository(IDatabase database)
    {
        _database = database;
    }

    public Car? Add(Car car)
    {
        using var connection = _database.GetConnection();

        try
        {
            connection.Open();

            var command = new NpgsqlCommand(
                @"INSERT INTO Cars (""Year"", Make, Model, Odometer, Price, Status)
                  VALUES (@year, @make, @model, @odometer, @price, @status)
                  RETURNING *;",
                connection);

            command.Parameters.AddWithValue("year", car.Year);
            command.Parameters.AddWithValue("make", car.Make);
            command.Parameters.AddWithValue("model", car.Model);
            command.Parameters.AddWithValue("odometer", car.Odometer);
            command.Parameters.AddWithValue("price", car.Price);
            command.Parameters.AddWithValue("status", car.Status.ToString());

            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new Car(
                    reader.GetInt64(0),
                    reader.GetInt32(1),
                    reader.GetString(2),
                    reader.GetString(3),
                    reader.GetInt64(4),
                    reader.GetDecimal(5),
                    Enum.Parse<CarStatus>(reader.GetString(6))
                    );
            }
        }
        catch (Exception e)
        {
            MessageBox.ErrorQuery("Database Error", e.ToString(), "_OK");
            throw;
        }
        finally
        {
            if (connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
            }
        }

        return null;
    }

    private IReadOnlyCollection<Car> GetByStatus(string status)
    {
        var cars = new List<Car>();

        using var connection = _database.GetConnection();

        try
        {
            connection.Open();

            var command = new NpgsqlCommand(
                @"SELECT Id, ""Year"", Make, Model, Odometer, Price, Status
                  FROM Cars
                  WHERE Status = @status;",
                connection);

            command.Parameters.AddWithValue("status", status);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var car = new Car(
                    reader.GetInt64(0),
                    reader.GetInt32(1),
                    reader.GetString(2),
                    reader.GetString(3),
                    reader.GetInt64(4),
                    reader.GetDecimal(5),
                    Enum.Parse<CarStatus>(reader.GetString(6))
                    );

                cars.Add(car);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return [];
        }
        finally
        {
            if (connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
            }
        }

        return cars;
    }

    public IReadOnlyCollection<Car> GetAvailable()
    {
        return GetByStatus("Available");
    }

    public IReadOnlyCollection<Car> GetSold()
    {
        return GetByStatus("Sold");
    }

    public bool MarkAsSold(long id)
    {
        try
        {
            using var connection = _database.GetConnection();
            connection.Open();

            var command = new NpgsqlCommand(
                @"UPDATE Cars SET Status = 'Sold' WHERE Id = @id;",
                connection);

            command.Parameters.AddWithValue("id", id);

            return command.ExecuteNonQuery() == 1;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public void Update(Car car)
    {
        try
        {
            using var connection = _database.GetConnection();
            connection.Open();

            var command = new NpgsqlCommand(
                @"UPDATE Cars
                  SET ""Year"" = @year, Make = @make, Model = @model,
                      Odometer = @odometer, Price = @price
                  WHERE Id = @id;",
                connection);

            command.Parameters.AddWithValue("year", car.Year);
            command.Parameters.AddWithValue("make", car.Make);
            command.Parameters.AddWithValue("model", car.Model);
            command.Parameters.AddWithValue("odometer", car.Odometer);
            command.Parameters.AddWithValue("price", car.Price);
            command.Parameters.AddWithValue("id", car.Id);

            command.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public void Delete(long id)
    {
        try
        {
            using var connection = _database.GetConnection();
            connection.Open();

            var command = new NpgsqlCommand(
                @"DELETE FROM Cars WHERE Id = @id;",
                connection);

            command.Parameters.AddWithValue("id", id);

            command.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}