using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Infraestrutura.Orm.Compartilhado;
using ControleDeCinema.WebApp.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace ControleDeCinema.WebApp.DependencyInjection;

public static class IdentityConfig
{
    public static void AddIdentityProviderConfig(this IServiceCollection services)
    {
        services.AddScoped<ITenantProvider, IdentityTenantProvider>();

        services.AddIdentity<Usuario, Cargo>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;
        })
        .AddEntityFrameworkStores<ControleDeCinemaDbContext>()
        .AddDefaultTokenProviders();
    }

    public static void AddCookieAuthenticationConfig(this IServiceCollection services)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "AspNetCore.Cookies";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                options.SlidingExpiration = true;
            });

        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/autenticacao/login";
            options.AccessDeniedPath = "/";
        });
    }
}
