using AutoMapper;
using Gym.Core.DTO;
using Gym.Core.Models;

namespace Gym.Infrastructure.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Membership → MembershipDTO
            CreateMap<Membership, MembershipDTO>()
                .ForMember(dest => dest.TraineeName, opt => opt.MapFrom(src => src.Trainee != null ? src.Trainee.FullName : ""))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

            // MembershipDTO → Membership (for updates)
            CreateMap<MembershipDTO, Membership>()
                .ForMember(dest => dest.Trainee, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

            // Visit → VisitDTO
            CreateMap<Visit, VisitDTO>()
                .ForMember(dest => dest.TraineeName, opt => opt.MapFrom(src => src.Trainee != null ? src.Trainee.FullName : ""));

            // AdditionalService → AdditionalServiceDTO
            CreateMap<AdditionalService, AdditionalServiceDTO>()
                .ForMember(dest => dest.TraineeName, opt => opt.MapFrom(src => src.Trainee != null ? src.Trainee.FullName : ""));

            // AdditionalServiceDTO → AdditionalService (for updates)
            CreateMap<AdditionalServiceDTO, AdditionalService>()
                .ForMember(dest => dest.Trainee, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

            // Expense → ExpenseDTO
            CreateMap<Expense, ExpenseDTO>();

            // ExpenseDTO → Expense (for updates)
            CreateMap<ExpenseDTO, Expense>()
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

            // Trainee → TraineeDTO
            CreateMap<Trainee, TraineeDTO>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));

            // UpdateTraineeDTO → Trainee
            CreateMap<UpdateTraineeDTO, Trainee>()
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Memberships, opt => opt.Ignore())
                .ForMember(dest => dest.Visits, opt => opt.Ignore());
        }
    }
}
