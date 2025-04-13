namespace PowerPulse.Modules.EnergyConsumption.Models;

public class MeterReadingModel
{
    public DateTime Date { get; set; }
    public double Reading { get; set; }
    public decimal Cost { get; set; }
}