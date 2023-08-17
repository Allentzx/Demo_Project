using ITC.Core.Interface;
using ITC.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ITC.Core.Data
{
    public class ITCDBContext : AuditableDbContext
    {

        private readonly IUserContextService _userContextService;
        public ITCDBContext(DbContextOptions options, IUserContextService userContextService) : base(options)
        {
            _userContextService = userContextService;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        {
            if (_userContextService.UserID.ToString() == null)
            {
                return await base.SaveChangesAsync(cancellationToken);
            }

            foreach (var entry in ChangeTracker.Entries<IAuditableEntity>().ToList())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTimeOffset.Now;
                        entry.Entity.CreatedById = _userContextService.UserID.ToString();
                        entry.Entity.CreatedBy = _userContextService.FullName.ToString();
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedAt = DateTimeOffset.Now;
                        entry.Entity.LastModifiedById = _userContextService.UserID.ToString();
                        entry.Entity.LastModifiedBy = _userContextService.FullName.ToString();

                        break;

                    case EntityState.Detached:
                    case EntityState.Unchanged:
                    case EntityState.Deleted:
                    default:
                        break;
                }
            }

            return await base.SaveChangesAsync(_userContextService.UserID,_userContextService.FullName, cancellationToken);
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<Registration>(entity =>
            //{
            //    entity.HasKey(e => new { e.ProjectId, e.StudentId });

            //    entity.ToTable("Registration");

            //    entity.HasIndex(e => e.ProjectId, "IX_StudentJoinProject_ProjectId");
            //    entity.HasIndex(e => e.StudentId, "IX_StudentJoinProject_StudentId");

            //    entity.HasOne(d => d.Project)
            //        .WithMany(p => p.Registrations)
            //        .HasForeignKey(d => d.ProjectId).OnDelete(DeleteBehavior.ClientSetNull);

            //    entity.HasOne(d => d.Student)
            //        .WithMany(p => p.Registration)
            //        .HasForeignKey(d => d.StudentId).OnDelete(DeleteBehavior.ClientSetNull);

            //});

            modelBuilder.Entity<JoinProject>(entity =>
            {
                entity.HasKey(e => new { e.Id,e.ProjectId, e.StaffId });

                entity.ToTable("JoinProject");
                entity.HasKey(e=>e.Id)
                        .HasName("PrimaryKey_BlogId");
                entity.HasIndex(e => e.ProjectId, "IX_JoinProject_ProjectId");
                entity.HasIndex(e => e.StaffId, "IX_JoinProjectt_StaffId");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.JoinProjects)
                    .HasForeignKey(d => d.ProjectId).OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Staffs)
                    .WithMany(p => p.JoinProjects)
                    .HasForeignKey(d => d.StaffId).OnDelete(DeleteBehavior.ClientSetNull);

            });


            modelBuilder.Entity<Tasks>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.HasIndex(c => c.ParentId);

                entity.HasOne(c => c.ParentTask)
                       .WithMany(c => c.ChildrenTask)
                       .HasForeignKey(c => c.ParentId).OnDelete(DeleteBehavior.ClientSetNull);

            });


            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(c => c.AccountId);
                entity.HasIndex(c => c.Email).IsUnique(true);
                entity.HasIndex(c => c.PhoneNumber).IsUnique(true).HasFilter("PhoneNumber IS NOT NULL");

            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.HasIndex(c => c.CourseName).IsUnique(true);
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.HasIndex(c => c.Email).IsUnique(true);
                entity.HasIndex(c => c.PhoneNumber).IsUnique(true).HasFilter("PhoneNumber IS NOT NULL");
                entity.HasIndex(c => c.RollNumber).IsUnique(true);
                entity.HasIndex(c => c.MemberCode).IsUnique(true);

            });
            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.HasIndex(c => c.ProjectName).IsUnique(true);
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.HasIndex(c => c.CourseName).IsUnique(true);
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.HasIndex(c => c.Email).IsUnique(true);
                entity.HasIndex(c => c.PhoneNumber).IsUnique(true);
                entity.HasIndex(c => c.RollNumber).IsUnique(true);
                entity.HasIndex(c => c.MemberCode).IsUnique(true);

            });

            modelBuilder.Entity<Slot>()
            .Property(f => f.Id)
            .ValueGeneratedOnAdd();
            modelBuilder.Entity<Phase>()
            .Property(f => f.Id)
            .ValueGeneratedOnAdd();
            modelBuilder.Entity<FileTracking>()
            .Property(f => f.Id)
            .ValueGeneratedOnAdd();

        }
        #region DB Set

        //base v1
        public DbSet<Deputy> Deputy { get; set; }
        public DbSet<Campus> Campus { get; set; }
        public DbSet<Course> Course { get; set; }
        public DbSet<ConfigProject> ConfigProject { get; set; }
        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<JoinProject> JoinProject { get; set; }
        public DbSet<Major> Major { get; set; }
        public DbSet<Notification> Notification { get; set; }
        // public DbSet<NotificationType> NotificationTypes { get; set; }
        public DbSet<Post> Post { get; set; }
        public DbSet<Partner> Partner { get; set; }
        public DbSet<Project> Project { get; set; }
        public DbSet<Registration> Registration { get; set; }
        public DbSet<Slot> Slot { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<Student> Student { get; set; }
        public DbSet<Syllabus> Syllabus { get; set; }

        //add v4
        public DbSet<Phase> Phase { get; set; }
        public DbSet<Reason> Reason { get; set; }
        public DbSet<PostImage> PostImage { get; set; }
        public DbSet<CommentTask> CommentTask { get; set; }
        public DbSet<CancelProject> CancelProject { get; set; }
        //add v5
        public DbSet<FileTracking> FileTracking { get; set; }
        public DbSet<Document> Document { get; set; }
        public DbSet<Account> Account { get; set; }
        public DbSet<AssignTask> AssignTask { get; set; }

        //add v6
        public DbSet<Program> Program { get; set; }
        public DbSet<ProjectPhase> ProjectPhase { get; set; }
        public DbSet<RegistrationAddOn> RegistrationAddOn { get; set; }
        public DbSet<FeedBackAddOn> FeedBackAddOn { get; set; }
        public DbSet<FeedBack> FeedBack { get; set; }




        #endregion
    }
}

