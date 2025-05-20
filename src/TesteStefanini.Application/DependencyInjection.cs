using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TesteStefanini.Application.Interfaces;
using TesteStefanini.Application.Services;
using TesteStefanini.Domain.Entities;
using TesteStefanini.Domain.Validators;

namespace TesteStefanini.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            // Registra os serviços
            services.AddScoped<IPessoaService, PessoaService>();
            services.AddScoped<IPessoaServiceV2, PessoaServiceV2>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUsuarioService, UsuarioService>();

            // Registra os validadores
            services.AddScoped<IValidator<Pessoa>, PessoaValidator>();
            services.AddScoped<IValidator<Endereco>, EnderecoValidator>();

            // Registra o AutoMapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
