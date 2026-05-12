using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Proba_Sklada.Hardik.Connector;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Proba_Sklada.Reports;

namespace Inventori_Manager.ViewModels;

public enum StatsGrouping
{
    Day,
    Week,
    Month
}

public partial class MenegerViewModel : ObservableObject
{
    private readonly dbBaza _context;

    public ObservableCollection<StatsGrouping> GroupingOptions { get; } =
        new(new[] { StatsGrouping.Day, StatsGrouping.Week, StatsGrouping.Month });

    [ObservableProperty]
    private StatsGrouping selectedGrouping = StatsGrouping.Day;

    [ObservableProperty]
    private DateTimeOffset? startDate = DateTimeOffset.Now.Date.AddDays(-30);

    [ObservableProperty]
    private DateTimeOffset? endDate = DateTimeOffset.Now.Date;

    [ObservableProperty]
    private ISeries[] movementSeries = Array.Empty<ISeries>();

    [ObservableProperty]
    private Axis[] xAxes = Array.Empty<Axis>();

    [ObservableProperty]
    private Axis[] yAxes = Array.Empty<Axis>();

    public ObservableCollection<ReportOperationFilter> ReportOperationOptions { get; } =
        new(new[]
        {
            ReportOperationFilter.All,
            ReportOperationFilter.Incoming,
            ReportOperationFilter.Outgoing,
            ReportOperationFilter.InternalMovement
        });

    [ObservableProperty]
    private ReportOperationFilter selectedReportOperation = ReportOperationFilter.All;

    [ObservableProperty]
    private ObservableCollection<ReportRow> reportRows = new();

    [ObservableProperty]
    private bool isReportBusy;

    public bool IsReportNotBusy => !IsReportBusy;

    public MenegerViewModel(dbBaza context)
    {
        _context = context;
        _ = RefreshAsync();
    }

    partial void OnSelectedGroupingChanged(StatsGrouping value) => _ = RefreshAsync();
    partial void OnStartDateChanged(DateTimeOffset? value) => _ = RefreshAsync();
    partial void OnEndDateChanged(DateTimeOffset? value) => _ = RefreshAsync();
    partial void OnSelectedReportOperationChanged(ReportOperationFilter value) => _ = BuildReportAsync();
    partial void OnIsReportBusyChanged(bool value) => OnPropertyChanged(nameof(IsReportNotBusy));

    public async Task RefreshAsync()
    {
        var startDt = (StartDate ?? DateTimeOffset.Now.Date).Date;
        var endDt = (EndDate ?? DateTimeOffset.Now.Date).Date;

        var start = DateOnly.FromDateTime(startDt);
        var end = DateOnly.FromDateTime(endDt);
        if (end < start)
        {
            (start, end) = (end, start);
        }

        var incomingRaw = await _context.postuplenia_items
            .AsNoTracking()
            .Where(x => x.invoice.invoice_date >= start && x.invoice.invoice_date <= end)
            .Select(x => new RawRow(x.invoice.invoice_date, (double)x.quantity))
            .ToListAsync();

        var outgoingRaw = await _context.schet_faktura_soderzanies
            .AsNoTracking()
            .Where(x => x.invoice.invoice_date >= start && x.invoice.invoice_date <= end)
            .Select(x => new RawRow(x.invoice.invoice_date, (double)x.quantity))
            .ToListAsync();

        var incomingAgg = Aggregate(incomingRaw, SelectedGrouping);
        var outgoingAgg = Aggregate(outgoingRaw, SelectedGrouping);

        var keys = incomingAgg.Keys
            .Union(outgoingAgg.Keys)
            .OrderBy(k => k)
            .ToList();

        var labels = keys.Select(k => k.Label).ToArray();
        var incoming = keys.Select(k => incomingAgg.TryGetValue(k, out var v) ? v : 0d).ToArray();
        var outgoing = keys.Select(k => outgoingAgg.TryGetValue(k, out var v) ? v : 0d).ToArray();

        MovementSeries = new ISeries[]
        {
            new ColumnSeries<double>
            {
                Name = "Привезено",
                Values = incoming
            },
            new ColumnSeries<double>
            {
                Name = "Отгружено",
                Values = outgoing
            }
        };

        XAxes = new[]
        {
            new Axis
            {
                Labels = labels,
                LabelsRotation = 0,
                SeparatorsPaint = null
            }
        };

        YAxes = new[]
        {
            new Axis
            {
                Name = "Кол-во (шт.)"
            }
        };
    }

    private sealed record PeriodKey(DateTime SortKey, string Label);
    private sealed record RawRow(DateOnly Date, double Qty);

    private static Dictionary<PeriodKey, double> Aggregate(
        IEnumerable<RawRow> rows,
        StatsGrouping grouping)
    {
        var dict = new Dictionary<PeriodKey, double>();

        foreach (var r in rows)
        {
            var dt = r.Date.ToDateTime(TimeOnly.MinValue);

            PeriodKey key = grouping switch
            {
                StatsGrouping.Day => new PeriodKey(dt.Date, dt.ToString("dd.MM", CultureInfo.InvariantCulture)),
                StatsGrouping.Month => new PeriodKey(new DateTime(dt.Year, dt.Month, 1), dt.ToString("MM.yyyy", CultureInfo.InvariantCulture)),
                _ => BuildWeekKey(dt)
            };

            dict[key] = dict.TryGetValue(key, out var cur) ? cur + r.Qty : r.Qty;
        }

        return dict;
    }

    private static PeriodKey BuildWeekKey(DateTime dt)
    {
        // ISO week label, week starts on Monday.
        int week = ISOWeek.GetWeekOfYear(dt);
        int year = ISOWeek.GetYear(dt);

        var weekStart = ISOWeek.ToDateTime(year, week, DayOfWeek.Monday).Date;
        var label = $"{weekStart:dd.MM}-{weekStart.AddDays(6):dd.MM}";
        return new PeriodKey(weekStart, label);
    }

    [RelayCommand]
    public async Task BuildReportAsync()
    {
        if (IsReportBusy) return;
        IsReportBusy = true;
        try
        {
            var (start, end) = GetReportPeriod();

            var incomingTask = SelectedReportOperation is ReportOperationFilter.All or ReportOperationFilter.Incoming
                ? LoadIncomingAsync(start, end)
                : Task.FromResult(new List<ReportRow>());

            var outgoingTask = SelectedReportOperation is ReportOperationFilter.All or ReportOperationFilter.Outgoing
                ? LoadOutgoingAsync(start, end)
                : Task.FromResult(new List<ReportRow>());

            var internalTask = SelectedReportOperation is ReportOperationFilter.All or ReportOperationFilter.InternalMovement
                ? LoadInternalAsync(start, end)
                : Task.FromResult(new List<ReportRow>());

            await Task.WhenAll(incomingTask, outgoingTask, internalTask);

            var rows = incomingTask.Result
                .Concat(outgoingTask.Result)
                .Concat(internalTask.Result)
                .OrderBy(r => r.DateTime)
                .ToList();

            ReportRows = new ObservableCollection<ReportRow>(rows);
        }
        finally
        {
            IsReportBusy = false;
        }
    }

    public async Task ExportReportPdfAsync(string filePath)
    {
        if (IsReportBusy) return;
        IsReportBusy = true;
        try
        {
            if (ReportRows.Count == 0)
                await BuildReportAsync();

            var (start, end) = GetReportPeriod();
            OperationsReportPdf.SavePdf(filePath, ReportRows.ToList(), start, end, SelectedReportOperation);
        }
        finally
        {
            IsReportBusy = false;
        }
    }

    private (DateOnly start, DateOnly end) GetReportPeriod()
    {
        var startDt = (StartDate ?? DateTimeOffset.Now.Date).Date;
        var endDt = (EndDate ?? DateTimeOffset.Now.Date).Date;
        var start = DateOnly.FromDateTime(startDt);
        var end = DateOnly.FromDateTime(endDt);
        if (end < start) (start, end) = (end, start);
        return (start, end);
    }

    private async Task<List<ReportRow>> LoadIncomingAsync(DateOnly start, DateOnly end)
    {
        var items = await _context.postuplenia_items
            .AsNoTracking()
            .Include(x => x.invoice).ThenInclude(i => i.supplier)
            .Include(x => x.product)
            .Include(x => x.location)
            .Where(x => x.invoice.invoice_date >= start && x.invoice.invoice_date <= end)
            .ToListAsync();

        return items.Select(x => new ReportRow(
                DateTime: x.invoice.invoice_date.ToDateTime(TimeOnly.MinValue),
                Operation: "Приход",
                DocumentNumber: x.invoice.invoice_number,
                Counterparty: x.invoice.supplier.name,
                Product: x.product.name,
                Quantity: x.quantity,
                UnitPrice: x.unit_price,
                TotalPrice: x.total_price ?? (x.unit_price * x.quantity),
                LocationFrom: "",
                LocationTo: x.location.location_code
            ))
            .ToList();
    }

    private async Task<List<ReportRow>> LoadOutgoingAsync(DateOnly start, DateOnly end)
    {
        var items = await _context.schet_faktura_soderzanies
            .AsNoTracking()
            .Include(x => x.invoice).ThenInclude(i => i.customer)
            .Include(x => x.product)
            .Where(x => x.invoice.invoice_date >= start && x.invoice.invoice_date <= end)
            .ToListAsync();

        return items.Select(x => new ReportRow(
                DateTime: x.invoice.invoice_date.ToDateTime(TimeOnly.MinValue),
                Operation: "Отгрузка",
                DocumentNumber: x.invoice.invoice_number,
                Counterparty: x.invoice.customer.name,
                Product: x.product.name,
                Quantity: x.quantity,
                UnitPrice: x.unit_price,
                TotalPrice: x.total_price ?? (x.unit_price * x.quantity),
                LocationFrom: "",
                LocationTo: ""
            ))
            .ToList();
    }

    private async Task<List<ReportRow>> LoadInternalAsync(DateOnly start, DateOnly end)
    {
        var startDt = start.ToDateTime(TimeOnly.MinValue);
        var endDt = end.ToDateTime(TimeOnly.MaxValue);

        var movements = await _context.vnutrinie_movements
            .AsNoTracking()
            .Include(x => x.product)
            .Include(x => x.from_location)
            .Include(x => x.to_location)
            .Where(x => x.movement_date != null && x.movement_date >= startDt && x.movement_date <= endDt)
            .ToListAsync();

        return movements.Select(x => new ReportRow(
                DateTime: x.movement_date ?? startDt,
                Operation: "Перемещение",
                DocumentNumber: x.movement_number,
                Counterparty: "",
                Product: x.product.name,
                Quantity: x.quantity,
                UnitPrice: 0m,
                TotalPrice: 0m,
                LocationFrom: x.from_location.location_code,
                LocationTo: x.to_location.location_code
            ))
            .ToList();
    }
}

