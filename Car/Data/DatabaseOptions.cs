using System.ComponentModel.DataAnnotations;

namespace CarApp.Data;

public class DatabaseOptions
{
    public const string SectionName = "Database";

    [Required(AllowEmptyStrings = false, ErrorMessage =
        "Please set the 'Database:ConnectionString' configuration value " +
        "(e.g. in appsettings.json, or via the 'Database__ConnectionString' environment variable) " +
        "before running the application.")]
    public string ConnectionString { get; set; } = string.Empty;
}
