using System;
using System.Globalization;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.SystemConfigs;
using FitBridge_Domain.Entities.Systems;
using FitBridge_Domain.Enums.SystemConfigs;
using FitBridge_Domain.Exceptions;

namespace FitBridge_Application.Services;

public class SystemConfigurationService(IUnitOfWork _unitOfWork)
{
    public async Task<T> GetSystemConfigurationValueAsync<T>(string key)
    {
        var systemConfiguration = await _unitOfWork.Repository<SystemConfiguration>().GetBySpecificationAsync(new GetSystemConfigurationByKeySpecification(key));
        if (systemConfiguration == null)
        {
            throw new NotFoundException(nameof(SystemConfiguration), key);
        }
        return (T)Convert.ChangeType(systemConfiguration.Value, typeof(T));
    }

    public async Task<SystemConfiguration> GetSystemConfigurationEntityAsync(string key)
    {
        var systemConfiguration = await _unitOfWork.Repository<SystemConfiguration>().GetBySpecificationAsync(new GetSystemConfigurationByKeySpecification(key));
        if (systemConfiguration == null)
        {
            throw new NotFoundException(nameof(SystemConfiguration), key);
        }
        return systemConfiguration;
    }

    public async Task<object> GetSystemConfigurationAutoConvertDataTypeAsync(string key)
    {
        var systemConfiguration = await GetSystemConfigurationEntityAsync(key);
        switch (systemConfiguration.DataType)
        {
            case SystemConfigurationDataType.String:
                return systemConfiguration.Value;
            case SystemConfigurationDataType.Int:
                return int.Parse(systemConfiguration.Value, CultureInfo.InvariantCulture);
            case SystemConfigurationDataType.Decimal:
                return decimal.Parse(systemConfiguration.Value, CultureInfo.InvariantCulture);
            case SystemConfigurationDataType.Double:
                return double.Parse(systemConfiguration.Value, CultureInfo.InvariantCulture);
            default:
                throw new NotFoundException("System configuration data type not found");
        }
    }
}
