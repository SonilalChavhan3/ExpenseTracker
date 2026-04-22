namespace ExpenseTracker
{
    using Microsoft.EntityFrameworkCore;

    public class ExpenseDbContext : DbContext
    {
        public ExpenseDbContext(DbContextOptions<ExpenseDbContext> options)
            : base(options)
        {
        }

        public DbSet<Expense> Expenses { get; set; } = null!;

        public DbSet<Category> Categories { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>()
                .HasMany(c => c.Expenses)
                .WithOne()
                .HasForeignKey("CategoryId")
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
