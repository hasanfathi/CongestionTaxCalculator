using System;

namespace congestion.calculator.Configurations;

public sealed record TollTimeWindow(TimeSpan Start, TimeSpan End, int Amount)
{
    // To match windows
    public bool Matches(TimeSpan time) =>
        time > Start && time <= End;
}