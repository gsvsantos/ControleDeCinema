using ControleDeCinema.Dominio.ModuloAutenticacao;
using System.Security.Claims;

namespace ControleDeCinema.WebApp.Identity;

public class IdentityTenantProvider(IHttpContextAccessor contextAccessor) : ITenantProvider
{
    public Guid? UsuarioId
    {
        get
        {
            var claimId = contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);

            if (claimId is null)
                return null;

            return Guid.Parse(claimId.Value);
        }
    }

    public bool IsInRole(string role)
    {
        return contextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
    }
}
