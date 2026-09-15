namespace CarApp.Repository;

public class SqlCommands
{
    public const string AddCar =
        "INSERT INTO Cars (\"Year\", Make, Model, Odometer, Price, Status) VALUES (@year, @make, @model, @odometer, @price, @status) RETURNING *;";
    public const string GetCarsByStatus =
        "SELECT Id, \"Year\", Make, Model, Odometer, Price, Status FROM Cars WHERE Status = @status;";
    public const string MarkCarAsSoldById = "UPDATE Cars SET Status = 'Sold' WHERE Id = @id";
    public const string UpdateCarById = "UPDATE Cars SET \"Year\" = @year, Make = @make, Model = @model, Odometer = @odometer, Price = @price WHERE Id = @id";
    public const string DeleteCarById = "DELETE FROM Cars WHERE Id = @id";
    
}