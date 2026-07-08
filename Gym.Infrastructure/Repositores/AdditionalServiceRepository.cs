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
            var additionalService = new AdditionalService
            {
                TraineeId = additionalServiceDTO.TraineeId,
                ServiceType = additionalServiceDTO.ServiceType,
                DurationInMinutes = additionalServiceDTO.DurationInMinutes,
                Price = additionalServiceDTO.Price,
                TakenAt = DateTime.Now,
            };
            await _context.AdditionalServices.AddAsync(additionalService);

            return true;
        }

        public async Task<bool> DeleteAdditionalServiceAsync(int id)
        {
            var additionalService = await _context.AdditionalServices.FindAsync(id);
            if (additionalService == null) return false;
            additionalService.IsDeleted = true;

            return true;
        }

        public async Task<IReadOnlyList<AdditionalServiceDTO>> GetAllAdditionalServicesAsync(int? month = null, int? year = null)
        {
            var targetMonth = month ?? Gym.Core.Helpers.AccountingDateHelper.GetCurrentAccountingMonth();
            var targetYear = year ?? Gym.Core.Helpers.AccountingDateHelper.GetCurrentAccountingYear();
            var period = Gym.Core.Helpers.AccountingDateHelper.GetAccountingPeriod(targetMonth, targetYear);
            
            var additionalServices = await Task.Run(() => _context.AdditionalServices
                .Where(a => !a.IsDeleted && a.TakenAt >= period.StartDate && a.TakenAt < period.EndDate)
                .Include(a => a.Trainee)
                .OrderByDescending(a => a.TakenAt)
                .ToList());
            var additionalServicesDTOs = _mapper.Map<IReadOnlyList<AdditionalServiceDTO>>(additionalServices);
            return additionalServicesDTOs;
        }

        public async Task<PagedResult<AdditionalServiceDTO>> GetAdditionalServicesPagedAsync(int pageNumber, int pageSize, int? month = null, int? year = null, string searchQuery = "")
        {
            var targetMonth = month ?? Gym.Core.Helpers.AccountingDateHelper.GetCurrentAccountingMonth();
            var targetYear = year ?? Gym.Core.Helpers.AccountingDateHelper.GetCurrentAccountingYear();
            var period = Gym.Core.Helpers.AccountingDateHelper.GetAccountingPeriod(targetMonth, targetYear);
            
            var query = _context.AdditionalServices
                .Where(a => !a.IsDeleted && a.TakenAt >= period.StartDate && a.TakenAt < period.EndDate)
                .Include(a => a.Trainee)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(a => (a.Trainee != null && a.Trainee.FullName.Contains(searchQuery)) ||
                                         a.ServiceType.Contains(searchQuery));
            }

            query = query.OrderByDescending(a => a.TakenAt).ThenByDescending(a => a.Id);

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            var dtos = _mapper.Map<IReadOnlyList<AdditionalServiceDTO>>(items);

            return new PagedResult<AdditionalServiceDTO>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
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



        public async Task<bool> UpdateAdditionalServiceAsync(AdditionalServiceDTO additionalServiceDTO)
        {
            var additionalService = await _context.AdditionalServices.FindAsync(additionalServiceDTO.Id);
            if (additionalService == null) return false;
            var lastDate = additionalService.TakenAt;
            _mapper.Map(additionalServiceDTO, additionalService);
            additionalService.TakenAt = lastDate;
            additionalService.UpdatedAt = DateTime.Now;

            return true;
        }

        public async Task<IReadOnlyList<AdditionalServiceDTO>> GetAdditionalServicesByMonthAsync(int month, int year)
        {
            var period = Gym.Core.Helpers.AccountingDateHelper.GetAccountingPeriod(month, year);
            var services = await _context.AdditionalServices
                .Include(a => a.Trainee)
                .Where(a => !a.IsDeleted &&
                           a.TakenAt >= period.StartDate &&
                           a.TakenAt < period.EndDate)
                .ToListAsync();
            var servicesDTO = _mapper.Map<IReadOnlyList<AdditionalServiceDTO>>(services);
            return servicesDTO;
        }
    }
}
