using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Triowave.Models;

public partial class GeneralDWContext : DbContext
{
    public GeneralDWContext()
    {
    }

    public GeneralDWContext(DbContextOptions<GeneralDWContext> options)
        : base(options)
    {
    }

    public virtual DbSet<StockPrice> StockPrices { get; set; }

    public virtual DbSet<Symbol> Symbols { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=LivTop\\TRIOWAVEDEV;Database=GeneralDW;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
