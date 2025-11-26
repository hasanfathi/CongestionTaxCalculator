using System;

namespace congestion.calculator.Configurations;

public class FeeWindow
{
    public DateTime Start { get; }
    public int MaxFee { get; private set; }

    public FeeWindow(DateTime start, int initialFee)
    {
        Start = start;
        MaxFee = initialFee;
    }

    public void AddFee(int fee)
    {
        if (fee > MaxFee)
            MaxFee = fee;
    }
}