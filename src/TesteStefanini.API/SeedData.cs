using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using TesteStefanini.Domain.Entities;
using TesteStefanini.Infrastructure.Data;

namespace TesteStefanini.API
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());


            context.Database.EnsureCreated();
        }

        public static void LimparDados(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());


            context.Usuarios.RemoveRange(context.Usuarios);
            context.SaveChanges();
        }


    }
}
