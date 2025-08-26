using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Core.Models
{
    public class Membership : BaseEntity
    {
        public int TraineeId { get; set; }
        public Trainee Trainee { get; set; }

        public string MembershipType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int? RemainingSessions { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
