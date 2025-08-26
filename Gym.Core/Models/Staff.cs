using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Core.Models
{
    public class Staff : BaseEntity
    {
        public string FullName { get; set; }
        public string Role { get; set; }

        public ICollection<StaffAttendance> Attendances { get; set; }

    }
}
