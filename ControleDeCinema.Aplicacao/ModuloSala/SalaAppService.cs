using ControledeCinema.Dominio.Compartilhado;
using ControleDeCinema.Aplicacao.Compartilhado;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace ControleDeCinema.Aplicacao.ModuloSala;

public class SalaAppService
{
    private readonly ITenantProvider tenantProvider;
    private readonly IRepositorioSala repositorioSala;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<SalaAppService> logger;

    public SalaAppService(
        ITenantProvider tenantProvider,
        IRepositorioSala repositorioSala,
        IUnitOfWork unitOfWork,
        ILogger<SalaAppService> logger
    )
    {
        this.tenantProvider = tenantProvider;
        this.repositorioSala = repositorioSala;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public Result Cadastrar(Sala sala)
    {
        var registros = repositorioSala.SelecionarRegistros();

        if (registros.Any(i => i.Numero.Equals(sala.Numero)))
            return Result.Fail(ResultadosErro.RegistroDuplicadoErro("Já existe uma sala registrada com este número."));

        try
        {
            sala.UsuarioId = tenantProvider.UsuarioId.GetValueOrDefault();

            repositorioSala.Cadastrar(sala);

            unitOfWork.Commit();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(
                ex,
                "Ocorreu um erro durante o registro de {@Registro}.",
                sala
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result Editar(Guid id, Sala salaEditada)
    {
        var registros = repositorioSala.SelecionarRegistros();

        if (registros.Any(i => !i.Id.Equals(id) && i.Numero.Equals(salaEditada.Numero)))
            return Result.Fail(ResultadosErro.RegistroDuplicadoErro("Já existe uma sala registrada com este número."));

        try
        {
            repositorioSala.Editar(id, salaEditada);

            unitOfWork.Commit();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(
                ex,
                "Ocorreu um erro durante a edição do registro {@Registro}.",
                salaEditada
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result Excluir(Guid id)
    {
        try
        {
            var exclusaoConcluida = repositorioSala.Excluir(id);

            if (!exclusaoConcluida)
                return Result.Fail(ResultadosErro.RegistroNaoEncontradoErro(id));

            unitOfWork.Commit();

            return Result.Ok();

        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(
                ex,
                "Ocorreu um erro durante a exclusão do registro {Id}.",
                id
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result<Sala> SelecionarPorId(Guid id)
    {
        try
        {
            var registroSelecionado = repositorioSala.SelecionarRegistroPorId(id);

            if (registroSelecionado is null)
                return Result.Fail(ResultadosErro.RegistroNaoEncontradoErro(id));

            return Result.Ok(registroSelecionado);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ocorreu um erro durante a seleção do registro {Id}.",
                id
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result<List<Sala>> SelecionarTodos()
    {
        try
        {
            var registros = repositorioSala.SelecionarRegistros();

            return Result.Ok(registros);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ocorreu um erro durante a seleção de registros."
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }
}
