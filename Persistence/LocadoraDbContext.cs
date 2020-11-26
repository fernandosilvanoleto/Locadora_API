using Locadora_API.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Locadora_API.Persistence
{
    public class LocadoraDbContext : DbContext
    {
        public LocadoraDbContext(DbContextOptions<LocadoraDbContext> options)
            : base(options)
        {
        }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Filme> Filmes { get; set; }
        public DbSet<EstoqueFilme> EstoqueFilme { get; set; }
        public DbSet<Locacao> Locacao { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cliente>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Cliente>()                
                .HasMany(c => c.Locacoes)
                .WithOne()
                .HasForeignKey(lo => lo.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.Nome).IsUnique();

            modelBuilder.Entity<Filme>()
                .HasKey(f => f.Id);

            modelBuilder.Entity<Filme>()
                .HasMany(f => f.Locacoes)
                .WithOne()
                .HasForeignKey(lo => lo.FilmeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Locacao>()
                .HasKey(l => l.Id);

            modelBuilder.Entity<EstoqueFilme>()
                .HasKey(ef => ef.Id);

            modelBuilder.Entity<EstoqueFilme>(ef => {
                ef.HasIndex(ef => ef.FilmeId)
                    .IsUnique();
            });

        }
    }
}
