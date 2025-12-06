using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.Api.Shared.Models;

public sealed class DatabaseSettings
{
    [Required]
    public string ConnectionString { get; init; } = string.Empty;
}
