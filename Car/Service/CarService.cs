using CarApp.Models;
using CarApp.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarApp.Service;

public class CarService
{
    private readonly ICarRepository _repository;

    public CarService(ICarRepository repository)
    {
        _repository = repository;
    }

    public void AddCar(int year, string make, string model, long odometer, decimal price)
    {
        var car = new Car(year, make, model, odometer, price);
        _repository.Add(car);
    }

    public IReadOnlyCollection<Car> GetAvailableCars()
    {
        return _repository.GetAvailable();
    }

    public IReadOnlyCollection<Car> GetSoldCars()
    {
        return _repository.GetSold();
    }

    public void MarkCarAsSold(long id)
    {
        _repository.MarkAsSold(id);

    }

    public void UpdateCar(Car car, int year, string make, string model, long odometer, decimal price)
    {
        car.UpdateDetails(year, make, model, odometer, price);
        _repository.Update(car);
    }

    public void DeleteCar(long id)
    {
        _repository.Delete(id);
    }
}
