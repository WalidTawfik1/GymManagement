using AutoMapper;
using Gym.Core.DTO;
using Gym.Core.Interfaces;
using Gym.Core.Models;
using Gym.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Infrastructure.Repositores
{
    public class ExpenseAndRevenueRepository : IExpenseAndRevenueRepository
    {
        private readonly MambelaDbContext _context;
        private readonly IMapper _mapper;

        public ExpenseAndRevenueRepository(MambelaDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> AddExpenseAsync(AddExpenseDTO expense)
        {
            var expenseEntity = new Expense
            {
                ExpenseType = expense.ExpenseType,
                Amount = expense.Amount,
                Description = expense.Description,
                IncurredAt = DateTime.Now,
            };
            _context.Expenses.Add(expenseEntity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteExpenseAsync(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null)
            {
                return false;
            }
            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ExpenseDTO>> GetAllExpensesByMonthAsync(int month)
        {
            if (month < 1 || month > 12)
            {
                throw new ArgumentOutOfRangeException("Month must be between 1 and 12.");
            }
            var currentYear = DateTime.Now.Year;
            var expenses = await Task.Run(() => _context.Expenses
                .Where(e => e.IncurredAt.Month == month && e.IncurredAt.Year == currentYear)
                .ToList());
            return _mapper.Map<IEnumerable<ExpenseDTO>>(expenses);
        }

        public async Task<decimal> GetTotalExpensesByMonthAsync(int month)
        {
            if (month < 1 || month > 12)
            {
                throw new ArgumentOutOfRangeException("Month must be between 1 and 12.");
            }
            var currentYear = DateTime.Now.Year;
            var total = await Task.Run(() => _context.Expenses
                .Where(e => e.IncurredAt.Month == month && e.IncurredAt.Year == currentYear && !e.IsDeleted)
                .Sum(e => e.Amount));
            return total;
        }

        public async Task<decimal> GetTotalRevenueByMonthAsync(int month)
        {
            if (month < 1 || month > 12)
            {
                throw new ArgumentOutOfRangeException("Month must be between 1 and 12.");
            }

            var currentYear = DateTime.Now.Year;

            // Calculate membership revenue for the month and year (memberships that started in this month of current year)
            var membershipRevenue = await _context.Memberships
                .Where(m => m.StartDate.Month == month && m.StartDate.Year == currentYear && !m.IsDeleted)
                .SumAsync(m => m.Price);

            // Calculate additional services revenue for the month and year
            var additionalServiceRevenue = await _context.AdditionalServices
                .Where(a => a.TakenAt.Month == month && a.TakenAt.Year == currentYear && !a.IsDeleted)
                .SumAsync(a => a.Price);

            // Total revenue = Membership revenue + Additional service revenue
            var totalRevenue = membershipRevenue + additionalServiceRevenue;

            return totalRevenue;
        }

        public async Task<bool> UpdateExpenseAsync(ExpenseDTO expense)
        {
            var existingExpense = await _context.Expenses.FindAsync(expense.Id);
            if (existingExpense == null)
            {
                return false;
            }
            _mapper.Map(expense, existingExpense);
            existingExpense.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
