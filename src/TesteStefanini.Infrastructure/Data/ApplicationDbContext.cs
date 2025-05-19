using Microsoft.EntityFrameworkCore;
using TesteStefanini.Domain.Entities;

namespace TesteStefanini.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Pessoa> Pessoas { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração da entidade Pessoa
            modelBuilder.Entity<Pessoa>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CPF).IsRequired().HasMaxLength(14);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Sexo).HasMaxLength(20);
                entity.Property(e => e.Naturalidade).HasMaxLength(100);
                entity.Property(e => e.Nacionalidade).HasMaxLength(100);
                
                // Índice para busca rápida por CPF
                entity.HasIndex(e => e.CPF).IsUnique();
            });

            // Configuração da entidade Endereco
            modelBuilder.Entity<Endereco>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Logradouro).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Numero).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Complemento).HasMaxLength(100);
                entity.Property(e => e.Bairro).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Cidade).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Estado).IsRequired().HasMaxLength(2);
                entity.Property(e => e.CEP).IsRequired().HasMaxLength(9);

                // Relacionamento com Pessoa
                entity.HasOne(e => e.Pessoa)
                      .WithMany()
                      .HasForeignKey(e => e.PessoaId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuração da entidade Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Senha).IsRequired();
                entity.Property(e => e.Salt).IsRequired();
                
                // Índice para busca rápida por Email
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Seed de dados iniciais (usuários para autenticação)
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Adicionar usuário padrão para autenticação
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario(
                    "Administrador", 
                    "admin@teste.com", 
                    // Senha: "admin123" com hash
                    "AQAAAAIAAYagAAAAELTsGAC+WQGKG3btvXYZcTQxJvwpg6vJ2GVYbQ3z9BQQdi+YYXpJIWNnOJ8Lc7aMQA==", 
                    "79e05e9d-9e0d-4c94-9a69-271568a58320"
                ) { Id = Guid.Parse("79e05e9d-9e0d-4c94-9a69-271568a58320"), DataCadastro = DateTime.Now }
            );
        }
    }
}
