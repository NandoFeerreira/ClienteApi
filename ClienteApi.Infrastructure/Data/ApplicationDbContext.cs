using ClienteApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClienteApi.Infrastructure.Data
{
    /// <summary>
    /// Context principal do Entity Framework Core
    /// Responsável por gerenciar as entidades e configurações do banco de dados
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; } = null!;
        public DbSet<Endereco> Enderecos { get; set; } = null!;
        public DbSet<Contato> Contatos { get; set; } = null!;

        /// <summary>
        /// Configuração do modelo usando Fluent API
        /// Seguindo as melhores práticas da Microsoft
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.ToTable("Clientes");

                entity.HasKey(c => c.Id);

                entity.Property(c => c.Id)
                    .HasColumnName("Id")
                    .ValueGeneratedOnAdd();

                entity.Property(c => c.Nome)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnName("Nome");

                entity.Property(c => c.DataCadastro)
                    .IsRequired()
                    .HasColumnName("DataCadastro")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasMany(c => c.Enderecos)
                    .WithOne(e => e.Cliente)
                    .HasForeignKey(e => e.ClienteId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(c => c.Contatos)
                    .WithOne(ct => ct.Cliente)
                    .HasForeignKey(ct => ct.ClienteId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(c => c.Nome)
                    .HasDatabaseName("IX_Clientes_Nome");

                entity.HasIndex(c => c.DataCadastro)
                    .HasDatabaseName("IX_Clientes_DataCadastro");
            });

            modelBuilder.Entity<Endereco>(entity =>
            {
                entity.ToTable("Enderecos");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("Id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Cep)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("Cep");

                entity.Property(e => e.Logradouro)
                    .IsRequired()
                    .HasMaxLength(300)
                    .HasColumnName("Logradouro");

                entity.Property(e => e.Cidade)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("Cidade");

                entity.Property(e => e.Numero)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnName("Numero");

                entity.Property(e => e.Complemento)
                    .HasMaxLength(200)
                    .HasColumnName("Complemento");

                entity.Property(e => e.ClienteId)
                    .IsRequired()
                    .HasColumnName("ClienteId");

                entity.HasIndex(e => e.Cep)
                    .HasDatabaseName("IX_Enderecos_Cep");

                entity.HasIndex(e => e.ClienteId)
                    .HasDatabaseName("IX_Enderecos_ClienteId");
            });

            modelBuilder.Entity<Contato>(entity =>
            {
                entity.ToTable("Contatos");
                
                entity.HasKey(ct => ct.Id);
               
                entity.Property(ct => ct.Id)
                    .HasColumnName("Id")
                    .ValueGeneratedOnAdd();

                entity.Property(ct => ct.Tipo)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("Tipo");

                entity.Property(ct => ct.Texto)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnName("Texto");

                entity.Property(ct => ct.ClienteId)
                    .IsRequired()
                    .HasColumnName("ClienteId");

                entity.HasIndex(ct => ct.Tipo)
                    .HasDatabaseName("IX_Contatos_Tipo");

                entity.HasIndex(ct => ct.ClienteId)
                    .HasDatabaseName("IX_Contatos_ClienteId");
            });
        }
    }
}
