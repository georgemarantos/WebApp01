using CleanDDTest.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanDDTest.Data
{
    public partial class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)

        {

        }

        public virtual DbSet<AdminUser> AdminUsers { get; set; } = null!;
        public virtual DbSet<Bankaccount> Bankaccounts { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Voidedcheck> Voidedchecks { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=tcp:sql-dts-dev-azsqlserver-001.database.windows.net;Authentication=Active Directory Default; Database=sql-dts-dev-directdeposit;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdminUser>(entity =>
            {
                entity.ToTable("adminUser", "dbo");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(225)
                    .HasColumnName("firstName");

                entity.Property(e => e.LastName)
                    .HasMaxLength(225)
                    .HasColumnName("lastName");

                entity.Property(e => e.PasswordReset).HasColumnName("passwordReset");

                entity.Property(e => e.UserEmail)
                    .HasMaxLength(225)
                    .HasColumnName("userEmail");

                entity.Property(e => e.UserPword)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("userPword");

                entity.Property(e => e.UserRole)
                    .HasMaxLength(25)
                    .HasColumnName("userRole");

                entity.Property(e => e.UserStatus).HasColumnName("userStatus");
            });

            modelBuilder.Entity<Bankaccount>(entity =>
            {
                entity.ToTable("Bankaccount", "dbo");

                entity.Property(e => e.AccountNum).UseCollation("Latin1_General_BIN2");

                entity.Property(e => e.BankName).HasMaxLength(50);

                entity.Property(e => e.CityState).HasMaxLength(225);

                entity.Property(e => e.DateProccessed)
                    .HasColumnType("datetime")
                    .HasColumnName("dateProccessed");

                entity.Property(e => e.DateReceived)
                    .HasColumnType("datetime")
                    .HasColumnName("dateReceived");

                entity.Property(e => e.EmpId)
                    .HasMaxLength(50)
                    .UseCollation("Latin1_General_BIN2");

                entity.Property(e => e.RoutingNumber).HasMaxLength(50);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.EmpId);

                entity.ToTable("user", "dbo");

                entity.Property(e => e.EmpId)
                    .HasMaxLength(50)
                    .HasColumnName("EmpID")
                    .UseCollation("Latin1_General_BIN2");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(255)
                    .HasColumnName("firstName");

                entity.Property(e => e.LastName)
                    .HasMaxLength(255)
                    .HasColumnName("lastName");

                entity.Property(e => e.MiddleInit)
                    .HasMaxLength(5)
                    .HasColumnName("middleInit");

                entity.Property(e => e.NetId)
                    .HasMaxLength(25)
                    .HasColumnName("netId");

                entity.Property(e => e.PayPeriod)
                    .HasMaxLength(50)
                    .HasColumnName("payPeriod");
            });

            modelBuilder.Entity<Voidedcheck>(entity =>
            {
                entity.HasKey(e => e.BankAcctId);

                entity.ToTable("voidedchecks", "dbo");

                entity.Property(e => e.BankAcctId)
                    .ValueGeneratedNever()
                    .HasColumnName("bankAcctId");

                entity.Property(e => e.AccountNum)
                    .HasColumnName("accountNum")
                    .UseCollation("Latin1_General_BIN2");

                entity.Property(e => e.EmpId)
                    .HasMaxLength(50)
                    .HasColumnName("EmpID")
                    .UseCollation("Latin1_General_BIN2");

                entity.Property(e => e.FileName).HasColumnName("fileName");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);


    }
}