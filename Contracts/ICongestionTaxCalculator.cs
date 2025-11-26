using System;
using System.Collections.Generic;
using congestion.calculator.Configurations;

namespace congestion.calculator.Contracts;

public interface ICongestionTaxCalculator
{
    int CalculateDailyTax(VehicleType vehicleType, IEnumerable<DateTime> passages);

    int CalculateTax(VehicleType vehicleType, IEnumerable<DateTime> passages);
}