    using System;
    using FitBridge_Domain.Entities.Orders;
    using FitBridge_Domain.Exceptions;
    using FitBridge_Application.Interfaces.Repositories;
    using FitBridge_Domain.Entities.Ecommerce;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Application.Interfaces.Services;

namespace FitBridge_Application.Services;

public class OrderService(IUnitOfWork _unitOfWork, IApplicationUserService _applicationUserService)
{
    public async Task<bool> ReturnQuantityToProductDetail(Order order)
    {
        foreach (var orderItem in order.OrderItems)
        {
            var productDetail = await _unitOfWork.Repository<ProductDetail>().GetByIdAsync(orderItem.ProductDetailId.Value);
            if (productDetail == null)
            {
                throw new NotFoundException("Product detail not found");
            }
            productDetail.Quantity += orderItem.Quantity;
            productDetail.SoldQuantity -= orderItem.Quantity;
            productDetail.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Repository<ProductDetail>().Update(productDetail);
        }
        return true;
    }

    public async Task<bool> UpdateProductDetailQuantity(ProductDetail productDetail, int quantity)
    {
        productDetail.Quantity -= quantity;
        productDetail.SoldQuantity += quantity;
        productDetail.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<ProductDetail>().Update(productDetail);
        return true;
    }

    public async Task<bool> UpdatePTCurrentCourse(ApplicationUser Pt)
    {
        Pt.PtCurrentCourse += 1;
        Pt.UpdatedAt = DateTime.UtcNow;
        await _applicationUserService.UpdateAsync(Pt);
        return true;
    }

    public async Task<bool> ReturnQuantityToPT(Order order)
    {
        foreach (var orderItem in order.OrderItems)
        {
            var ptId = orderItem.GymPtId ?? orderItem.FreelancePTPackage.PtId;
            var pt = await _applicationUserService.GetByIdAsync(ptId, null, true);
            if (pt == null)
            {
                throw new NotFoundException("PT not found");
            }
            pt.PtCurrentCourse--;
            return true;
        }
        return true;
    }
}
