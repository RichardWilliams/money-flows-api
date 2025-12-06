using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.Api.Shared.Models;

public sealed class CorsSettings
{
    [Required]
    [MinLength(1)]
    public string[] AllowedOrigins { get; init; } = Array.Empty<string>();
}
