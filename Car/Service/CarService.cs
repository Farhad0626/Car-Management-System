using CarApp.Models;
using CarApp.Repository;

namespace CarApp.Service;

public class CarService(ICarRepository repository)
{
    public void AddCar(int year, string make, string model, long odometer, decimal price)
    {
        var car = new Car(year, make, model, odometer, price);
        repository.Add(car);
    }

    public IReadOnlyCollection<Car> GetAvailableCars()
    {
        return repository.GetAvailable();
    }

    public IReadOnlyCollection<Car> GetSoldCars()
    {
        return repository.GetSold();
    }

    public void MarkCarAsSold(long id)
    {
        repository.MarkAsSold(id);

    }

    public void UpdateCar(Car car, int year, string make, string model, long odometer, decimal price)
    {
        car.UpdateDetails(year, make, model, odometer, price);
        repository.Update(car);
    }

    public void DeleteCar(long id)
    {
        repository.Delete(id);
    }
}
