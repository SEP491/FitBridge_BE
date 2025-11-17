using FitBridge_Application.Dtos.Shippings;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.Orders;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using FitBridge_Domain.Entities.Accounts;

namespace FitBridge_Application.Features.Orders.CreateShippingOrder;

public class CreateShippingOrderCommandHandler : IRequestHandler<CreateShippingOrderCommand, CreateShippingOrderResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAhamoveService _ahamoveService;
    private readonly ITransactionService _transactionService;
    private readonly ILogger<CreateShippingOrderCommandHandler> _logger;

    public CreateShippingOrderCommandHandler(
        IUnitOfWork unitOfWork,
        IAhamoveService ahamoveService,
        ITransactionService transactionService,
        ILogger<CreateShippingOrderCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _ahamoveService = ahamoveService;
        _transactionService = transactionService;
        _logger = logger;
    }

    public async Task<CreateShippingOrderResponseDto> Handle(CreateShippingOrderCommand request, CancellationToken cancellationToken)
    {
        // Get order from database
        var order = await _unitOfWork.Repository<Order>().GetByIdAsync(request.OrderId, includes: new List<string> { "Transactions", "Address", "Account" });
        
        if (order == null)
        {
            throw new NotFoundException($"Order with ID {request.OrderId} not found");
        }

        // Validate order status
        if (order.Status == OrderStatus.Shipping || order.Status == OrderStatus.Arrived || order.Status == OrderStatus.Finished)
        {
            throw new BusinessException($"Order is already in {order.Status} status and cannot be shipped again");
        }
        var shopAddress = await _unitOfWork.Repository<Address>().GetByIdAsync(Guid.Parse(ProjectConstant.DefaultShopAddressId));
        if (shopAddress == null)
        {
            throw new NotFoundException("Shop address not found");
        }
    
        var pickUpaddress = new AhamovePathDto
        {
            Lat = shopAddress.Latitude,
            Lng = shopAddress.Longitude,
            Address = shopAddress.GoogleMapAddressString,
            ShortAddress = shopAddress.Ward + ", " + shopAddress.District,
            Name = shopAddress.ReceiverName,
            Mobile = shopAddress.PhoneNumber,
            Remarks = request.Remarks ?? shopAddress.Note,
        };

        var deliveryAddress = new AhamovePathDto
        {
            Lat = order.Address.Latitude,
            Lng = order.Address.Longitude,
            Address = order.Address.GoogleMapAddressString,
            ShortAddress = order.Address.Ward + ", " + order.Address.District,
            Name = order.Address.ReceiverName,
            Mobile = order.Address.PhoneNumber,
            Cod = order.TotalAmount,
            Remarks = order.Address.Note,
            TrackingNumber = order.Id.ToString(),
        };

        try
        {
            // Prepare Ahamove order request
            var ahamoveRequest = new AhamoveCreateOrderDto
            {
                OrderTime = 0, // 0 means order immediately
                Path = new List<AhamovePathDto>
                {
                    pickUpaddress,
                    deliveryAddress
                },
                ServiceId = ProjectConstant.DefaultAhamoveServiceId,
                PaymentMethod = "CASH",
                Remarks = request.Remarks
            };

            // Call Ahamove API to create order
            _logger.LogInformation($"Creating Ahamove shipping order for Order ID: {request.OrderId}");
            var ahamoveResponse = await _ahamoveService.CreateOrderAsync(ahamoveRequest);

            // Update order and transaction using TransactionService
            await _transactionService.UpdateOrderShippingDetails(
                orderId: order.Id,
                shippingActualCost: ahamoveResponse.Order.TotalFee,
                shippingTrackingId: ahamoveResponse.OrderId
            );

            _logger.LogInformation($"Successfully created Ahamove order {ahamoveResponse.OrderId} for Order ID: {request.OrderId}. Shared link: {ahamoveResponse.SharedLink}");

            return new CreateShippingOrderResponseDto
            {
                AhamoveOrderId = ahamoveResponse.OrderId,
                Status = ahamoveResponse.Status,
                ShippingFeeActualCost = ahamoveResponse.Order.TotalFee,
                Message = "Shipping order created successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to create shipping order for Order ID: {request.OrderId}");
            throw;
        }
    }
}

