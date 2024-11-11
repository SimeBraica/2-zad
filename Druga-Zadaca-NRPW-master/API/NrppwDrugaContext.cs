using System;
using System.Collections.Generic;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API;

public partial class NrppwDrugaContext : DbContext
{
    public NrppwDrugaContext()
    {
    }

    public NrppwDrugaContext(DbContextOptions<NrppwDrugaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Owner> Owners { get; set; }

    public virtual DbSet<Pet> Pets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=dpg-csidcst6l47c73f8u500-a.frankfurt-postgres.render.com;Database=nrppw_druga;Username=nrppw_druga_user;Password=iJuoqCllmMBKXgKkB4Ail0ndi2qZ7pBf");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Owner>(entity =>
        {
            entity.HasKey(e => e.OwnerId).HasName("person_pkey");

            entity.ToTable("owner");

            entity.Property(e => e.OwnerId)
                .HasDefaultValueSql("nextval('person_user_id_seq'::regclass)")
                .HasColumnName("owner_id");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .HasColumnName("username");
        });

        modelBuilder.Entity<Pet>(entity =>
        {
            entity.HasKey(e => e.PetId).HasName("pet_pkey");

            entity.ToTable("pet");

            entity.Property(e => e.PetId).HasColumnName("pet_id");
            entity.Property(e => e.Animal)
                .HasMaxLength(255)
                .HasColumnName("animal");
            entity.Property(e => e.OwnerId).HasColumnName("owner_id");
            entity.Property(e => e.PetName)
                .HasMaxLength(255)
                .HasColumnName("pet_name");

            entity.HasOne(d => d.Owner).WithMany(p => p.Pets)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_owner");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
