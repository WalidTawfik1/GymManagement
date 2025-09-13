using System;
using System.Collections.Generic;

namespace Gym.Core.DTO
{
    public class DashboardDTO
    {
        public int TotalActiveMembers { get; set; }
        public int TotalVisitsThisMonth { get; set; }
        public int MembershipsEndingSoon { get; set; }
        public decimal NetProfitThisMonth { get; set; }
        public decimal NetProfitLastMonth { get; set; }
        public decimal TotalRevenueThisMonth { get; set; }
        public decimal TotalExpensesThisMonth { get; set; }
        public int NewMembersThisMonth { get; set; }
        public int TotalTrainees { get; set; }
        public MembershipDistributionDTO MembershipDistribution { get; set; } = new();
        public List<MembershipDTO> UpcomingExpirations { get; set; } = new();
        public MonthlyComparisonDTO MonthlyComparison { get; set; } = new();
    }

    public class MembershipDistributionDTO
    {
        public int SingleSessionMemberships { get; set; }
        public int OneMonthMemberships { get; set; }
        public int ThreeMonthMemberships { get; set; }
        public int TwelveSessionMemberships { get; set; }
    }

    public class MonthlyComparisonDTO
    {
        public decimal RevenueGrowthPercentage { get; set; }
        public decimal ProfitGrowthPercentage { get; set; }
        public decimal MemberGrowthPercentage { get; set; }
        public decimal VisitGrowthPercentage { get; set; }
    }
}
