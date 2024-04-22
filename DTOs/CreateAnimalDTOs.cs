using System.ComponentModel.DataAnnotations;

namespace APBD6.DTOs;

public record CreateAnimalRequest(
    [Required] [MaxLength(50)] string Name,
    [Required] [MaxLength(50)] string Description,
    [Required] [MaxLength(50)] string Category,
    [Required] [MaxLength(50)] string Area
);

public record CreateAnimalResponse(int IdAnimal, string Name, string Description, string Category, string Area)
{
    public CreateAnimalResponse(int id, CreateAnimalRequest request) : this(id, request.Name, request.Description,
        request.Category, request.Area){}
}