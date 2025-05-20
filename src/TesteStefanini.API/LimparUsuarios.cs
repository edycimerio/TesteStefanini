using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using TesteStefanini.Infrastructure.Data;

namespace TesteStefanini.API
{
    public class LimparUsuarios
    {
        public static void Main(string[] args)
        {
            // Configurar os serviços
            var serviceProvider = new ServiceCollection()
                .AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlite("Data Source=Database/TesteStefanini.db"))
                .BuildServiceProvider();

            // Obter o contexto
            using var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Remover todos os usuários
            Console.WriteLine("Removendo todos os usuários...");
            context.Usuarios.RemoveRange(context.Usuarios);
            context.SaveChanges();
            Console.WriteLine("Usuários removidos com sucesso!");
        }
    }
}
