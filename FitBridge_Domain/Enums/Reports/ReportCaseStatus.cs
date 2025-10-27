namespace FitBridge_Domain.Enums.Reports;

[Flags]
public enum ReportCaseStatus
{
    Pending = 0,

    Processing = 1,

    Resolved = 2,

    FraudConfirmed = 3
}