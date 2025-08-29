using AutoMapper;
using Gym.Core.DTO;
using Gym.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.UI.Mapping
{
    public class VisitMapping: Profile
    {
        public VisitMapping()
        {
            CreateMap<Visit, VisitDTO>()
                .ForMember(dest => dest.TraineeName, opt => opt.MapFrom(src => src.Trainee != null ? src.Trainee.FullName : string.Empty))
                .ReverseMap();
        }
    }
}
