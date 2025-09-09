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
    public class AdditionalServiceRepository : IAdditionalServiceRepository
    {
        private readonly MambelaDbContext _context;
        private readonly IMapper _mapper;

        public AdditionalServiceRepository(MambelaDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> AddAdditionalServiceAsync(AddAdditionalServiceDTO additionalServiceDTO)
        {
            if (additionalServiceDTO.ServiceType == "مشاية") {

                var additionalService = new AdditionalService
                {
                    TraineeId = additionalServiceDTO.TraineeId,
                    ServiceType = additionalServiceDTO.ServiceType,
                    DurationInMinutes = additionalServiceDTO.DurationInMinutes,
                    Price = (decimal)(1.5 * (additionalServiceDTO.DurationInMinutes ?? 0)),
                    TakenAt = DateTime.Now,
                };
                await _context.AdditionalServices.AddAsync(additionalService);
            }
            else if (additionalServiceDTO.ServiceType == "ميزان") 
            {
                var additionalService = new AdditionalService
                {
                    TraineeId = additionalServiceDTO.TraineeId,
                    ServiceType = additionalServiceDTO.ServiceType,
                    DurationInMinutes = null,
                    Price = 5,
                    TakenAt = DateTime.Now,
                };
                await _context.AdditionalServices.AddAsync(additionalService);
            }
            else if (additionalServiceDTO.ServiceType == "InBody") 
            {
                var additionalService = new AdditionalService
                {
                    TraineeId = additionalServiceDTO.TraineeId,
                    ServiceType = additionalServiceDTO.ServiceType,
                    DurationInMinutes = null,
                    Price = 10,
                    TakenAt = DateTime.Now,
                };
                await _context.AdditionalServices.AddAsync(additionalService);
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAdditionalServiceAsync(int id)
        {
            var additionalService = await _context.AdditionalServices.FindAsync(id);
            if (additionalService == null) return false;
            additionalService.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IReadOnlyList<AdditionalServiceDTO>> GetAdditionalServiceByTraineeIdAsync(int traineeId)
        {
            var additionalServices = await _context.AdditionalServices
                .Where(a => a.TraineeId == traineeId && !a.IsDeleted)
                .OrderByDescending(a => a.TakenAt)
                .ToListAsync();
            var additionalServicesDTO = _mapper.Map<IReadOnlyList<AdditionalServiceDTO>>(additionalServices);
            return additionalServicesDTO;
        }

        public async Task<IReadOnlyList<AdditionalServiceDTO>> GetAllAdditionalServicesAsync()
        {
            var additionalServices = await _context.AdditionalServices
                .Where(a => !a.IsDeleted)
                .Include(a => a.Trainee)
                .ToListAsync();
            var additionalServicesDTO = _mapper.Map<IReadOnlyList<AdditionalServiceDTO>>(additionalServices);
            return additionalServicesDTO;
        }

        public async Task<bool> UpdateAdditionalServiceAsync(AdditionalServiceDTO additionalServiceDTO)
        {
            var additionalService = await _context.AdditionalServices.FindAsync(additionalServiceDTO.Id);
            if (additionalService == null) return false;
            var lastDate = additionalService.TakenAt;
            _mapper.Map(additionalServiceDTO, additionalService);
            additionalService.TakenAt = lastDate;
            additionalService.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
