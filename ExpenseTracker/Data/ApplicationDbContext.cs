using System;
using System.Collections.Generic;
using System.Text;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Expense> Expenses { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Category>().Property(category => category.Name).IsRequired();
            builder.Entity<Category>().HasMany(category => category.Expenses)
                                      .WithOne(expense => expense.Category).IsRequired();

            builder.Entity<Expense>().Property(expense => expense.CreatedById).IsRequired();

            builder.Entity<Category>().HasData(new Category { Name = "Travel", Id = -1 }, new Category { Name = "Education", Id = -2 },
                                               new Category { Name = "Food", Id = -3 }, new Category { Name = "Healthcare", Id = -4 });
        }
    }
}
