using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Coupons.GetAllStartingCoupons;
using FitBridge_Domain.Entities.Orders;
using Quartz;

namespace FitBridge_Infrastructure.Jobs.Coupons
{
    internal class EnableStartingCouponsJob(IUnitOfWork unitOfWork) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var spec = new GetAllStartingCouponsSpec();
            var coupons = await unitOfWork.Repository<Coupon>().GetAllWithSpecificationAsync(spec, asNoTracking: false);

            foreach (var coupon in coupons)
            {
                coupon.IsActive = false;
                unitOfWork.Repository<Coupon>().Update(coupon);
            }

            await unitOfWork.CommitAsync();
        }
    }
}