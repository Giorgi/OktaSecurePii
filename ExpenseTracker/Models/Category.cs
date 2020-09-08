using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Models
{
    public class Category
    {
        public int Id { get; set; }
        
        public string Name { get; set; }

        public IdentityUser CreatedBy { get; set; }
        public string CreatedById { get; set; }

        public List<Expense> Expenses { get; set; }
    }
}
