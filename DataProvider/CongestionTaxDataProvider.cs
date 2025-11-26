using System;
using System.Collections.Generic;
using System.Linq;
using congestion.calculator.Configurations;
using congestion.calculator.DataProvider.Contracts;

namespace congestion.calculator.DataProvider;

public class CongestionTaxDataProvider : ICongestionTaxDataProvider
{
    public IReadOnlyList<TollTimeWindow> TimeWindows { get; }
    public ISet<VehicleType> ExemptVehicles { get; }
    public ISet<DateTime> TollFreeDays { get; }
    public ISet<DateTime> PublicHolidays { get; }
    public ISet<DateTime> DaysBeforePublicHolidays { get; }

    public CongestionTaxDataProvider()
    {
        // To set Calculator requirements
        TimeWindows = InitializeTimeWindows();
        ExemptVehicles = InitializeExemptVehicles();
        TollFreeDays = InitializeTollFreeDays();
        PublicHolidays = InitializePublicHolidays();
        DaysBeforePublicHolidays = InitializeDaysBeforePublicHolidays(PublicHolidays.ToHashSet());
    }

    private static IReadOnlyList<TollTimeWindow> InitializeTimeWindows() =>
        new List<TollTimeWindow>
        {
            new(TimeSpan.FromHours(6), TimeSpan.FromHours(6.5), 8), // 06:00–06:30
            new(TimeSpan.FromHours(6.5), TimeSpan.FromHours(7), 13), // 06:30–07:00
            new(TimeSpan.FromHours(7), TimeSpan.FromHours(8), 18), // 07:00–08:00
            new(TimeSpan.FromHours(8), TimeSpan.FromHours(8.5), 13), // 08:00–08:30
            new(TimeSpan.FromHours(8.5), TimeSpan.FromHours(15), 8), // 08:30–15:00
            new(TimeSpan.FromHours(15), TimeSpan.FromHours(15.5), 13), // 15:00–15:30
            new(TimeSpan.FromHours(15.5), TimeSpan.FromHours(17), 18), // 15:30–17:00
            new(TimeSpan.FromHours(17), TimeSpan.FromHours(18), 13), // 17:00–18:00
            new(TimeSpan.FromHours(18), TimeSpan.FromHours(18.5), 8), // 18:00–18:30
            new(TimeSpan.FromHours(18.5), TimeSpan.FromHours(24), 0), // 18:30–24:00
            new(TimeSpan.FromHours(0), TimeSpan.FromHours(6), 0), // 00:00–06:00
        };

    private static ISet<VehicleType> InitializeExemptVehicles() =>
        new HashSet<VehicleType>
        {
            VehicleType.Emergency,
            VehicleType.Bus,
            VehicleType.Diplomat,
            VehicleType.Motorcycle,
            VehicleType.Military,
            VehicleType.Foreign
        };

    private static ISet<DateTime> InitializeTollFreeDays()
    {
        return Generate().ToHashSet();

        IEnumerable<DateTime> Generate()
        {
            // Full July
            for (int day = 1; day <= 31; day++)
                yield return new DateTime(2013, 7, day);
        }
    }

    private static ISet<DateTime> InitializePublicHolidays()
    {
        var holidays = new HashSet<DateTime>
        {
            new(2013, 1, 1),
            new(2013, 3, 29),
            new(2013, 4, 1),
            new(2013, 5, 1),
            new(2013, 5, 9),
            new(2013, 6, 6),
            new(2013, 6, 21),
            new(2013, 6, 22),
            new(2013, 12, 24),
            new(2013, 12, 25),
            new(2013, 12, 26),
            new(2013, 12, 31)
        };

        return holidays;
    }

    private static ISet<DateTime> InitializeDaysBeforePublicHolidays(HashSet<DateTime> holidays)
    {
        return Generate().ToHashSet();

        IEnumerable<DateTime> Generate()
        {
            // Add days before
            foreach (var d in holidays)
                yield return d.AddDays(-1);
        }
    }
}