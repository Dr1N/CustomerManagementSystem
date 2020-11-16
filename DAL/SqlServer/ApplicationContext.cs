using Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace DAL.SqlServer
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext()
        {
        }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Customer> Customers { get; set; }

        public virtual DbSet<RoleCustomer> RoleCustomer { get; set; }

        public virtual DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("customers");

                entity.HasIndex(e => e.Login)
                    .IsUnique();

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreatedBy).HasDefaultValueSql("((1))");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Login)
                    .IsRequired()
                    .HasMaxLength(15);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.PhoneNumber).HasMaxLength(50);

                entity.Property(e => e.Updated)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UpdatedBy).HasDefaultValueSql("((1))");

                entity.Property(e => e.Deleted).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.CreatedByCustomer)
                    .WithMany(p => p.CreatedCustomers)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_createdby_customer");

                entity.HasOne(d => d.UpdatedByCustomer)
                    .WithMany(p => p.UpdatedCustomers)
                    .HasForeignKey(d => d.UpdatedBy)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_updateby_customer");
            });

            modelBuilder.Entity<RoleCustomer>(entity =>
            {
                entity.ToTable("role_customer");

                entity.HasKey(bc => new { bc.RoleId, bc.CustomerId });

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.RoleCustomer)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_role_customers");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RoleCustomer)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_role_roles");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("roles");

                entity.HasIndex(e => e.Name)
                    .IsUnique();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });
        }
    }
}
