using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Redirect.Models
{
    public class RedirectContext : DbContext
    {
        public RedirectContext (DbContextOptions<RedirectContext> options)
            : base(options)
        {
        }

        public DbSet<Redirect.Models.UriModel> UriModel { get; set; }
    }
}
