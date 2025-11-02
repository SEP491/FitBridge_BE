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
            CreateMap<Transaction, GetAllTransactionAdminDto>()
                .ForMember(x => x.TransactionId, opt => opt.MapFrom(y => y.Id))
                .ForMember(x => x.PaymentMethod, opt => opt.MapFrom(y => y.PaymentMethod.MethodType))
                .ForMember(x => x.CustomerName, opt => opt.MapFrom(y => y.OrderId == null ? null : y.Order.Account.FullName))
                .ForMember(x => x.CustomerAvatarUrl, opt => opt.MapFrom(y => y.OrderId == null ? null : y.Order.Account.AvatarUrl))
                .ForMember(x => x.OrderId, opt => opt.MapFrom(y => y.OrderId))
                .ForMember(x => x.WalletId, opt => opt.MapFrom(y => y.WalletId))
                .ForMember(x => x.WithdrawalRequestId, opt => opt.MapFrom(y => y.WithdrawalRequestId));
        }
    }
}