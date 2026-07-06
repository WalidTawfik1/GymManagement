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
        Task<IEnumerable<ExpenseDTO>> GetAllExpensesByMonthAsync(int month, int year);
        Task<bool> UpdateExpenseAsync(ExpenseDTO expense);
        Task<bool> DeleteExpenseAsync(int id);
        Task<decimal> GetTotalExpensesByMonthAsync(int month, int year);
        Task<decimal> GetTotalRevenueByMonthAsync(int month, int year);
        Task<IEnumerable<ExpenseDTO>> GetExpensesByMonthAsync(int month, int year);
    }
}
