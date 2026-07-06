using System;

namespace Gym.Core.Helpers
{
    public static class AccountingDateHelper
    {
        public const int AccountingStartDay = 5;

        public static (DateTime StartDate, DateTime EndDate) GetAccountingPeriod(int month, int year)
        {
            var startDate = new DateTime(year, month, AccountingStartDay);
            var endDate = startDate.AddMonths(1);
            return (startDate, endDate);
        }

        public static (DateOnly StartDate, DateOnly EndDate) GetAccountingPeriodDateOnly(int month, int year)
        {
            var startDate = new DateOnly(year, month, AccountingStartDay);
            var endDate = startDate.AddMonths(1);
            return (startDate, endDate);
        }

        public static int GetCurrentAccountingMonth()
        {
            var now = DateTime.Now;
            if (now.Day < AccountingStartDay)
            {
                var prevMonth = now.AddMonths(-1);
                return prevMonth.Month;
            }
            return now.Month;
        }

        public static int GetCurrentAccountingYear()
        {
            var now = DateTime.Now;
            if (now.Day < AccountingStartDay)
            {
                var prevMonth = now.AddMonths(-1);
                return prevMonth.Year;
            }
            return now.Year;
        }
    }
}
