using Gym.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Core.Interfaces
{
    public interface IExpenseAndRevenueRepository
    {
        Task<bool> AddExpenseAsync(AddExpenseDTO expense);
        Task<IEnumerable<ExpenseDTO>> GetAllExpensesByMonthAsync(int month);
        Task<bool> UpdateExpenseAsync(ExpenseDTO expense);
        Task<bool> DeleteExpenseAsync(int id);
        Task<decimal> GetTotalExpensesByMonthAsync(int month);
        Task<decimal> GetTotalRevenueByMonthAsync(int month);
        Task<IEnumerable<ExpenseDTO>> GetExpensesByMonthAsync(int month, int year);
    }
}
