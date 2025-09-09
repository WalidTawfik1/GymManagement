using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Core.DTO
{
    public record AddExpenseDTO
    {
        public string ExpenseType { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
    }

    public record ExpenseDTO
    {
        public int Id { get; set; }
        public string ExpenseType { get; set; }
        public decimal Amount { get; set; }
        public DateTime IncurredAt { get; set; }
        public string? Description { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
