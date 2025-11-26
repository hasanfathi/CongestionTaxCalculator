using System;
using System.Collections.Generic;
using System.Linq;
using congestion.calculator.Configurations;
using congestion.calculator.Contracts;
using congestion.calculator.DataProvider.Contracts;

namespace congestion.calculator.Services;

public class CongestionTaxCalculator : ICongestionTaxCalculator
{
    private readonly ICongestionTaxDataProvider _data;
    private readonly IDateValidator _dateValidator;

    public CongestionTaxCalculator(ICongestionTaxDataProvider data, IDateValidator dateValidator)
    {
        _data = data;
        _dateValidator = dateValidator;
    }

    public int CalculateTax(VehicleType vehicleType, IEnumerable<DateTime> passages)
    {
        try
        {
            int total = 0;
            foreach (var group in passages.GroupBy(p => p.Date).ToList())
            {
                var dailyPassages = group.ToList();

                var dailyTotal = CalculateDailyTax(vehicleType, dailyPassages);

                total += dailyTotal;
            }

            return total;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public int CalculateDailyTax(VehicleType vehicleType, IEnumerable<DateTime> passages)
    {
        // To validate daily passages
        var isAllInSameDay = _dateValidator.AllInSameDay(passages);
        if (!isAllInSameDay)
            throw new ArgumentException("Multiple days are not allowed");

        if (_data.ExemptVehicles.Contains(vehicleType))
            return 0;

        if (!passages.Any())
            return 0;

        if (IsTaxFreeDate(passages.First()))
            return 0;

        // To Calculate
        var orderedPassages = passages.OrderBy(p => p).ToList();

        // To manage 60-minutes periods
        var windows = new List<FeeWindow>();

        FeeWindow currentWindow = null;

        foreach (var passage in orderedPassages)
        {
            var fee = GetFee(passage.TimeOfDay);

            if (currentWindow == null)
            {
                currentWindow = new FeeWindow(passage, fee);
                windows.Add(currentWindow);
                continue;
            }

            // To check if this passage is within 60-minute window from the start
            var minutes = (passage - currentWindow.Start).TotalMinutes;

            if (minutes <= 60)
            {
                // Same window and update max fee
                currentWindow.AddFee(fee);
            }
            else
            {
                // New window
                currentWindow = new FeeWindow(passage, fee);
                windows.Add(currentWindow);
            }
        }

        // To sum max fees
        int total = windows.Sum(w => w.MaxFee);

        // To apply daily limitation
        return Math.Min(total, 60);
    }

    // Private methods to internal use
    private int GetFee(TimeSpan time)
    {
        foreach (var window in _data.TimeWindows)
        {
            if (window.Matches(time))
                return window.Amount;
        }

        return default;
    }

    private bool IsTaxFreeDate(DateTime date)
    {
        // Weekend?
        if (IsWeekend(date))
            return true;

        // FreeDay?
        if (IsTollFreeDay(date))
            return true;

        // Holiday?
        if (IsPublicHoliday(date))
            return true;

        // DayBeforeHoliday?
        if (IsDaysBeforePublicHolidays(date))
            return true;

        return false;
    }

    private bool IsWeekend(DateTime date)
    {
        // Weekend?
        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            return true;

        return false;
    }

    private bool IsTollFreeDay(DateTime date)
    {
        // FreeDay?
        var tollFreeDays = _data.TollFreeDays;
        return tollFreeDays.Contains(date.Date);
    }

    private bool IsPublicHoliday(DateTime date)
    {
        // Holiday?
        var publicHolidays = _data.PublicHolidays;
        return publicHolidays.Contains(date.Date);
    }

    private bool IsDaysBeforePublicHolidays(DateTime date)
    {
        // DayBeforeHoliday?
        var daysBeforePublicHolidays = _data.DaysBeforePublicHolidays;
        return daysBeforePublicHolidays.Contains(date.Date);
    }
}