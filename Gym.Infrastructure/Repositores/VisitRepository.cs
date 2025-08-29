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
    public class VisitRepository : GenericRepository<Visit>, IVisitRepository
    {
        private readonly MambelaDbContext _context;
        private readonly IMapper _mapper;

        public VisitRepository(MambelaDbContext context, IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<VisitResponseDTO?> AddVisitAsync(int traineeId)
        {
            // Use transaction to ensure data consistency
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (await IsTraineeCheckedInAsync(traineeId))
                {
                    return new VisitResponseDTO
                    {
                        TraineeName = "",
                        MembershipType = "",
                        RemainingSessions = null,
                        IsActive = false,
                        VisitDate = DateTime.Now,
                        Message = "المتدرب قام بتسجيل الدخول اليوم بالفعل"
                    };
                }

                // Check if trainee has active membership before creating visit
                if (!await HasActiveMembershipAsync(traineeId))
                {
                    return new VisitResponseDTO
                    {
                        TraineeName = "",
                        MembershipType = "",
                        RemainingSessions = null,
                        IsActive = false,
                        VisitDate = DateTime.Now,
                        Message = "لا يوجد اشتراك نشط"
                    };
                }

                var visit = new Visit
                {
                    TraineeId = traineeId,
                    VisitDate = DateTime.Now
                };
                
                await _context.Visits.AddAsync(visit);

                // Get membership info before updating sessions
                var membership = await _context.Memberships
                    .Include(m => m.Trainee)
                    .Where(m => m.TraineeId == traineeId && 
                               m.EndDate >= DateOnly.FromDateTime(DateTime.Now) && 
                               m.IsActive && 
                               !m.IsDeleted)
                    .OrderBy(m => m.EndDate)
                    .FirstOrDefaultAsync();

                if (membership == null)
                {
                    await transaction.RollbackAsync();
                    return new VisitResponseDTO
                    {
                        TraineeName = "",
                        MembershipType = "",
                        RemainingSessions = null,
                        IsActive = false,
                        VisitDate = DateTime.Now,
                        Message = "لم يتم العثور على اشتراك نشط"
                    };
                }

                // Decrement sessions directly on the membership entity we already have
                if ((membership.MembershipType == "محدود" || membership.MembershipType == "Limit") && membership.RemainingSessions.HasValue)
                {
                    membership.RemainingSessions -= 1;
                    if (membership.RemainingSessions <= 0)
                    {
                        membership.IsActive = false; // Deactivate membership if no sessions left
                    }
                    membership.UpdatedAt = DateTime.Now;
                    _context.Memberships.Update(membership);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                
                return new VisitResponseDTO
                {
                    TraineeName = membership.Trainee?.FullName ?? "",
                    MembershipType = membership.MembershipType,
                    RemainingSessions = membership.RemainingSessions,
                    IsActive = membership.IsActive,
                    VisitDate = visit.VisitDate,
                    Message = "تم تسجيل الزيارة بنجاح"
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new VisitResponseDTO
                {
                    TraineeName = "",
                    MembershipType = "",
                    RemainingSessions = null,
                    IsActive = false,
                    VisitDate = DateTime.Now,
                    Message = $"خطأ في تسجيل الزيارة: {ex.Message}"
                };
            }
        }

        public async Task<bool> DecrementRemainingSessionsAsync(int traineeId, bool saveChanges = true)
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

            if ((membership.MembershipType == "محدود" || membership.MembershipType == "Limit")  && membership.RemainingSessions.HasValue)
            {
                membership.RemainingSessions -= 1;
                if (membership.RemainingSessions <= 0)
                {
                    membership.IsActive = false; // Deactivate membership if no sessions left
                }
                membership.UpdatedAt = DateTime.Now;
                _context.Memberships.Update(membership);
                
                if (saveChanges)
                {
                    await _context.SaveChangesAsync(); // Save changes to database only if requested
                }
                return true;
            }
            else if (membership.MembershipType == "مفتوح" || membership.MembershipType == "Open")
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

        public async Task<IReadOnlyList<VisitDTO>> GetAllVisitsAsync()
        {
            var visits = await _context.Visits
                .Include(v => v.Trainee)
                .Where(v => !v.IsDeleted)
                .OrderByDescending(v => v.VisitDate)
                .ToListAsync();
            return _mapper.Map<IReadOnlyList<VisitDTO>>(visits);
        }

        public async Task<IReadOnlyList<VisitResponseDTO>> GetAllVisitsWithResponseAsync()
        {
            var visits = await _context.Visits
                .Include(v => v.Trainee)
                .ThenInclude(t => t.Memberships.Where(m => m.IsActive && !m.IsDeleted))
                .Where(v => !v.IsDeleted)
                .OrderByDescending(v => v.VisitDate)
                .ToListAsync();

            var visitResponses = new List<VisitResponseDTO>();

            foreach (var visit in visits)
            {
                var activeMembership = visit.Trainee?.Memberships
                    .Where(m => m.IsActive && !m.IsDeleted && m.EndDate >= DateOnly.FromDateTime(DateTime.Now))
                    .OrderBy(m => m.EndDate)
                    .FirstOrDefault();

                var response = new VisitResponseDTO
                {
                    TraineeName = visit.Trainee?.FullName ?? "غير محدد",
                    MembershipType = activeMembership?.MembershipType ?? "لا يوجد اشتراك",
                    RemainingSessions = activeMembership?.RemainingSessions,
                    IsActive = activeMembership?.IsActive ?? false,
                    VisitDate = visit.VisitDate,
                    Message = activeMembership != null ? "زيارة مسجلة" : "لا يوجد اشتراك نشط"
                };

                visitResponses.Add(response);
            }

            return visitResponses;
        }

        public async Task<IReadOnlyList<VisitDTO>> GetTodayVisits(DateOnly today)
        {
            var visits = await _context.Visits
                .Include(v => v.Trainee)
                .Where(v => DateOnly.FromDateTime(v.VisitDate) == today && !v.IsDeleted)
                .OrderByDescending(v => v.VisitDate)
                .ToListAsync();
            return _mapper.Map<IReadOnlyList<VisitDTO>>(visits);

        }
    }
}
