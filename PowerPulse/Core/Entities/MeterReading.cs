namespace PowerPulse.Core.Entities;

public class MeterReading
{
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public DateTime Date { get; set; } // Месяц и год
    public double Reading { get; set; } // Показания (кВт·ч)
    public decimal Cost { get; set; } // Стоимость
}