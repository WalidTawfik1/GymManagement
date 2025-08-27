using AutoMapper;
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
    public class VisitRepository : GenericRepository<Visit>, IVisitRepository
    {
        private readonly MambelaDbContext _context;

        public VisitRepository(MambelaDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> AddVisitAsync(int traineeId)
        {
            // Use transaction to ensure data consistency
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (await IsTraineeCheckedInAsync(traineeId))
                {
                    return false; // Trainee has already checked in today
                }

                // Check if trainee has active membership before creating visit
                if (!await HasActiveMembershipAsync(traineeId))
                {
                    return false; // No active membership
                }

                var visit = new Visit
                {
                    TraineeId = traineeId,
                    VisitDate = DateTime.Now
                };
                
                await _context.Visits.AddAsync(visit);

                if (!await DecrementRemainingSessionsAsync(traineeId))
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> DecrementRemainingSessionsAsync(int traineeId)
        {
            var membership = await _context.Memberships
                .Where(m => m.TraineeId == traineeId && 
                           m.EndDate >= DateOnly.FromDateTime(DateTime.Now) && 
                           m.IsActive && 
                           !m.IsDeleted)
                .OrderBy(m => m.EndDate)
                .FirstOrDefaultAsync();

            if (membership == null)
            {
                return false; // No active membership found
            }

            if (membership.MembershipType == "محدود" && membership.RemainingSessions.HasValue)
            {
                membership.RemainingSessions -= 1;
                if (membership.RemainingSessions <= 0)
                {
                    membership.IsActive = false; // Deactivate membership if no sessions left
                }
                _context.Memberships.Update(membership);
                return true;
            }
            else if (membership.MembershipType == "مفتوح")
            {
                // For unlimited memberships, just check if it's still valid
                // If we reach here, the membership is active and valid (checked above)
                return true;
            }

            return false;
        }

        public async Task<int> GetTodayVisitsCountAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var visitsCount = await _context.Visits
                .CountAsync(v => DateOnly.FromDateTime(v.VisitDate) == today && !v.IsDeleted);

            return visitsCount;
        }

        public async Task<bool> IsTraineeCheckedInAsync(int traineeId)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var isCheckedIn = await _context.Visits
                .AnyAsync(v => v.TraineeId == traineeId && 
                              DateOnly.FromDateTime(v.VisitDate) == today && 
                              !v.IsDeleted);
            return isCheckedIn;
        }

        public async Task<bool> HasActiveMembershipAsync(int traineeId)
        {
            // First, deactivate any expired memberships for this trainee
            await DeactivateExpiredMembershipsForTraineeAsync(traineeId);

            var hasActiveMembership = await _context.Memberships
                .AnyAsync(m => m.TraineeId == traineeId &&
                              m.EndDate >= DateOnly.FromDateTime(DateTime.Now) &&
                              m.IsActive &&
                              !m.IsDeleted &&
                              (m.RemainingSessions == null || m.RemainingSessions > 0));
            return hasActiveMembership;
        }

        private async Task DeactivateExpiredMembershipsForTraineeAsync(int traineeId)
        {
            var expiredMemberships = await _context.Memberships
                .Where(m => m.TraineeId == traineeId &&
                           m.EndDate < DateOnly.FromDateTime(DateTime.Now) &&
                           m.IsActive &&
                           !m.IsDeleted)
                .ToListAsync();

            if (expiredMemberships.Any())
            {
                foreach (var membership in expiredMemberships)
                {
                    membership.IsActive = false;
                }
                _context.Memberships.UpdateRange(expiredMemberships);
                await _context.SaveChangesAsync();
            }
        }
    }
}
