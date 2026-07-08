using AutoMapper;
using Gym.Core.DTO;
using Gym.Core.Interfaces;
using Gym.Core.Models;
using Gym.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Infrastructure.Repositores
{
    public class MembershipRepository : GenericRepository<Membership>, IMembershipRepository
    {
        private readonly MambelaDbContext _context;
        private readonly IMapper _mapper;
        public MembershipRepository(MambelaDbContext context, IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> AddMembership(AddMembershipDTO membershipDTO)
        {
            var hasActiveMembership = await GetMembershipByTraineeIdAsync(membershipDTO.TraineeId);

            if (hasActiveMembership != null) return false;

            if (membershipDTO.MembershipType == "حصة واحدة" || membershipDTO.MembershipType == "Single Session")
            {
                var membership = new Membership
                {
                    TraineeId = membershipDTO.TraineeId,
                    MembershipType = membershipDTO.MembershipType,
                    Price = membershipDTO.Price,
                    StartDate = DateOnly.FromDateTime(DateTime.Now),
                    EndDate = DateOnly.FromDateTime(DateTime.Now),
                    RemainingSessions = null,
                    IsActive = false
                };
                await _context.Memberships.AddAsync(membership);
            }
            else if (membershipDTO.MembershipType == "8 حصص" || membershipDTO.MembershipType == "8 Sessions")
            {
                var membership = new Membership
                {
                    TraineeId = membershipDTO.TraineeId,
                    MembershipType = membershipDTO.MembershipType,
                    Price = membershipDTO.Price,
                    StartDate = DateOnly.FromDateTime(DateTime.Now),
                    EndDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(1).AddDays(-1)),
                    RemainingSessions = 8,
                    IsActive = true
                };
                await _context.Memberships.AddAsync(membership);
            }
            else if (membershipDTO.MembershipType == "12 حصة" || membershipDTO.MembershipType == "12 Sessions")
            {
                var membership = new Membership
                {
                    TraineeId = membershipDTO.TraineeId,
                    MembershipType = membershipDTO.MembershipType,
                    Price = membershipDTO.Price,
                    StartDate = DateOnly.FromDateTime(DateTime.Now),
                    EndDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(1).AddDays(-1)),
                    RemainingSessions = 12,
                    IsActive = true
                };
                await _context.Memberships.AddAsync(membership);
            }
            else if(membershipDTO.MembershipType == "3 شهور" || membershipDTO.MembershipType == "3 Months")
            {
                var membership = new Membership
                {
                    TraineeId = membershipDTO.TraineeId,
                    MembershipType = membershipDTO.MembershipType,
                    Price = membershipDTO.Price,
                    StartDate = DateOnly.FromDateTime(DateTime.Now),
                    EndDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(3).AddDays(-1)),
                    RemainingSessions = null,
                    IsActive = true
                };
                await _context.Memberships.AddAsync(membership);
            }
            else if(membershipDTO.MembershipType == "شهر" || membershipDTO.MembershipType == "1 Month")
            {
                var membership = new Membership
                {
                    TraineeId = membershipDTO.TraineeId,
                    MembershipType = membershipDTO.MembershipType,
                    Price = membershipDTO.Price,
                    StartDate = DateOnly.FromDateTime(DateTime.Now),
                    EndDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(1).AddDays(-1)),
                    RemainingSessions = null,
                    IsActive = true
                };
                await _context.Memberships.AddAsync(membership);
            }
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task DeactivateExpiredMembershipsAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            await _context.Memberships
                .Where(m => m.IsActive && !m.IsDeleted && (m.RemainingSessions <= 0 || m.EndDate < today))
                .ExecuteUpdateAsync(s => s.SetProperty(m => m.IsActive, false));
        }

        public async Task<IReadOnlyList<MembershipDTO>> GetAllMembershipsAsync(int? month = null, int? year = null)
        {
            await DeactivateExpiredMembershipsAsync();
            var targetMonth = month ?? Gym.Core.Helpers.AccountingDateHelper.GetCurrentAccountingMonth();
            var targetYear = year ?? Gym.Core.Helpers.AccountingDateHelper.GetCurrentAccountingYear();
            var period = Gym.Core.Helpers.AccountingDateHelper.GetAccountingPeriodDateOnly(targetMonth, targetYear);
            
            var memberships = await Task.Run(() => _context.Memberships
                .Where(m => !m.IsDeleted && m.StartDate >= period.StartDate && m.StartDate < period.EndDate)
                .Include(m => m.Trainee)
                .OrderByDescending(m => m.StartDate)
                .ToList());
            
            return _mapper.Map<IReadOnlyList<MembershipDTO>>(memberships);
        }

        public async Task<PagedResult<MembershipDTO>> GetMembershipsPagedAsync(int pageNumber, int pageSize, int? month = null, int? year = null, string searchQuery = "", string sortOrder = "Newest")
        {
            await DeactivateExpiredMembershipsAsync();
            var targetMonth = month ?? Gym.Core.Helpers.AccountingDateHelper.GetCurrentAccountingMonth();
            var targetYear = year ?? Gym.Core.Helpers.AccountingDateHelper.GetCurrentAccountingYear();
            var period = Gym.Core.Helpers.AccountingDateHelper.GetAccountingPeriodDateOnly(targetMonth, targetYear);
            
            var query = _context.Memberships
                .Where(m => !m.IsDeleted && m.StartDate >= period.StartDate && m.StartDate < period.EndDate)
                .Include(m => m.Trainee)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(m => (m.Trainee != null && m.Trainee.FullName.Contains(searchQuery)) ||
                                         m.MembershipType.Contains(searchQuery));
            }

            if (sortOrder == "Newest")
            {
                query = query.OrderByDescending(m => m.StartDate).ThenByDescending(m => m.Id);
            }
            else
            {
                query = query.OrderBy(m => m.StartDate).ThenBy(m => m.Id);
            }

            var totalCount = await query.CountAsync();
            var memberships = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            var dtos = _mapper.Map<IReadOnlyList<MembershipDTO>>(memberships);

            return new PagedResult<MembershipDTO>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<(int TotalCount, int ActiveCount, decimal TotalRevenue)> GetMembershipsSummaryAsync(int month, int year)
        {
            var period = Gym.Core.Helpers.AccountingDateHelper.GetAccountingPeriodDateOnly(month, year);
            
            var query = _context.Memberships
                .Where(m => !m.IsDeleted && m.StartDate >= period.StartDate && m.StartDate < period.EndDate);

            var totalCount = await query.CountAsync();
            var activeCount = await query.CountAsync(m => m.IsActive);
            var totalRevenue = await query.SumAsync(m => m.Price);

            return (totalCount, activeCount, totalRevenue);
        }

        public async Task<MembershipDTO?> GetMembershipByTraineeIdAsync(int id)
        {
            var membership = await _context.Memberships
                .Include(m => m.Trainee)
                .Where(m => !m.IsDeleted && m.IsActive && m.TraineeId == id)
                .FirstOrDefaultAsync();
            var membershipDTO = _mapper.Map<MembershipDTO>(membership);
            return membershipDTO;
        }

        public async Task<bool> UpdateMembership(MembershipDTO membershipDTO)
        {
            var membership = await _context.Memberships.FindAsync(membershipDTO.Id);
            if (membership == null) return false;
            _mapper.Map(membershipDTO, membership);
            membership.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IReadOnlyList<MembershipDTO>> GetMembershipsByMonthAsync(int month, int year)
        {
            var period = Gym.Core.Helpers.AccountingDateHelper.GetAccountingPeriodDateOnly(month, year);
            var memberships = await _context.Memberships
                .Include(m => m.Trainee)
                .Where(m => !m.IsDeleted && 
                           m.StartDate >= period.StartDate && 
                           m.StartDate < period.EndDate)
                .ToListAsync();
            var membershipsDTO = _mapper.Map<IReadOnlyList<MembershipDTO>>(memberships);
            return membershipsDTO;
        }

        public async Task<IReadOnlyList<MembershipDTO>> GetNearFinishMemberships()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var threeDaysFromNow = DateOnly.FromDateTime(DateTime.Now.AddDays(3));

            var memberships = await _context.Memberships
                .Include(m => m.Trainee)
                .Where(m =>
                    !m.IsDeleted &&
                    m.IsActive &&
                    (
                        // Ends within 1–3 days
                        (m.EndDate > today && m.EndDate <= threeDaysFromNow) ||

                        // Has 3 or fewer sessions remaining and not yet expired
                        (m.RemainingSessions != null &&
                         m.RemainingSessions <= 3 &&
                         m.EndDate > today)
                    )
                )
                .ToListAsync();

            var membershipsDTO = _mapper.Map<IReadOnlyList<MembershipDTO>>(memberships);
            return membershipsDTO;
        }

    }
}
