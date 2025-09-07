using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Core.Models
{
    public class AddiotnalServices : BaseEntity
    {
        public int TraineeId { get; set; }
        public Trainee Trainee { get; set; }
        public string ServiceType { get; set; }
        public decimal Price { get; set; }
        public int? DurationInMinutes { get; set; }
    }
}
