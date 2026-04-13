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

    public MenegerViewModel(dbBaza context)
    {
        _context = context;
        _ = RefreshAsync();
    }

    partial void OnSelectedGroupingChanged(StatsGrouping value) => _ = RefreshAsync();
    partial void OnStartDateChanged(DateTimeOffset? value) => _ = RefreshAsync();
    partial void OnEndDateChanged(DateTimeOffset? value) => _ = RefreshAsync();

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
}

