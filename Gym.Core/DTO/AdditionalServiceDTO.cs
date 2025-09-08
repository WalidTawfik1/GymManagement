using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Core.DTO
{
    public record AddAdditionalServiceDTO
    {
        public int TraineeId { get; set; }
        public string ServiceType { get; set; }
        public int? DurationInMinutes { get; set; }
    }
    public record AdditionalServiceDTO
    {
        public int Id { get; set; }
        public int TraineeId { get; set; }
        public string TraineeName { get; set; }
        public string ServiceType { get; set; }
        public decimal Price { get; set; }
        public int? DurationInMinutes { get; set; }
        public DateTime TakenAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

}
