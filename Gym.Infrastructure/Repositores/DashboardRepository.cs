using AutoMapper;
using Gym.Core.DTO;
using Gym.Core.Interfaces;
using Gym.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gym.Infrastructure.Repositores
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly MambelaDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMembershipRepository _membershipRepository;
        private readonly IVisitRepository _visitRepository;
        private readonly IExpenseAndRevenueRepository _expenseRevenueRepository;

        public DashboardRepository(
            MambelaDbContext context, 
            IMapper mapper,
            IMembershipRepository membershipRepository,
            IVisitRepository visitRepository,
            IExpenseAndRevenueRepository expenseRevenueRepository)
        {
            _context = context;
            _mapper = mapper;
            _membershipRepository = membershipRepository;
            _visitRepository = visitRepository;
            _expenseRevenueRepository = expenseRevenueRepository;
        }

        public async Task<DashboardDTO> GetDashboardDataAsync()
        {
            await _membershipRepository.DeactivateExpiredMembershipsAsync();
            var dashboard = new DashboardDTO
            {
                TotalActiveMembers = await GetTotalActiveMembersAsync(),
                TotalVisitsThisMonth = await GetTotalVisitsThisMonthAsync(),
                MembershipsEndingSoon = await GetMembershipsEndingSoonAsync(),
                NetProfitThisMonth = await GetNetProfitThisMonthAsync(),
                NetProfitLastMonth = await GetNetProfitLastMonthAsync(),
                MembershipDistribution = await GetMembershipDistributionAsync(),
                MonthlyComparison = await GetMonthlyComparisonAsync(),
                UpcomingExpirations = (await _membershipRepository.GetNearFinishMemberships()).ToList(),
                TotalRevenueThisMonth = await _expenseRevenueRepository.GetTotalRevenueByMonthAsync(Gym.Core.Helpers.AccountingDateHelper.GetCurrentAccountingMonth(), Gym.Core.Helpers.AccountingDateHelper.GetCurrentAccountingYear()),
                TotalExpensesThisMonth = await _expenseRevenueRepository.GetTotalExpensesByMonthAsync(Gym.Core.Helpers.AccountingDateHelper.GetCurrentAccountingMonth(), Gym.Core.Helpers.AccountingDateHelper.GetCurrentAccountingYear()),
                NewMembersThisMonth = await GetNewMembersThisMonthAsync(),
                TotalTrainees = await GetTotalTraineesAsync()
            };

            return dashboard;
        }

        public async Task<int> GetTotalActiveMembersAsync()
        {
            return await _context.Memberships
                .Include(m => m.Trainee)
                .CountAsync(m => m.IsActive && !m.IsDeleted && !m.Trainee.IsDeleted);
        }

        public async Task<int> GetTotalVisitsThisMonthAsync()
        {
            var currentMonth = Gym.Core.Helpers.AccountingDateHelper.GetCurrentAccountingMonth();
            var currentYear = Gym.Core.Helpers.AccountingDateHelper.GetCurrentAccountingYear();
            return await _visitRepository.GetVisitsCountByMonthAsync(currentMonth, currentYear);
        }

        public async Task<int> GetMembershipsEndingSoonAsync()
        {
            var nearFinishMemberships = await _membershipRepository.GetNearFinishMemberships();
            return nearFinishMemberships.Count;
        }

        public async Task<decimal> GetNetProfitThisMonthAsync()
        {
            var currentMonth = Gym.Core.Helpers.AccountingDateHelper.GetCurrentAccountingMonth();
            var currentYear = Gym.Core.Helpers.AccountingDateHelper.GetCurrentAccountingYear();
            var revenue = await _expenseRevenueRepository.GetTotalRevenueByMonthAsync(currentMonth, currentYear);
            var expenses = await _expenseRevenueRepository.GetTotalExpensesByMonthAsync(currentMonth, currentYear);
            return revenue - expenses;
        }

        public async Task<decimal> GetNetProfitLastMonthAsync()
        {
            var currentAccountingMonth = Gym.Core.Helpers.AccountingDateHelper.GetCurrentAccountingMonth();
            var currentAccountingYear = Gym.Core.Helpers.AccountingDateHelper.GetCurrentAccountingYear();
            var lastMonth = currentAccountingMonth == 1 ? 12 : currentAccountingMonth - 1;
            var lastMonthYear = currentAccountingMonth == 1 ? currentAccountingYear - 1 : currentAccountingYear;
            var revenue = await _expenseRevenueRepository.GetTotalRevenueByMonthAsync(lastMonth, lastMonthYear);
            var expenses = await _expenseRevenueRepository.GetTotalExpensesByMonthAsync(lastMonth, lastMonthYear);
            return revenue - expenses;
        }

        public async Task<MembershipDistributionDTO> GetMembershipDistributionAsync()
        {
            var activeMemberships = await _context.Memberships
                .Include(m => m.Trainee)
                .Where(m => !m.IsDeleted && !m.Trainee.IsDeleted)
                .ToListAsync();

            return new MembershipDistributionDTO
            {
                SingleSessionMemberships = activeMemberships.Count(m => 
                    m.MembershipType.Contains("حصة واحدة") || m.MembershipType.Contains("Single Session")),
                OneMonthMemberships = activeMemberships.Count(m => 
                    (m.MembershipType.Contains("شهر") || m.MembershipType.Contains("Month")) &&
                    !m.MembershipType.Contains("3")),
                ThreeMonthMemberships = activeMemberships.Count(m => 
                    m.MembershipType.Contains("3 شهور") || m.MembershipType.Contains("3 Months")),
                TwelveSessionMemberships = activeMemberships.Count(m => 
                    m.MembershipType.Contains("12 حصة") || m.MembershipType.Contains("12 Sessions"))
            };
        }

        public async Task<MonthlyComparisonDTO> GetMonthlyComparisonAsync()
        {
            var currentMonth = Gym.Core.Helpers.AccountingDateHelper.GetCurrentAccountingMonth();
            var currentYear = Gym.Core.Helpers.AccountingDateHelper.GetCurrentAccountingYear();
            var lastMonth = currentMonth == 1 ? 12 : currentMonth - 1;
            var lastMonthYear = currentMonth == 1 ? currentYear - 1 : currentYear;

            // Revenue comparison
            var currentRevenue = await _expenseRevenueRepository.GetTotalRevenueByMonthAsync(currentMonth, currentYear);
            var lastRevenue = await _expenseRevenueRepository.GetTotalRevenueByMonthAsync(lastMonth, lastMonthYear);

            // Profit comparison
            var currentProfit = await GetNetProfitThisMonthAsync();
            var lastProfit = await GetNetProfitLastMonthAsync();

            // Member comparison
            var currentMembers = await GetNewMembersThisMonthAsync();
            var lastMonthMembers = await GetNewMembersByMonthAsync(lastMonth, lastMonthYear);

            // Visit comparison
            var currentVisits = await _visitRepository.GetVisitsCountByMonthAsync(currentMonth, currentYear);
            var lastVisits = await _visitRepository.GetVisitsCountByMonthAsync(lastMonth, lastMonthYear);

            return new MonthlyComparisonDTO
            {
                RevenueGrowthPercentage = CalculateGrowthPercentage(currentRevenue, lastRevenue),
                ProfitGrowthPercentage = CalculateGrowthPercentage(currentProfit, lastProfit),
                MemberGrowthPercentage = CalculateGrowthPercentage(currentMembers, lastMonthMembers),
                VisitGrowthPercentage = CalculateGrowthPercentage(currentVisits, lastVisits)
            };
        }

        private async Task<int> GetNewMembersThisMonthAsync()
        {
            var currentMonth = Gym.Core.Helpers.AccountingDateHelper.GetCurrentAccountingMonth();
            var currentYear = Gym.Core.Helpers.AccountingDateHelper.GetCurrentAccountingYear();
            return await GetNewMembersByMonthAsync(currentMonth, currentYear);
        }

        private async Task<int> GetNewMembersByMonthAsync(int month, int year)
        {
            var memberships = await _membershipRepository.GetMembershipsByMonthAsync(month, year);
            return memberships.Count;
        }

        private async Task<int> GetTotalTraineesAsync()
        {
            return await _context.Trainees.CountAsync(t => !t.IsDeleted);
        }

        private static decimal CalculateGrowthPercentage(decimal current, decimal previous)
        {
            if (previous == 0) return current > 0 ? 100 : 0;
            return Math.Round(((current - previous) / previous) * 100, 2);
        }
    }
}
