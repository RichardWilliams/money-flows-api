using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.Api.Shared.Models;

public sealed class AttachmentSettings
{
    [Required]
    public string BasePath { get; init; } = string.Empty;

    [Range(1, 100)]
    public int MaxFileSizeMb { get; init; } = 10;

    [Required]
    [MinLength(1)]
    public string[] AllowedExtensions { get; init; } = Array.Empty<string>();
}
