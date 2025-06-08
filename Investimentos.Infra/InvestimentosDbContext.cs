using Investimentos.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Investimentos.Infra
{
    public class InvestimentosDbContext : DbContext
    {
        public InvestimentosDbContext(DbContextOptions<InvestimentosDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Ativo> Ativos => Set<Ativo>();
        public DbSet<Operacao> Operacoes => Set<Operacao>();
        public DbSet<Cotacao> Cotacoes => Set<Cotacao>();
        public DbSet<Posicao> Posicoes => Set<Posicao>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("usuarios");
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Id).HasColumnName("id");
                entity.Property(u => u.Nome).HasColumnName("nome").HasMaxLength(100).IsRequired();
                entity.Property(u => u.Email).HasColumnName("email").HasMaxLength(100).IsRequired();
                entity.Property(u => u.PercentualCorretagem).HasColumnName("pct_corretagem").HasColumnType("decimal(5,2)");
            });

            modelBuilder.Entity<Ativo>(entity =>
            {
                entity.ToTable("ativos");
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Id).HasColumnName("id");
                entity.Property(a => a.Codigo).HasColumnName("codigo").HasMaxLength(10).IsRequired();
                entity.Property(a => a.Nome).HasColumnName("nome").HasMaxLength(100).IsRequired();
            });

            modelBuilder.Entity<Operacao>(entity =>
            {
                entity.ToTable("operacoes");
                entity.HasKey(o => o.Id);
                entity.Property(o => o.Id).HasColumnName("id");
                entity.Property(o => o.UsuarioId).HasColumnName("usuario_id");
                entity.Property(o => o.AtivoId).HasColumnName("ativo_id");
                entity.Property(o => o.Quantidade).HasColumnName("qtd");
                entity.Property(o => o.PrecoUnitario).HasColumnName("preco_unit").HasColumnType("decimal(15,4)");
                entity.Property(o => o.TipoOperacao).HasColumnName("tipo_operacao").HasMaxLength(10).IsRequired();
                entity.Property(o => o.Corretagem).HasColumnName("corretagem").HasColumnType("decimal(10,2)");
                entity.Property(o => o.DataHora).HasColumnName("dt_hr").IsRequired();

                entity.HasOne(o => o.Usuario).WithMany(u => u.Operacoes).HasForeignKey(o => o.UsuarioId);
                entity.HasOne(o => o.Ativo).WithMany(a => a.Operacoes).HasForeignKey(o => o.AtivoId);
            });

            modelBuilder.Entity<Cotacao>(entity =>
            {
                entity.ToTable("cotacoes");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Id).HasColumnName("id");
                entity.Property(c => c.AtivoId).HasColumnName("ativo_id");
                entity.Property(c => c.PrecoUnitario).HasColumnName("preco_unit").HasColumnType("decimal(15,4)");
                entity.Property(c => c.DataHora).HasColumnName("dt_hr").IsRequired();

                entity.HasOne(c => c.Ativo).WithMany(a => a.Cotacoes).HasForeignKey(c => c.AtivoId);
            });

            modelBuilder.Entity<Posicao>(entity =>
            {
                entity.ToTable("posicoes");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id).HasColumnName("id");
                entity.Property(p => p.UsuarioId).HasColumnName("usuario_id");
                entity.Property(p => p.AtivoId).HasColumnName("ativo_id");
                entity.Property(p => p.Quantidade).HasColumnName("qtd");
                entity.Property(p => p.PrecoMedio).HasColumnName("preco_medio").HasColumnType("decimal(15,4)");
                entity.Property(p => p.PL).HasColumnName("pl").HasColumnType("decimal(15,2)");

                entity.HasOne(p => p.Usuario).WithMany(u => u.Posicoes).HasForeignKey(p => p.UsuarioId);
                entity.HasOne(p => p.Ativo).WithMany(a => a.Posicoes).HasForeignKey(p => p.AtivoId);

                entity.HasIndex(p => new { p.UsuarioId, p.AtivoId }).IsUnique().HasDatabaseName("uk_usuario_ativo");
            });
        }
    }
}