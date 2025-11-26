using System;
using System.Collections.Generic;

namespace congestion.calculator.Contracts;

public interface IDateValidator
{
    bool AllInSameDay(IEnumerable<DateTime> times);
}