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

        public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public DateOnly EndDate { get; set; } = DateOnly.FromDateTime(DateTime.Now.AddMonths(1));

        public int? RemainingSessions { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
