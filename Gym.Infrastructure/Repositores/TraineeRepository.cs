using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Gym.Core.DTO;
using Gym.Core.Interfaces;
using Gym.Core.Models;
using Gym.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Gym.Infrastructure.Repositores
{
    public class TraineeRepository : GenericRepository<Trainee>, ITraineeRepository
    {
        private readonly MambelaDbContext context;
        private readonly IMapper mapper;
        public TraineeRepository(IMapper mapper, MambelaDbContext context) : base(context)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<bool> AddTraineeAsync(TraineeDTO trainee)
        {
            var entity = new Trainee
            {
                FullName = trainee.FullName,
                PhoneNumber = trainee.PhoneNumber,
                IdCardPhotoPath = trainee.IdCardPhotoPath
            };
            await context.Trainees.AddAsync(entity);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<IReadOnlyList<Trainee>> GetAllTraineesAsync()
        {
            return await context.Trainees
                .Where(t => !t.IsDeleted)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Trainee>> GetTraineeByNameAsync(string name)
        {
            return await context.Trainees
                .Where(t => t.FullName.Contains(name) && !t.IsDeleted)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> UpdateTraineeAsync(UpdateTraineeDTO trainee)
        {
            var entity = await context.Trainees.FindAsync(trainee.Id);
            if (entity == null) return false;
            mapper.Map(trainee, entity);
            entity.UpdatedAt = DateTime.Now;
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<PagedResult<Trainee>> GetTraineesPagedAsync(int pageNumber, int pageSize, string searchQuery = "", string sortOrder = "Newest")
        {
            var query = context.Trainees.Where(t => !t.IsDeleted).AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(t => t.FullName.Contains(searchQuery) || t.PhoneNumber.Contains(searchQuery));
            }

            if (sortOrder == "Newest")
            {
                query = query.OrderByDescending(t => t.Id);
            }
            else
            {
                query = query.OrderBy(t => t.Id);
            }

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedResult<Trainee>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
