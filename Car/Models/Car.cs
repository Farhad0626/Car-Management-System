namespace CarApp.Models;

public class Car
{
    public long Id { get; private set; }
    public int Year { get; private set; }
    public string Make { get; private set; }
    public string Model { get; private set; }
    public long Odometer { get; private set; }
    public decimal Price { get; private set; }
    public CarStatus Status { get; private set; }

    public Car(int year, string make, string model, long odometer, decimal price)
    {
        Year = year;
        Make = make;
        Model = model;
        Odometer = odometer;
        Price = price;
        Status = CarStatus.Available;
    }

    public Car(long id, int year, string make, string model,
               long odometer, decimal price, CarStatus status)
    {
        Id = id;
        Year = year;
        Make = make;
        Model = model;
        Odometer = odometer;
        Price = price;
        Status = status;
    }

    public void MarkAsSold()
    {
        Status = CarStatus.Sold;
    }

    public void UpdateDetails(int year, string make, string model, long odometer, decimal price)
    {
        Year = year;
        Make = make;
        Model = model;
        Odometer = odometer;
        Price = price;
    }
}
