namespace FitBridge_Application.Dtos.Shippings;

public class AhamoveOrderResponseDto
{
    public string _id { get; set; }
    public string OrderId { get; set; }
    public string Status { get; set; }
    public decimal TotalFee { get; set; }
    public decimal TotalPrice { get; set; }
    public string Path { get; set; }
    public long CreateTime { get; set; }
}

