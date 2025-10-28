using AutoMapper;
using FitBridge_Application.Dtos.Transactions;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Transactions.GetTransactionDetail;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.Transactions.GetTransactionDetail
{
    internal class GetTransactionDetailQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper) : IRequestHandler<GetTransactionDetailQuery, GetTransactionDetailDto>
    {
        public async Task<GetTransactionDetailDto> Handle(GetTransactionDetailQuery request, CancellationToken cancellationToken)
        {
            var spec = new GetTransactionDetailSpecification(request.TransactionId);
            var transaction = await unitOfWork.Repository<Transaction>()
                .GetBySpecificationProjectedAsync<GetTransactionDetailDto>(spec, mapper.ConfigurationProvider)
                ?? throw new NotFoundException(nameof(Transaction), request.TransactionId);

            return transaction;
        }
    }
}