using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using VDMP.DBmodel;

namespace VDMP.DataAccess
{
    public class VDMPContext : DbContext
    {
        public VDMPContext()
        {
        }

        public VDMPContext(DbContextOptions<VDMPContext> options) : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }

        public DbSet<Library> Libraries { get; set; }

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = "(localdb)\\MSSQLLocalDB",
                InitialCatalog = "VDMP",
                IntegratedSecurity = true

            };

            optionsBuilder.UseSqlServer(builder.ConnectionString);
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();
        }
    }
}