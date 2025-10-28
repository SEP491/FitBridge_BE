using AutoMapper;
using FitBridge_Application.Dtos.Transactions;
using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Application.MappingProfiles
{
    public class TransactionMappingProfile : Profile
    {
        public TransactionMappingProfile()
        {
            CreateMap<Transaction, GetTransactionsDto>();
            CreateMap<Transaction, GetTransactionDetailDto>()
                .ForMember(x => x.PaymentMethod, opt => opt.MapFrom(y => y.PaymentMethod.MethodType));
            CreateMap<PaymentMethod, PaymentMethodDto>();
            CreateMap<WithdrawalRequest, WithdrawalRequestDto>();
        }
    }
}