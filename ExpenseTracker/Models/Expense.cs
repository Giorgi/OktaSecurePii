using System;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Models
{
    public class Expense
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }

        public DateTime CreatedAt { get; set; }

        public string CreatedById { get; set; }

        public Category Category { get; set; }
        public int CategoryId { get; set; }
    }
}