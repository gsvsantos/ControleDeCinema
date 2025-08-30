using ControleDeCinema.Aplicacao.ModuloFilme;
using ControleDeCinema.Aplicacao.ModuloGeneroFilme;
using ControleDeCinema.WebApp.Extensions;
using ControleDeCinema.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeCinema.WebApp.Controllers;

[Route("filmes")]
[Authorize(Roles = "Empresa")]
public class FilmeController : Controller
{
    private readonly FilmeAppService filmeAppService;
    private readonly GeneroFilmeAppService generoFilmeAppService;

    public FilmeController(FilmeAppService filmeAppService, GeneroFilmeAppService generoFilmeAppService)
    {
        this.filmeAppService = filmeAppService;
        this.generoFilmeAppService = generoFilmeAppService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var resultado = filmeAppService.SelecionarTodos();

        if (resultado.IsFailed)
            return this.RedirecionarParaNotificacaoHome(resultado.ToResult());

        var visualizarVM = new VisualizarFilmesViewModel(resultado.Value);

        this.ObterNotificacaoPendente();

        return View(visualizarVM);
    }

    [HttpGet("cadastrar")]
    public IActionResult Cadastrar()
    {
        var generosDisponiveis = generoFilmeAppService.SelecionarTodos().ValueOrDefault;

        var cadastrarVM = new CadastrarFilmeViewModel(generosDisponiveis);

        return View(cadastrarVM);
    }

    [HttpPost("cadastrar")]
    [ValidateAntiForgeryToken]
    public IActionResult Cadastrar(CadastrarFilmeViewModel cadastrarVM)
    {
        var generosDisponiveis = generoFilmeAppService.SelecionarTodos().ValueOrDefault;

        var entidade = FormularioFilmeViewModel.ParaEntidade(cadastrarVM, generosDisponiveis);

        var resultado = filmeAppService.Cadastrar(entidade);

        if (resultado.IsFailed)
            return this.PreencherErrosModelState(resultado, cadastrarVM);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("editar/{id:guid}")]
    public IActionResult Editar(Guid id)
    {
        var resultado = filmeAppService.SelecionarPorId(id);

        if (resultado.IsFailed)
            return this.RedirecionarParaNotificacao(resultado.ToResult());

        var generosDisponiveis = generoFilmeAppService.SelecionarTodos().ValueOrDefault;

        var editarVM = new EditarFilmeViewModel(
            id,
            resultado.Value.Titulo,
            resultado.Value.Duracao,
            resultado.Value.Lancamento,
            resultado.Value.Genero.Id,
            generosDisponiveis
        );

        return View(editarVM);
    }

    [HttpPost("editar/{id:guid}")]
    [ValidateAntiForgeryToken]
    public IActionResult Editar(Guid id, EditarFilmeViewModel editarVM)
    {
        var generosDisponiveis = generoFilmeAppService.SelecionarTodos().ValueOrDefault;

        var entidadeEditada = FormularioFilmeViewModel.ParaEntidade(editarVM, generosDisponiveis);

        var resultado = filmeAppService.Editar(id, entidadeEditada);

        if (resultado.IsFailed)
            return this.PreencherErrosModelState(resultado, editarVM);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("excluir/{id:guid}")]
    public IActionResult Excluir(Guid id)
    {
        var resultado = filmeAppService.SelecionarPorId(id);

        if (resultado.IsFailed)
            return this.RedirecionarParaNotificacao(resultado.ToResult());

        var excluirVM = new ExcluirFilmeViewModel(
            resultado.Value.Id,
            resultado.Value.Titulo
        );

        return View(excluirVM);
    }

    [HttpPost("excluir/{id:guid}")]
    [ValidateAntiForgeryToken]
    public IActionResult Excluir(ExcluirFilmeViewModel excluirVm)
    {
        var resultado = filmeAppService.Excluir(excluirVm.Id);

        if (resultado.IsFailed)
            return this.RedirecionarParaNotificacao(resultado);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("detalhes/{id:guid}")]
    public IActionResult Detalhes(Guid id)
    {
        var resultado = filmeAppService.SelecionarPorId(id);

        if (resultado.IsFailed)
            return this.RedirecionarParaNotificacao(resultado.ToResult());

        var detalhesVm = DetalhesFilmeViewModel.ParaDetalhesVm(resultado.Value);

        return View(detalhesVm);
    }
}
