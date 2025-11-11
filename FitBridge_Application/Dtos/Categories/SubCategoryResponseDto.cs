using System;

namespace FitBridge_Application.Dtos.Categories;

public class SubCategoryResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid CategoryId { get; set; }
}
