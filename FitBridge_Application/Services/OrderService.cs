using System;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Exceptions;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Ecommerce;

namespace FitBridge_Application.Services;

public class OrderService(IUnitOfWork _unitOfWork)
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
            _unitOfWork.Repository<ProductDetail>().Update(productDetail);
        }
        return true;
    }
}
