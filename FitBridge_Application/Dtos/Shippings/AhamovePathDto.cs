namespace FitBridge_Application.Dtos.Shippings;

public class AhamovePathDto
{
    public double Lat { get; set; }
    public double Lng { get; set; }
    public string Address { get; set; }
    public string ShortAddress { get; set; }
    public string Name { get; set; }
    public string Mobile { get; set; }
    public string? Remarks { get; set; }
    public decimal? Cod { get; set; }
    public decimal? ItemValue { get; set; }
    public string? TrackingNumber { get; set; }
}

