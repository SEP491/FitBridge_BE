using System;
using Quartz;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Exceptions;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace FitBridge_Infrastructure.Jobs.Orders;

public class UpdatePTCurrentCourseJob(IUnitOfWork _unitOfWork, IApplicationUserService _applicationUserService, ILogger<UpdatePTCurrentCourseJob> _logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var orderItemId = Guid.Parse(context.JobDetail.JobDataMap.GetString("orderItemId"));
        var orderItem = await _unitOfWork.Repository<OrderItem>().GetByIdAsync(orderItemId, includes: new List<string> { "FreelancePTPackage" });
        if (orderItem == null)
        {
            _logger.LogError("Order item not found");
            return;
        }
        if (orderItem.GymPtId != null)
        {
            var pt = await _applicationUserService.GetByIdAsync(orderItem.GymPtId.Value, null, true);
            if (pt == null)
            {
                _logger.LogError("PT not found");
                return;
            }
            pt.PtCurrentCourse--;
        }
        else if (orderItem.FreelancePTPackageId != null)
        {
            var freelancePt = await _applicationUserService.GetByIdAsync(orderItem.FreelancePTPackage.PtId, null, true);
            if (freelancePt == null)
            {
                _logger.LogError("Freelance PT not found");
                return;
            }
            if(freelancePt.PtCurrentCourse > 0)
            {
                freelancePt.PtCurrentCourse--;
            }
        }
        await _unitOfWork.CommitAsync();
    }
}
