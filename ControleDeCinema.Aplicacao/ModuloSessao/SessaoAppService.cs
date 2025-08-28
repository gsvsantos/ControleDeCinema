using ControledeCinema.Dominio.Compartilhado;
using ControleDeCinema.Aplicacao.Compartilhado;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloSessao;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace ControleDeCinema.Aplicacao.ModuloSessao;

public class SessaoAppService
{
    private readonly ITenantProvider tenantProvider;
    private readonly IRepositorioSessao repositorioSessao;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<SessaoAppService> logger;

    public SessaoAppService(
        ITenantProvider tenantProvider,
        IRepositorioSessao repositorioSessao,
        IUnitOfWork unitOfWork,
        ILogger<SessaoAppService> logger)
    {
        this.tenantProvider = tenantProvider;
        this.repositorioSessao = repositorioSessao;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public Result Cadastrar(Sessao sessao)
    {
        List<string> erros = new List<string>();

        if (sessao.NumeroMaximoIngressos > sessao.Sala.Capacidade)
            erros.Add("O número máximo de ingressos não pode exceder a capacidade da sala.");

        var novaSessaoInicio = sessao.Inicio;
        var novoSessaoFim = sessao.Inicio.AddMinutes(Convert.ToDouble(sessao.Filme.Duracao));

        // evitar duplicidade de sessão por sala/horário
        var duplicada = repositorioSessao.SelecionarRegistros()
            .Any(s => s.Sala.Id.Equals(sessao.Sala.Id) && s.Inicio.Date.Equals(sessao.Inicio.Date) &&
                novaSessaoInicio < s.Inicio.AddMinutes(Convert.ToDouble(s.Filme.Duracao)) &&
                s.Inicio < novoSessaoFim
            );

        if (duplicada)
            erros.Add("Já existe uma sessão nesta sala para o mesmo horário.");

        if (erros.Any())
            return Result.Fail(erros.Select(ResultadosErro.RegistroDuplicadoErro).ToList());

        try
        {
            sessao.UsuarioId = tenantProvider.UsuarioId.GetValueOrDefault();

            repositorioSessao.Cadastrar(sessao);

            unitOfWork.Commit();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(ex, "Erro ao cadastrar sessão {@Sessao}", sessao);

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result Editar(Guid id, Sessao sessaoEditada)
    {
        List<string> erros = new List<string>();

        if (sessaoEditada.NumeroMaximoIngressos > sessaoEditada.Sala.Capacidade)
            erros.Add("O número máximo de ingressos não pode exceder a capacidade da sala.");

        var novaSessaoInicio = sessaoEditada.Inicio;
        var novoSessaoFim = sessaoEditada.Inicio.AddMinutes(Convert.ToDouble(sessaoEditada.Filme.Duracao));

        // evitar duplicidade de sessão por sala/horário
        var duplicada = repositorioSessao.SelecionarRegistros()
            .Any(s => !s.Id.Equals(id) &&
                s.Sala.Id.Equals(sessaoEditada.Sala.Id) && s.Inicio.Date.Equals(sessaoEditada.Inicio.Date) &&
                novaSessaoInicio < s.Inicio.AddMinutes(Convert.ToDouble(s.Filme.Duracao)) &&
                s.Inicio < novoSessaoFim
            );

        if (duplicada)
            erros.Add("Já existe uma sessão nesta sala para o mesmo horário.");

        if (erros.Any())
            return Result.Fail(erros.Select(ResultadosErro.RegistroDuplicadoErro).ToList());

        try
        {
            var ok = repositorioSessao.Editar(id, sessaoEditada);

            if (!ok)
                return Result.Fail(ResultadosErro.RegistroNaoEncontradoErro(id));

            unitOfWork.Commit();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(ex, "Erro ao editar sessão {@Sessao}", sessaoEditada);

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result Excluir(Guid id)
    {
        try
        {
            var exclusaoConcluida = repositorioSessao.Excluir(id);

            if (!exclusaoConcluida)
                return Result.Fail(ResultadosErro.RegistroNaoEncontradoErro(id));

            unitOfWork.Commit();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(ex, "Erro ao excluir sessão {Id}", id);

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result<Sessao> SelecionarPorId(Guid id)
    {
        try
        {
            var sessao = repositorioSessao.SelecionarRegistroPorId(id);

            if (sessao is null)
                return Result.Fail(ResultadosErro.RegistroNaoEncontradoErro(id));

            return Result.Ok(sessao);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao selecionar sessão {Id}", id);

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result<List<Sessao>> SelecionarTodos()
    {
        try
        {
            List<Sessao> registros = new List<Sessao>();

            if (tenantProvider.IsInRole("Empresa"))
                registros = repositorioSessao.SelecionarRegistrosDoUsuario(tenantProvider.UsuarioId.GetValueOrDefault());

            else if (tenantProvider.IsInRole("Cliente"))
                registros = repositorioSessao.SelecionarRegistros();

            return Result.Ok(registros);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao selecionar sessões");

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result Encerrar(Guid id)
    {
        try
        {
            var sessao = repositorioSessao.SelecionarRegistroPorId(id);

            if (sessao is null)
                return Result.Fail(ResultadosErro.RegistroNaoEncontradoErro(id));

            sessao.Encerrar();

            unitOfWork.Commit();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(ex, "Erro ao encerrar sessão {Id}", id);

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result<Ingresso> VenderIngresso(Guid sessaoId, int assento, bool meiaEntrada)
    {
        try
        {
            var sessao = repositorioSessao.SelecionarRegistroPorId(sessaoId);

            if (sessao is null)
                return Result.Fail(ResultadosErro.RegistroNaoEncontradoErro(sessaoId));

            if (sessao.Encerrada)
                return Result.Fail(new Error("A sessão já foi encerrada."));

            if (assento < 1 || assento > sessao.NumeroMaximoIngressos)
                return Result.Fail(new Error("Assento inválido para esta sessão."));

            if (sessao.Ingressos.Any(i => i.NumeroAssento == assento))
                return Result.Fail(new Error("Este assento já está ocupado."));

            if (sessao.ObterQuantidadeIngressosDisponiveis() <= 0)
                return Result.Fail(new Error("Sessão lotada."));

            var ingresso = sessao.GerarIngresso(assento, meiaEntrada);

            ingresso.UsuarioId = tenantProvider.UsuarioId.GetValueOrDefault();

            unitOfWork.Commit();

            return Result.Ok(ingresso);
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(ex, "Erro ao vender ingresso: sessão {SessaoId}, assento {Assento}", sessaoId, assento);

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }
}
