using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HelpdeskDAL;

public partial class TestDbContext : DbContext
{
    public TestDbContext()
    {
    }

    public TestDbContext(DbContextOptions<TestDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Call> Calls { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Problem> Problems { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<UserEmail> UserEmails { get; set; }

    public virtual DbSet<UserLogin> UserLogins { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=KEITHLAPTOP\\INFO5052;Database=TestDB;User Id=tuskit0201;Password=Kiet@020105;TrustServerCertificate=True;");
        optionsBuilder.UseLazyLoadingProxies();
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Call>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Call");

            entity.Property(e => e.DateClosed).HasColumnType("smalldatetime");
            entity.Property(e => e.DateOpened).HasColumnType("smalldatetime");
            entity.Property(e => e.ExpectedProcessingDays).HasDefaultValue(0);
            entity.Property(e => e.Notes)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Timer)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Customer).WithMany(p => p.Calls)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Calls_Customers");

            entity.HasOne(d => d.Employee).WithMany(p => p.Calls)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CallHasEmployee");

            entity.HasOne(d => d.Problem).WithMany(p => p.Calls)
                .HasForeignKey(d => d.ProblemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CallHasProblem");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Customer__3214EC0748674B09");

            entity.HasIndex(e => e.Email, "UQ_Customers_Email").IsUnique();

            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNo)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Timer)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Title).HasMaxLength(4);

            entity.HasOne(d => d.EmailNavigation).WithOne(p => p.Customer)
                .HasPrincipalKey<UserEmail>(p => p.Email)
                .HasForeignKey<Customer>(d => d.Email)
                .HasConstraintName("FK_Customers_Email");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Department");

            entity.Property(e => e.DepartmentName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Timer)
                .IsRowVersion()
                .IsConcurrencyToken();
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Employee");

            entity.HasIndex(e => e.Email, "UQ_Employees_Email").IsUnique();

            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNo)
                .HasMaxLength(25)
                .IsUnicode(false);
            entity.Property(e => e.Timer)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Title)
                .HasMaxLength(4)
                .IsUnicode(false);

            entity.HasOne(d => d.Department).WithMany(p => p.Employees)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeeInDept");

            entity.HasOne(d => d.EmailNavigation).WithOne(p => p.Employee)
                .HasPrincipalKey<UserEmail>(p => p.Email)
                .HasForeignKey<Employee>(d => d.Email)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Empolyees_Email");
        });

        modelBuilder.Entity<Problem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Problem");

            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Timer)
                .IsRowVersion()
                .IsConcurrencyToken();
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC07249FB264");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B61606C5A7DDC").IsUnique();

            entity.Property(e => e.RoleName).HasMaxLength(50);
            entity.Property(e => e.Timer)
                .IsRowVersion()
                .IsConcurrencyToken();
        });

        modelBuilder.Entity<UserEmail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserEmai__3214EC0780B24C8E");

            entity.HasIndex(e => e.Email, "UQ_UserEmails_Email").IsUnique();

            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Timer)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.EmailNavigation).WithOne(p => p.UserEmail)
                .HasPrincipalKey<UserLogin>(p => p.Email)
                .HasForeignKey<UserEmail>(d => d.Email)
                .HasConstraintName("FK_UserEmails_Email");
        });

        modelBuilder.Entity<UserLogin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserLogi__3214EC07CD4D3B2F");

            entity.ToTable("UserLogin");

            entity.HasIndex(e => e.Email, "UQ_UserLogin_Email").IsUnique();

            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Timer)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.UserPassword)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Customer).WithMany(p => p.UserLogins)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_UserLogin_CustomerId");

            entity.HasOne(d => d.Employee).WithMany(p => p.UserLogins)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK_UserLogin_EmployeeId");

            entity.HasOne(d => d.Role).WithMany(p => p.UserLogins)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_UserLogin_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
