using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FitBridge_Infrastructure.Persistence
{
    internal class UtcToLocalDateTimeConverter : ValueConverter<DateTime, DateTime>
    {
        public UtcToLocalDateTimeConverter() : base(
            v => v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc).ToLocalTime())
        { }
    }
}