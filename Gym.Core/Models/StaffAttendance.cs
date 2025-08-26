using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Core.Models
{
    public class StaffAttendance : BaseEntity
    {
        public int StaffId { get; set; }
        public Staff Staff { get; set; }

        public DateTime CheckIn { get; set; } = DateTime.Now;
    }
}
