using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using TesteStefanini.Infrastructure;
using TesteStefanini.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Configuração da string de conexão para SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Adiciona os serviços de infraestrutura (repositórios)
builder.Services.AddInfrastructure(builder.Configuration);

// Adiciona controladores com opções
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Para evitar referências circulares no JSON
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        // Para manter o padrão de nomenclatura em camelCase
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// Configuração do Swagger com suporte a versionamento
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Configuração do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TesteStefanini API", Version = "v1" });
    c.SwaggerDoc("v2", new OpenApiInfo { Title = "TesteStefanini API", Version = "v2" });
    
    // Configuração para autenticação JWT no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configuração do CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// Configuração do pipeline de requisições HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TesteStefanini API v1");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "TesteStefanini API v2");
    });
    
    // Cria o banco de dados automaticamente em ambiente de desenvolvimento
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.EnsureCreated();
    }
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Middleware de autenticação será adicionado posteriormente
// app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
