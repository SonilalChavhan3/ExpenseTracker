namespace ExpenseTracker.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IExpenseService
    {
        Task<IEnumerable<Expense>> GetAllAsync();
        Task<Expense?> GetByIdAsync(Guid id);
        Task<Expense> CreateAsync(Expense expense, int? categoryId = null);
        Task<bool> UpdateAsync(Expense expense, int? categoryId = null);
        Task<bool> DeleteAsync(Guid id);
    }
}
