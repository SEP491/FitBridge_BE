using FitBridge_Application.Dtos.Shippings;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.Orders;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

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
        var order = await _unitOfWork.Repository<Order>().GetByIdAsync(request.OrderId, includes: new List<string> { "Transactions", "Address" });
        
        if (order == null)
        {
            throw new NotFoundException($"Order with ID {request.OrderId} not found");
        }

        // Validate order status
        if (order.Status == OrderStatus.Shipping || order.Status == OrderStatus.Arrived || order.Status == OrderStatus.Finished)
        {
            throw new BusinessException($"Order is already in {order.Status} status and cannot be shipped again");
        }

        try
        {
            // Prepare Ahamove order request
            var ahamoveRequest = new AhamoveCreateOrderDto
            {
                OrderTime = 0, // 0 means order immediately
                Path = new List<AhamovePathDto>
                {
                    request.PickupAddress,
                    request.DeliveryAddress
                },
                ServiceId = "SGN-BIKE",
                PaymentMethod = "CASH",
                Remarks = request.Remarks
            };

            // Call Ahamove API to create order
            _logger.LogInformation($"Creating Ahamove shipping order for Order ID: {request.OrderId}");
            var ahamoveResponse = await _ahamoveService.CreateOrderAsync(ahamoveRequest);

            // Update order and transaction using TransactionService
            await _transactionService.UpdateOrderShippingDetails(
                orderId: order.Id,
                shippingActualCost: ahamoveResponse.TotalFee,
                ahamoveOrderId: ahamoveResponse._id
            );

            _logger.LogInformation($"Successfully created Ahamove order {ahamoveResponse._id} for Order ID: {request.OrderId}");

            return new CreateShippingOrderResponseDto
            {
                AhamoveOrderId = ahamoveResponse._id,
                Status = ahamoveResponse.Status,
                ShippingFeeActualCost = ahamoveResponse.TotalFee,
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

