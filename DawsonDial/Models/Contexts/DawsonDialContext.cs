using Microsoft.EntityFrameworkCore;
using DawsonDial.Models.Rooms;
using DawsonDial.Models.Events;
using DawsonDial.Models.People;

namespace DawsonDial.Models.Contexts
{
    /// <summary>
    /// Represents the context for the Dawson Dial application.
    /// </summary>
    public class DawsonDialContext : DbContext
    {
        // Event related DbSets
        public DbSet<Event> Events { get; set; }
        public DbSet<Conference> Conferences { get; set; }
        public DbSet<OfficeMeeting> OfficeMeetings { get; set; }
        public DbSet<OfficeHours> OfficeHours { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<SectionRoom> SectionRooms { get; set; }
        public DbSet<ClassSession> ClassSessions { get; set; }

        // People DbSets
        public DbSet<Person> People { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Admin> Admins { get; set; }

        // Room DbSets
        public DbSet<Room> Rooms { get; set; }
        public DbSet<ClassRoom> ClassRooms { get; set; }
        public DbSet<ConferenceRoom> ConferenceRooms { get; set; }
        public DbSet<Office> Offices { get; set; }

        //WorkLog DbSets
        public DbSet<AdminLog> AdminLogs { get; set; }

        // Db Settings
        public string DbPath { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DawsonDialContext"/> class.
        /// </summary>
        public DawsonDialContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "dawsondial.db");
        }

        /// <summary>
        /// Configures the database connection.
        /// </summary>
        /// <param name="optionsBuilder">The options builder.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // SQLite Configuration
            // optionsBuilder.UseSqlite($"Data Source={DbPath}");

            // PostgreSQL Configuration
            string? postgresUser = Environment.GetEnvironmentVariable("POSTGRES_USER");
            string? postgresPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
            string? postgresHost = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";
            string? postgresPort = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432";
            string? postgresDbName = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "dawson_dial";

            if (string.IsNullOrEmpty(postgresUser) || string.IsNullOrEmpty(postgresPassword))
                throw new InvalidOperationException("PostgreSQL connection details are missing.");

            // Build Connection String and connect to Datebase (must export environment variables for user and password)
            var connectionString = $"Host={postgresHost};Port={postgresPort};Username={postgresUser};Password={postgresPassword};Database={postgresDbName};Include Error Detail=true;";
            optionsBuilder.UseNpgsql(connectionString);
        }

        /// <summary>
        /// Configures the model for the database.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Event Building
            modelBuilder.Entity<Course>()
                .Property(c => c.RowVersion)
                .IsRowVersion()
                .HasColumnType("bytea");

            modelBuilder.Entity<Event>()
                .Property(e => e.RowVersion)
                .IsRowVersion()
                .HasColumnType("bytea");

            modelBuilder.Entity<Schedule>()
                .Property(s => s.RowVersion)
                .IsRowVersion()
                .HasColumnType("bytea");

            modelBuilder.Entity<Schedule>()
                .HasMany(s => s.TimeSlots)
                .WithOne(ts => ts.Schedule)
                .HasForeignKey(ts => ts.ScheduleId)
                .IsRequired();

            modelBuilder.Entity<Section>()
                .Property(s => s.RowVersion)
                .IsRowVersion()
                .HasColumnType("bytea");

            // User Building
            modelBuilder.Entity<Person>()
                .Property(p => p.RowVersion)
                .IsRowVersion()
                .HasColumnType("bytea");

            modelBuilder.Entity<Teacher>()
                .HasOne(t => t.OfficeHours)
                .WithOne(o => o.Teacher)
                .HasForeignKey<OfficeHours>(o => o.TeacherId)
                .IsRequired(false);

            // Room Building
            modelBuilder.Entity<Room>()
                .Property(r => r.RowVersion)
                .IsRowVersion()
                .HasColumnType("bytea");

            modelBuilder.Entity<OfficeHours>()
                .HasOne(o => o.Schedule)
                .WithMany()
                .HasForeignKey(o => o.ScheduleId)
                .IsRequired();
        }
    }
}
