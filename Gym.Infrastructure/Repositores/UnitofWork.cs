using AutoMapper;
using Gym.Core.Interfaces;
using Gym.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Infrastructure.Repositores
{
    public class UnitofWork : IUnitofWork
    {
        private readonly MambelaDbContext _context;
        private readonly IMapper _mapper;


        public ITraineeRepository TraineeRepository { get; }
        public IMembershipRepository MembershipRepository { get; }
        public IVisitRepository VisitRepository { get; }


        public UnitofWork(MambelaDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;



            TraineeRepository = new TraineeRepository(_mapper, _context);
            MembershipRepository = new MembershipRepository(_context, _mapper);
            VisitRepository = new VisitRepository(_context);
        }


    }
}
