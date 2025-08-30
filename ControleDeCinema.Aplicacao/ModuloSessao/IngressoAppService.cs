using ControleDeCinema.Aplicacao.Compartilhado;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloSessao;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace ControleDeCinema.Aplicacao.ModuloSessao;

public class IngressoAppService
{
    private readonly ITenantProvider tenantProvider;
    private readonly IRepositorioIngresso repositorioIngresso;
    private readonly ILogger<IngressoAppService> logger;

    public IngressoAppService(
        ITenantProvider tenantProvider,
        IRepositorioIngresso repositorioIngresso,
        ILogger<IngressoAppService> logger
    )
    {
        this.tenantProvider = tenantProvider;
        this.repositorioIngresso = repositorioIngresso;
        this.logger = logger;
    }

    public Result<List<Ingresso>> SelecionarTodos()
    {
        try
        {
            var registros = repositorioIngresso.SelecionarRegistros(tenantProvider.UsuarioId.GetValueOrDefault());

            return Result.Ok(registros);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao selecionar ingressos.");

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }
}
