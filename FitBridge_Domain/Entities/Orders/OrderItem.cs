using FitBridge_Domain.Entities.Ecommerce;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Entities.ServicePackages;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.Reports;

namespace FitBridge_Domain.Entities.Orders;

public class OrderItem : BaseEntity
{
    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public bool IsFeedback { get; set; }

    public Guid OrderId { get; set; }
    public Guid? GymPtId { get; set; }
    public Guid? CustomerPurchasedId { get; set; }

    public Order Order { get; set; }

    public Guid? ProductDetailId { get; set; }
    public Guid? GymCourseId { get; set; }
    public Guid? ServiceInformationId { get; set; }
    public Guid? FreelancePTPackageId { get; set; }
    public DateOnly? ProfitDistributePlannedDate { get; set; }
    public DateOnly? ProfitDistributeActualDate { get; set; }
    public bool IsRefunded { get; set; }
    public ServiceInformation? ServiceInformation { get; set; }
    public ProductDetail? ProductDetail { get; set; }
    public GymCourse? GymCourse { get; set; }
    public CustomerPurchased? CustomerPurchased { get; set; }
    public ApplicationUser? GymPt { get; set; }
    public FreelancePTPackage? FreelancePTPackage { get; set; }
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<ReportCases> ReportCases { get; set; } = new List<ReportCases>();
}