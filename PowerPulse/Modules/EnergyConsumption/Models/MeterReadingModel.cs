using System.ComponentModel.DataAnnotations;

namespace PowerPulse.Modules.EnergyConsumption.Models;

public class MeterReadingModel
{
    public Guid Id { get; set; }
    [Required(ErrorMessage = "Поле Дата является обязательным")]
    public DateTime Date { get; set; }
    [Required(ErrorMessage = "Поле Показания является обязательным")]
    [Range(0, double.MaxValue)]
    public double Reading { get; set; }
    [Required(ErrorMessage = "Поле Стоимость является обязательным")]
    [Range(0, double.MaxValue)]
    public decimal? Cost { get; set; }
}

public class NullableMeterReadingModel
{
    public Guid Id { get; set; }
    [Required(ErrorMessage = "Поле Дата является обязательным")]
    public DateTime Date { get; set; }
    [Required(ErrorMessage = "Поле Показания является обязательным")]
    [Range(0, double.MaxValue)]
    public double Reading { get; set; }
    
    public decimal? Cost { get; set; }
}