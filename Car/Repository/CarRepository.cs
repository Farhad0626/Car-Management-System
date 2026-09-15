using CarApp.Data;
using CarApp.Models;
using Terminal.Gui;
using Dapper;
using System;

namespace CarApp.Repository;

public class CarRepository(IDatabaseConnectionFactory databaseConnectionFactory) : ICarRepository
{
    // 1. Use Native Objects to interact with the database
    // 2. Use Lite-ORM (Dapper, PetaPoco, etc.)
    // 3. Use Full-ORM (Entity Framework, NHibernate, etc.)

    public Car? Add(Car car)
    {
        using var connection = databaseConnectionFactory.GetConnection();

        try
        {
            var result = connection.QueryFirst<Car?>(
                SqlCommands.AddCar,
                new
                {
                    year = car.Year,
                    make = car.Make,
                    model = car.Model,
                    odometer = car.Odometer,
                    price = car.Price,
                    status = car.Status.ToString()
                });

            return result;
        }
        catch (Exception e)
        {
            // This is a problem!!!
            MessageBox.ErrorQuery("Database Error", e.ToString(), "_OK");
            throw;
        }
    }

    private IReadOnlyCollection<Car> GetByStatus(string status)
    {
        using var connection = databaseConnectionFactory.GetConnection();

        try
        {
            return connection.Query<Car>(
                SqlCommands.GetCarsByStatus,
                new { status }).ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Array.Empty<Car>();
        }
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
            using var connection = databaseConnectionFactory.GetConnection();
            return connection.Execute(
                SqlCommands.MarkCarAsSoldById,
                new { id }) == 1;
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
            using var connection = databaseConnectionFactory.GetConnection();
            connection.Execute(
                SqlCommands.UpdateCarById,
                new
                {
                    year = car.Year,
                    make = car.Make,
                    model = car.Model,
                    odometer = car.Odometer,
                    price = car.Price,
                    id = car.Id
                });
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
            using var connection = databaseConnectionFactory.GetConnection();
            connection.Execute(
                SqlCommands.DeleteCarById,
                new { id });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}