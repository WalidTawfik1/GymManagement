using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Core.DTO
{
    public record AddMembershipDTO
    {
        public int TraineeId { get; set; }
        public string MembershipType { get; set; }
        public decimal Price { get; set; }
    }

    public record MembershipDTO
    {
        public int Id { get; set; }
        public int TraineeId { get; set; }
        public string TraineeName { get; set; }
        public string MembershipType { get; set; }
        public decimal Price { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int? RemainingSessions { get; set; }
        public bool IsActive { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
