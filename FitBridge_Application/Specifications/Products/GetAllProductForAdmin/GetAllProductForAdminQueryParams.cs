using System;

namespace FitBridge_Application.Specifications.Products.GetAllProductForAdmin;

public class GetAllProductForAdminQueryParams : BaseParams
{
    public bool? IsDisplayed { get; set; }
    public Guid? ProductId { get; set; }
    public Guid? SubCategoryId { get; set; }
    public Guid? BrandId { get; set; }
}
