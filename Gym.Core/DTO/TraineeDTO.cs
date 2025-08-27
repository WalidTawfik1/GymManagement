using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Core.DTO
{
    public record TraineeDTO
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
    }

    public record UpdateTraineeDTO: TraineeDTO
    {
        public int Id { get; set; }
    }
}
