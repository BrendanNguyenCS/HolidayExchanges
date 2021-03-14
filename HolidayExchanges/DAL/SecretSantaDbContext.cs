using HolidayExchanges.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace HolidayExchanges.DAL
{
    public class SecretSantaDbContext : DbContext
    {
        public SecretSantaDbContext() : base("SecretSanta2")
        {
        }

        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserGroup> UserGroups { get; set; }
        public virtual DbSet<Wish> Wishes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // prevents table names from being pluralized
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            // prevents relevant entries between usergroups and the other 2 models individually
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            // usergroup composite primary key
            modelBuilder.Entity<UserGroup>()
                .HasKey(u => new { u.UserID, u.GroupID });
            // group primary key
            modelBuilder.Entity<Group>()
                .HasKey(g => g.GroupID);
            // configure optional recipient user id property
            modelBuilder.Entity<UserGroup>()
                .Property(u => u.RecipientUserID)
                .IsOptional();
            // one to many relationship between user and usergroup
            modelBuilder.Entity<User>()
                .HasMany(u => u.UserGroups)
                .WithRequired()
                .HasForeignKey(u => u.UserID);
            // one to many relationship between group and usergroup
            modelBuilder.Entity<Group>()
                .HasMany(g => g.UserGroups)
                .WithRequired()
                .HasForeignKey(g => g.GroupID);
            // one to many relationship between user and wishes
            modelBuilder.Entity<User>()
                .HasMany(u => u.Wishes)
                .WithRequired()
                .HasForeignKey(w => w.UserID);

            #region Joint Table Mapping without UserGroup model

            //modelBuilder.Entity<User>()
            //    .HasMany(u => u.Groups)
            //    .WithMany(u => u.Users)
            //    .Map(ug =>
            //    {
            //        ug.MapLeftKey("UserID");
            //        ug.MapRightKey("GroupID");
            //        ug.ToTable("UserGroup");
            //    });

            #endregion Joint Table Mapping without UserGroup model
        }
    }
}