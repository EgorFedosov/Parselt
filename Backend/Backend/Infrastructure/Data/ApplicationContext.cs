namespace Backend.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using Entities;

public sealed class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
    }

    public DbSet<CsvParsedCell> CsvParsedCells { get; set; } = null!;
    public DbSet<CsvRawCell> CsvRawCells { get; set; } = null!;
    public DbSet<OperationLog> OperationLogs { get; set; } = null!;
    public DbSet<ErrorLog> ErrorLogs { get; set; } = null!;
    public DbSet<FileMetaData> FileMetaData { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CsvParsedCell>().HasKey(p => p.Id);
        modelBuilder.Entity<CsvRawCell>().HasKey(p => p.Id);
        modelBuilder.Entity<OperationLog>().HasKey(o => o.Id);
        modelBuilder.Entity<ErrorLog>().HasKey(e => e.Id);
        modelBuilder.Entity<FileMetaData>().HasKey(e => e.Id);

        modelBuilder.Entity<ErrorLog>()
            .HasOne(e => e.OperationLog)
            .WithMany(o => o.Errors)
            .HasForeignKey(e => e.OperationLogId);

        modelBuilder.Entity<OperationLog>()
            .Property(o => o.Type)
            .HasConversion<string>();

        modelBuilder.Entity<OperationLog>()
            .Property(o => o.Status)
            .HasConversion<string>();

        modelBuilder.Entity<FileMetaData>()
            .HasMany(e => e.CsvRawCells) 
            .WithOne(e => e.FileMetaData)
            .HasForeignKey(r => r.FileMetaDataId);

        modelBuilder.Entity<FileMetaData>()
            .HasMany(e => e.CsvParsedCells)
            .WithOne(e => e.FileMetaData)
            .HasForeignKey(r => r.FileMetaDataId);
        
        modelBuilder.Entity<FileMetaData>()
            .HasMany(e => e.OperationLogs)
            .WithOne(e => e.FileMetaData)
            .HasForeignKey(r => r.FileMetaDataId);
    }
}