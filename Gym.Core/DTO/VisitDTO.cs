using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Core.DTO
{
    public class VisitDTO
    {
        public int Id { get; set; }
        public int TraineeId { get; set; }
        public string TraineeName { get; set; }
        public DateTime VisitDate { get; set; }
    }
    public record VisitResponseDTO
    {
        public string TraineeName { get; set; }
        public string MembershipType { get; set; }
        public int? RemainingSessions { get; set; }
        public bool IsActive { get; set; }
        public DateTime VisitDate { get; set; }
        public string Message { get; set; }
    }
}
