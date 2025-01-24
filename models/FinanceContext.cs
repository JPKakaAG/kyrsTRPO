using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace kyrsTRPO.models;

public partial class FinanceContext : DbContext
{
    public FinanceContext()
    {
    }

    public FinanceContext(DbContextOptions<FinanceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Пользователь> Пользовательs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-I00R4RJ; Database=finance; User=des; Password=1234567890; Encrypt=false");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Пользователь>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Пользова__3214EC27EEC02411");

            entity.ToTable("Пользователь");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Логин).HasMaxLength(50);
            entity.Property(e => e.Пароль).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
