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
    public class TraineeRepository : GenericRepository<Trainee>, ITraineeRepository
    {
        private readonly MambelaDbContext context;
        private readonly IMapper mapper;
        public TraineeRepository(IMapper mapper,MambelaDbContext context) : base(context)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<bool> AddTraineeAsync(TraineeDTO trainee)
        {
            var entity = new Trainee
            {
                FullName = trainee.FullName,
                PhoneNumber = trainee.PhoneNumber
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
    }
}
