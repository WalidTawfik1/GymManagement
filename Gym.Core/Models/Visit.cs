using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Core.Models
{
    public class Visit : BaseEntity
    {
        public int TraineeId { get; set; }
        public Trainee Trainee { get; set; }

        public DateTime VisitDate { get; set; } = DateTime.Now;
    }
}
