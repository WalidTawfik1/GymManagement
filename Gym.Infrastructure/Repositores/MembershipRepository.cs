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

            if (membershipDTO.MembershipType == "12 حصة" || membershipDTO.MembershipType == "12 Sessions")
            {
                var membership = new Membership
                {
                    TraineeId = membershipDTO.TraineeId,
                    MembershipType = membershipDTO.MembershipType,
                    Price = membershipDTO.Price,
                    StartDate = DateOnly.FromDateTime(DateTime.Now),
                    EndDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(1)),
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
                    EndDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(3)),
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
                    EndDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(1)),
                    RemainingSessions = null,
                    IsActive = true
                };
                await _context.Memberships.AddAsync(membership);
            }
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IReadOnlyList<MembershipDTO>> GetAllMembershipsAsync()
        {
            var memberships = await _context.Memberships
                .Include(m => m.Trainee)
                .Where(m => !m.IsDeleted)
                .ToListAsync();
            var membershipsDTO = _mapper.Map<IReadOnlyList<MembershipDTO>>(memberships);
            return membershipsDTO;
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
    }
}
