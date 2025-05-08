namespace PowerPulse.Core.Entities;

public class MeterReading
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public DateTime Date { get; set; } // Месяц и год
    public DateTime CreateDate { get; set; }
    public double Reading { get; set; } // Показания (кВт·ч)
    public decimal? Cost { get; set; } // Стоимость
}