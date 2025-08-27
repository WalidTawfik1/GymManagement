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
    public class MembershipMapping:Profile
    {
        public MembershipMapping()
        {
            CreateMap<Membership,AddMembershipDTO>().ReverseMap();
            CreateMap<Membership, MembershipDTO>().ReverseMap();
        }
    }
}
