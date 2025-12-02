using System;

namespace FitBridge_Application.Dtos.Orders;

public class SummaryProductOrderDto
{
    public int totalProductOrders { get; set; }
    public decimal totalProfit { get; set; }
    public decimal totalRevenue { get; set; }
    public int totalPending { get; set; }
    public int totalCompleted { get; set; }
    public int totalCancelled { get; set; }
    public int totalShipping { get; set; }
    public int totalArrived { get; set; }
    public int totalFinished { get; set; }
    public int totalInReturn { get; set; }
    public int totalReturned { get; set; }
    public int totalProcessing { get; set; }
    public int totalCreated { get; set; }
    public int totalAccepted { get; set; }
    public int totalCustomerNotReceived { get; set; }
    public int totalAssigning { get; set; }
}
