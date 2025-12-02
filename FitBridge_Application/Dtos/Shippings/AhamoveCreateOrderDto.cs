namespace FitBridge_Application.Dtos.Shippings;

public class AhamoveCreateOrderDto
{
    public long OrderTime { get; set; }
    public List<AhamovePathDto> Path { get; set; }
    public string ServiceId { get; set; }
    public List<AhamoveRequestDto>? Requests { get; set; }
    public string PaymentMethod { get; set; }
    public string? Remarks { get; set; }
    public string? PromoCode { get; set; }
}

