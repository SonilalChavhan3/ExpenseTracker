namespace ExpenseTracker.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public class ExpenseService : IExpenseService
    {
        private readonly ExpenseDbContext _db;
        private readonly Microsoft.Extensions.Logging.ILogger<ExpenseService> _logger;

        public ExpenseService(ExpenseDbContext db, Microsoft.Extensions.Logging.ILogger<ExpenseService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Expense> CreateAsync(Expense expense, int? categoryId = null)
        {
            try
            {
                if (categoryId.HasValue)
                {
                    var category = await _db.Categories.FindAsync(categoryId.Value);
                    if (category == null)
                    {
                        throw new ArgumentException("Category not found", nameof(categoryId));
                    }

                    // Use shadow FK property if Expense doesn't define CategoryId
                    _db.Entry(expense).Property("CategoryId").CurrentValue = categoryId.Value;
                }

                await _db.Expenses.AddAsync(expense);
                await _db.SaveChangesAsync();
                return expense;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error in CreateAsync for Expense");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var existing = await _db.Expenses.FindAsync(id);
                if (existing == null) return false;
                _db.Expenses.Remove(existing);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteAsync for Expense Id {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Expense>> GetAllAsync()
        {
            try
            {
                return await _db.Expenses.AsNoTracking().ToListAsync();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllAsync for Expenses");
                throw;
            }
        }

        public async Task<Expense?> GetByIdAsync(Guid id)
        {
            try
            {
                return await _db.Expenses.FindAsync(id);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error in GetByIdAsync for Expense Id {Id}", id);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Expense expense, int? categoryId = null)
        {
            try
            {
                var existing = await _db.Expenses.FindAsync(expense.Id);
                if (existing == null) return false;

                // update scalar properties
                existing.Amount = expense.Amount;
                existing.Date = expense.Date;
                existing.Description = expense.Description;
                existing.Currency = expense.Currency;
                existing.IsBusiness = expense.IsBusiness;
                existing.UpdatedAt = DateTime.UtcNow;

                if (categoryId.HasValue)
                {
                    var category = await _db.Categories.FindAsync(categoryId.Value);
                    if (category == null) throw new ArgumentException("Category not found", nameof(categoryId));
                    _db.Entry(existing).Property("CategoryId").CurrentValue = categoryId.Value;
                }

                _db.Expenses.Update(existing);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateAsync for Expense Id {Id}", expense.Id);
                throw;
            }
        }
    }
}
