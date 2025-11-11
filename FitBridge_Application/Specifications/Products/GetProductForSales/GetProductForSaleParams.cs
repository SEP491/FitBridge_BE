using System;

namespace FitBridge_Application.Specifications.Products.GetProductForSales;

public class GetProductForSaleParams : BaseParams
{
    public decimal? FromPrice { get; set; }
    public decimal? ToPrice { get; set; }
    public double? Rating { get; set; }
    public Guid? BrandId { get; set; }
    public Guid? SubCategoryId { get; set; }
    public Guid? CategoryId { get; set; }
}
