using System;
using System.Collections.Generic;
using System.Linq;
using congestion.calculator.Contracts;

namespace congestion.calculator.Services;

public class DateValidator : IDateValidator
{
    public bool AllInSameDay(IEnumerable<DateTime> times)
    {
        var list = times.ToList();
        if (!list.Any()) return true;

        var firstDate = list.First().Date;
        return list.All(t => t.Date == firstDate);
    }
}