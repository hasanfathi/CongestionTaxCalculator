using System;
using System.Collections.Generic;
using congestion.calculator.Configurations;

namespace congestion.calculator.DataProvider.Contracts
{
    public interface ICongestionTaxDataProvider
    {
        IReadOnlyList<TollTimeWindow> TimeWindows { get; }
        ISet<VehicleType> ExemptVehicles { get; }
        ISet<DateTime> TollFreeDays { get; }
        ISet<DateTime> PublicHolidays { get; }
        ISet<DateTime> DaysBeforePublicHolidays { get; }
    }
}