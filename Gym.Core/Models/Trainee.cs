using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Core.Models
{
    public class Trainee : BaseEntity
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public DateOnly JoinDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        public ICollection<Membership> Memberships { get; set; }
        public ICollection<Visit> Visits { get; set; }
    }
}
