namespace PowerPulse.Modules.Report.Models;

public class KeyMetricsDto
{
    public double TotalConsumption { get; set; }
    public decimal TotalCost { get; set; }
    public double AvgConsumption { get; set; }
    public decimal AvgCostPerKwh { get; set; }
}