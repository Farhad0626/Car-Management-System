using CarApp.Models;

namespace CarApp.Repository;

public interface ICarRepository
{
    Car? Add(Car car);
    IReadOnlyCollection<Car> GetAvailable();
    IReadOnlyCollection<Car> GetSold();
    bool MarkAsSold(long id);
    void Update(Car car);
    void Delete(long id);
}
