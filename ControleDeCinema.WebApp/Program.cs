using ControleDeCinema.Aplicacao.ModuloAutenticacao;
using ControleDeCinema.Aplicacao.ModuloFilme;
using ControleDeCinema.Aplicacao.ModuloGeneroFilme;
using ControleDeCinema.Aplicacao.ModuloSala;
using ControleDeCinema.Aplicacao.ModuloSessao;
using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Dominio.ModuloSessao;
using ControleDeCinema.Infraestrutura.Orm.Compartilhado;
using ControleDeCinema.Infraestrutura.Orm.ModuloFilme;
using ControleDeCinema.Infraestrutura.Orm.ModuloGeneroFilme;
using ControleDeCinema.Infraestrutura.Orm.ModuloSala;
using ControleDeCinema.Infraestrutura.Orm.ModuloSessao;
using ControleDeCinema.WebApp.ActionFilters;
using ControleDeCinema.WebApp.DependencyInjection;
using ControleDeCinema.WebApp.Orm;

namespace ControleDeCinema.WebApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configuração de serviços personalizados
        builder.Services.AddScoped<AutenticacaoAppService>();

        builder.Services.AddScoped<SessaoAppService>();
        builder.Services.AddScoped<IngressoAppService>();
        builder.Services.AddScoped<IRepositorioSessao, RepositorioSessaoEmOrm>();
        builder.Services.AddScoped<IRepositorioIngresso, RepositorioIngressoEmOrm>();
        builder.Services.AddScoped<SalaAppService>();
        builder.Services.AddScoped<IRepositorioSala, RepositorioSalaEmOrm>();
        builder.Services.AddScoped<FilmeAppService>();
        builder.Services.AddScoped<IRepositorioFilme, RepositorioFilmeEmOrm>();
        builder.Services.AddScoped<GeneroFilmeAppService>();
        builder.Services.AddScoped<IRepositorioGeneroFilme, RepositorioGeneroFilmeEmOrm>();

        builder.Services.AddEntityFrameworkConfig(builder.Configuration);
        builder.Services.AddIdentityProviderConfig();
        builder.Services.AddCookieAuthenticationConfig();
        builder.Services.AddSerilogConfig(builder.Logging, builder.Configuration);

        // Configuração de serviços da Microsoft
        builder.Services.AddControllersWithViews(options =>
        {
            options.Filters.Add<ValidarModeloAttribute>();
        });

        builder.Services.AddHealthChecks()
            .AddDbContextCheck<ControleDeCinemaDbContext>();

        // Build das dependências
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.ApplyMigrations();

            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/erro");
        }

        app.UseAntiforgery();
        app.UseStaticFiles();
        app.UseRouting();

        app.MapHealthChecks("/health");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapDefaultControllerRoute();

        app.Run();
    }
}
