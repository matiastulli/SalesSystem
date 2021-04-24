using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SalesSystem.Areas.Customers.Models;
using SalesSystem.Areas.Setting.Models;
using SalesSystem.Areas.Users.Models;

namespace SalesSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        static DbContextOptions<ApplicationDbContext> _options;
        public ApplicationDbContext() : base(_options)
        {

        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            _options = options;
        }
        public DbSet<TUsers> TUsers { get; set; }
        public DbSet<TClients> TClients { get; set; }
        public DbSet<TReports_clients> TReports_clients { get; set; }
        public DbSet<TPayments_clients> TPayments_clients { get; set; }
        public DbSet<TCustomer_interests> TCustomer_interests { get; set; }
        public DbSet<TCustomer_interests_reports> TCustomer_interests_reports { get; set; }
        public DbSet<TPayments_Reports_Customer_Interest> TPayments_Reports_Customer_Interest { get; set; }
        public DbSet<TSetting> TSetting { get; set; }

    }
}
