using AutoMapper;
using Gym.Core.DTO;
using Gym.Core.Interfaces;
using Gym.Core.Models;
using Gym.Infrastructure.Data;
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
            if (membershipDTO.MembershipType == "محدود")
            {
                var membership = new Membership
                {
                    TraineeId = membershipDTO.TraineeId,
                    MembershipType = membershipDTO.MembershipType,
                    RemainingSessions = 11,
                    IsActive = true
                };
                await _context.Memberships.AddAsync(membership);
            }
            else
            {
                var membership = new Membership
                {
                    TraineeId = membershipDTO.TraineeId,
                    MembershipType = membershipDTO.MembershipType,
                    IsActive = true
                };
                await _context.Memberships.AddAsync(membership);
            }
            await _context.SaveChangesAsync();

            return true;
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
