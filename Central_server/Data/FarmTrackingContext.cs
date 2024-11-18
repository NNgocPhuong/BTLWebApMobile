using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Central_server.Data;

public partial class FarmTrackingContext : DbContext
{
    public FarmTrackingContext()
    {
    }

    public FarmTrackingContext(DbContextOptions<FarmTrackingContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<SensorsDatum> SensorsData { get; set; }

    public virtual DbSet<Setting> Settings { get; set; }

    public virtual DbSet<Station> Stations { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Valf> Valves { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=THINKPAD-T490\\SQLEXPRESS01;Initial Catalog=FarmTracking;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__Schedule__9C8A5B4902049938");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.Frequency).HasMaxLength(20);
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");

            entity.HasOne(d => d.Valve).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.ValveId)
                .HasConstraintName("FK__Schedules__Valve__29221CFB");
        });

        modelBuilder.Entity<SensorsDatum>(entity =>
        {
            entity.HasKey(e => e.DataId).HasName("PK__SensorsD__9D05303D61562ED4");

            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Station).WithMany(p => p.SensorsData)
                .HasForeignKey(d => d.StationId)
                .HasConstraintName("FK__SensorsDa__Stati__4E88ABD4");
        });

        modelBuilder.Entity<Setting>(entity =>
        {
            entity.HasKey(e => e.SettingId).HasName("PK__Settings__54372B1DC62303EE");

            entity.Property(e => e.SettingType).HasMaxLength(50);
            entity.Property(e => e.SettingUrl).HasMaxLength(200);
        });

        modelBuilder.Entity<Station>(entity =>
        {
            entity.HasKey(e => e.StationId).HasName("PK__Stations__E0D8A6BDA3C474A8");

            entity.Property(e => e.Location).HasMaxLength(100);
            entity.Property(e => e.Status).HasDefaultValue(true);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CAA3DAAA6");

            entity.HasIndex(e => e.UserName, "UQ__Users__C9F28456E9B663E4").IsUnique();

            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.PhotoUrl).HasMaxLength(200);
            entity.Property(e => e.StudentId).HasMaxLength(20);
            entity.Property(e => e.UserName).HasMaxLength(100);
        });

        modelBuilder.Entity<Valf>(entity =>
        {
            entity.HasKey(e => e.ValveId).HasName("PK__Valves__A5F1AE77B1760FB1");

            entity.Property(e => e.LastUpdated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.ValveName).HasMaxLength(100);
            entity.Property(e => e.ValveType).HasMaxLength(50);

            entity.HasOne(d => d.Station).WithMany(p => p.Valves)
                .HasForeignKey(d => d.StationId)
                .HasConstraintName("FK__Valves__StationI__02FC7413");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
