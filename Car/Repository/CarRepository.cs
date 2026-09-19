using CarApp.Data;
using CarApp.Models;
using CarApp.Exceptions;
using Dapper;
using System;
using System.Linq;

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
            throw new DataAccessException("Failed to add car to database.", e);
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
            throw new DataAccessException($"Failed to get cars with status '{status}'.", e);
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
            throw new DataAccessException($"Failed to mark car with id {id} as sold.", e);
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
            throw new DataAccessException($"Failed to update car with id {car.Id}.", e);
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
            throw new DataAccessException($"Failed to delete car with id {id}.", e);
        }
    }
}