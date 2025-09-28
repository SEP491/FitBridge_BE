using System;
using System.Text.Json.Serialization;

namespace FitBridge_Application.Dtos.OrderItems;

public class OrderItemDto
{
    public int Quantity { get; set; }
    [JsonIgnore]
    public decimal Price { get; set; }
    [JsonIgnore]
    public string ProductName { get; set; } = string.Empty;
    public Guid? ProductDetailId { get; set; }
    public Guid? GymCourseId { get; set; }
    public Guid? GymPtId { get; set; }
    [JsonIgnore]
    public Guid? GymCoursePTId { get; set; }
    public Guid? ServiceInformationId { get; set; }
    public Guid? FreelancePTPackageId { get; set; }
}
