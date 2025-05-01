using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using PowerPulse.Modules.Report.Models;

namespace PowerPulse.Modules.Report.Services;

public class ExcelService
{
    public byte[] GenerateExcelReport(
        int year,
        KeyMetricsDto keyMetrics,
        List<TimeSeriesDto> costPerKwh,
        List<TimeSeriesDto> totalCost,
        List<TimeSeriesDto> avgConsumption)
    {
        using var package = new ExcelPackage();

        // 1. Лист KeyMetrics
        CreateKeyMetricsSheet(package, keyMetrics, year);

        // 2. Лист CostPerKwh
        CreateCostPerKwhSheet(package, costPerKwh, year);

        // 3. Лист TotalCost
        CreateTotalCostSheet(package, totalCost, year);

        // 4. Лист AvgConsumptions
        CreateAvgConsumptionSheet(package, avgConsumption, year);

        // 5. Лист Charts
        CreateChartsSheet(package, year, costPerKwh.Count, totalCost.Count, avgConsumption.Count);

        return package.GetAsByteArray();
    }

    private void CreateKeyMetricsSheet(ExcelPackage package, KeyMetricsDto metrics, int year)
    {
        var ws = package.Workbook.Worksheets.Add("KeyMetrics");

        ws.Cells["A1"].Value = $"Ключевые показатели за {year}";
        ws.Cells["A1:D1"].Merge = true;
        ws.Cells["A1"].Style.Font.Bold = true;
        ws.Cells["A1"].Style.Font.Size = 14;

        // Заголовки
        ws.Cells["A3:D3"].LoadFromArrays(new List<object[]> {
            new object[] {
                "Общее потребление (кВт*ч)",
                "Общие затраты",
                "Среднее потребление (кВт*ч)",
                "Средняя стоимость за кВт*ч"
            }
        });

        var headerRange = ws.Cells["A3:D3"];
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
        headerRange.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

        // Данные
        ws.Cells["A4"].Value = metrics.TotalConsumption;
        ws.Cells["B4"].Value = metrics.TotalCost;
        ws.Cells["C4"].Value = metrics.AvgConsumption;
        ws.Cells["D4"].Value = metrics.AvgCostPerKwh;

        ws.Column(1).Width = 25;
        ws.Column(2).Width = 20;
        ws.Column(3).Width = 25;
        ws.Column(4).Width = 25;
    }

    private void CreateCostPerKwhSheet(ExcelPackage package, List<TimeSeriesDto> data, int year)
    {
        var ws = package.Workbook.Worksheets.Add("CostPerKwh");
        ws.Cells["A1"].Value = $"Стоимость за кВт*ч за {year}";
        ws.Cells["A1:D1"].Merge = true;
        ws.Cells["A1"].Style.Font.Bold = true;
        ws.Cells["A1"].Style.Font.Size = 14;

        ws.Cells["A3"].Value = "Дата";
        ws.Cells["B3"].Value = "Стоимость (за кВт*ч)";

        for (int i = 0; i < data.Count; i++)
        {
            ws.Cells[i + 4, 1].Value = data[i].Date;
            ws.Cells[i + 4, 1].Style.Numberformat.Format = "dd.MM.yyyy";
            ws.Cells[i + 4, 2].Value = data[i].Value;
        }

        var headerRange = ws.Cells["A3:B3"];
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
        headerRange.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

        ws.Column(1).Width = 15;
        ws.Column(2).Width = 20;
    }

    private void CreateTotalCostSheet(ExcelPackage package, List<TimeSeriesDto> data, int year)
    {
        var ws = package.Workbook.Worksheets.Add("TotalCost");
        ws.Cells["A1"].Value = $"Общие затраты за {year}";
        ws.Cells["A1:D1"].Merge = true;
        ws.Cells["A1"].Style.Font.Bold = true;
        ws.Cells["A1"].Style.Font.Size = 14;

        ws.Cells["A3"].Value = "Дата";
        ws.Cells["B3"].Value = "Затраты";

        for (int i = 0; i < data.Count; i++)
        {
            ws.Cells[i + 4, 1].Value = data[i].Date;
            ws.Cells[i + 4, 1].Style.Numberformat.Format = "dd.MM.yyyy";
            ws.Cells[i + 4, 2].Value = data[i].Value;
        }
        
        var headerRange = ws.Cells["A3:B3"];
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
        headerRange.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

        ws.Column(1).Width = 15;
        ws.Column(2).Width = 20;
    }

    private void CreateAvgConsumptionSheet(ExcelPackage package, List<TimeSeriesDto> data, int year)
    {
        var ws = package.Workbook.Worksheets.Add("AvgConsumptions");
        ws.Cells["A1"].Value = $"Среднее потребление за {year}";
        ws.Cells["A1:D1"].Merge = true;
        ws.Cells["A1"].Style.Font.Bold = true;
        ws.Cells["A1"].Style.Font.Size = 14;

        ws.Cells["A3"].Value = "Дата";
        ws.Cells["B3"].Value = "Потребление (кВт*ч)";

        for (int i = 0; i < data.Count; i++)
        {
            ws.Cells[i + 4, 1].Value = data[i].Date;
            ws.Cells[i + 4, 1].Style.Numberformat.Format = "dd.MM.yyyy";
            ws.Cells[i + 4, 2].Value = data[i].Value;
        }
        
        var headerRange = ws.Cells["A3:B3"];
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
        headerRange.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

        ws.Column(1).Width = 15;
        ws.Column(2).Width = 20;
    }

    private void CreateChartsSheet(
        ExcelPackage package,
        int year,
        int costRowCount,
        int totalRowCount,
        int avgRowCount)
    {
        var chartSheet = package.Workbook.Worksheets.Add("Charts");
        chartSheet.View.ShowGridLines = false;

        // График 1: Стоимость за кВт*ч
        CreateLineChart(
            chartSheet,
            "CostPerKwhChart",
            $"Стоимость за кВт*ч за {year}",
            package.Workbook.Worksheets["CostPerKwh"],
            4, 1, 2, costRowCount, 1, 1);

        // График 2: Общие затраты
        CreateLineChart(
            chartSheet,
            "TotalCostChart",
            $"Общие затраты за {year}",
            package.Workbook.Worksheets["TotalCost"],
            4, 1, 2, totalRowCount, 1, 14);

        // График 3: Среднее потребление
        CreateLineChart(
            chartSheet,
            "AvgConsumptionChart",
            $"Среднее потребление за {year}",
            package.Workbook.Worksheets["AvgConsumptions"],
            4, 1, 2, avgRowCount, 21, 1);
    }

    private void CreateLineChart(
        ExcelWorksheet chartSheet,
        string chartName,
        string title,
        ExcelWorksheet sourceSheet,
        int startRow,
        int dateCol,
        int valueCol,
        int rowCount,
        int chartRow,
        int chartCol)
    {
        var chart = chartSheet.Drawings.AddChart(chartName, eChartType.XYScatterLines);
        chart.Title.Text = title;
        chart.SetSize(800, 400);
        chart.SetPosition(chartRow, 0, chartCol, 0); // row, col, offsetRow, offsetCol

        // Ось X - даты
        var xRange = sourceSheet.Cells[startRow, dateCol, startRow + rowCount - 1, dateCol];
        // Ось Y - значения
        var yRange = sourceSheet.Cells[startRow, valueCol, startRow + rowCount - 1, valueCol];

        var series = chart.Series.Add(yRange, xRange);

        chart.Legend.Remove();
    }
}