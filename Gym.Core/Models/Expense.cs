using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Core.Models
{
    public class Expense: BaseEntity
    {
        public string ExpenseType { get; set; }
        public decimal Amount { get; set; }
        public DateTime IncurredAt { get; set; } = DateTime.Now;
        public string? Description { get; set; }
    }
}
