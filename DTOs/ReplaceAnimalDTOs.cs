using System.ComponentModel.DataAnnotations;

namespace APBD6.DTOs;

public record ReplaceAnimalRequest(
    [Required] [MaxLength(50)] string Name,
    [Required] [MaxLength(50)] string Description,
    [Required] [MaxLength(50)] string Category,
    [Required] [MaxLength(50)] string Area
);