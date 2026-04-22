namespace ExpenseTracker.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public class CategoryService : ICategoryService
    {
        private readonly ExpenseDbContext _db;
        private readonly Microsoft.Extensions.Logging.ILogger<CategoryService> _logger;

        public CategoryService(ExpenseDbContext db, Microsoft.Extensions.Logging.ILogger<CategoryService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Category> CreateAsync(Category category)
        {
            try
            {
                await _db.Categories.AddAsync(category);
                await _db.SaveChangesAsync();
                return category;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error in CreateAsync for Category");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var existing = await _db.Categories.FindAsync(id);
                if (existing == null) return false;
                _db.Categories.Remove(existing);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteAsync for Category Id {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            try
            {
                return await _db.Categories.AsNoTracking().ToListAsync();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllAsync for Categories");
                throw;
            }
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            try
            {
                return await _db.Categories.FindAsync(id);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error in GetByIdAsync for Category Id {Id}", id);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Category category)
        {
            try
            {
                var existing = await _db.Categories.FindAsync(category.Id);
                if (existing == null) return false;
                existing.Name = category.Name;
                _db.Categories.Update(existing);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateAsync for Category Id {Id}", category.Id);
                throw;
            }
        }
    }
}
