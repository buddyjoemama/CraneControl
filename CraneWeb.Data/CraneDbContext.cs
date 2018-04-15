using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraneWeb.Data
{
    public class CraneDbContext : DbContext
    {
        public CraneDbContext() : base("DefaultConnection")
        {
           
        }        

        public DbSet<CraneOperation> CraneOperations { get; set; }
    }
}
