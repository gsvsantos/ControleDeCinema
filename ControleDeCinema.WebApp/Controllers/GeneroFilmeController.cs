using ControleDeCinema.Aplicacao.ModuloGeneroFilme;
using ControleDeCinema.WebApp.Extensions;
using ControleDeCinema.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeCinema.WebApp.Controllers;

[Route("generos")]
[Authorize(Roles = "Empresa")]
public class GeneroFilmeController : Controller
{
    private readonly GeneroFilmeAppService generoFilmeAppService;

    public GeneroFilmeController(GeneroFilmeAppService generoFilmeAppService)
    {
        this.generoFilmeAppService = generoFilmeAppService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var resultado = generoFilmeAppService.SelecionarTodos();

        if (resultado.IsFailed)
            return this.RedirecionarParaNotificacaoHome(resultado.ToResult());

        var visualizarVM = new VisualizarGenerosFilmeViewModel(resultado.Value);

        this.ObterNotificacaoPendente();

        return View(visualizarVM);
    }

    [HttpGet("cadastrar")]
    public IActionResult Cadastrar()
    {
        var cadastrarVM = new CadastrarGeneroFilmeViewModel();

        return View(cadastrarVM);
    }

    [HttpPost("cadastrar")]
    [ValidateAntiForgeryToken]
    public IActionResult Cadastrar(CadastrarGeneroFilmeViewModel cadastrarVM)
    {
        var entidade = FormularioGeneroFilmeViewModel.ParaEntidade(cadastrarVM);

        var resultado = generoFilmeAppService.Cadastrar(entidade);

        if (resultado.IsFailed)
            return this.PreencherErrosModelState(resultado, cadastrarVM);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("editar/{id:guid}")]
    public IActionResult Editar(Guid id)
    {
        var resultado = generoFilmeAppService.SelecionarPorId(id);

        if (resultado.IsFailed)
            return this.RedirecionarParaNotificacao(resultado.ToResult());

        var editarVM = new EditarGeneroFilmeViewModel(
            id,
            resultado.Value.Descricao
        );

        return View(editarVM);
    }

    [HttpPost("editar/{id:guid}")]
    [ValidateAntiForgeryToken]
    public IActionResult Editar(Guid id, EditarGeneroFilmeViewModel editarVM)
    {
        var entidadeEditada = FormularioGeneroFilmeViewModel.ParaEntidade(editarVM);

        var resultado = generoFilmeAppService.Editar(id, entidadeEditada);

        if (resultado.IsFailed)
            return this.PreencherErrosModelState(resultado, editarVM);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("excluir/{id:guid}")]
    public IActionResult Excluir(Guid id)
    {
        var resultado = generoFilmeAppService.SelecionarPorId(id);

        if (resultado.IsFailed)
            return this.RedirecionarParaNotificacao(resultado.ToResult());

        var excluirVM = new ExcluirGeneroFilmeViewModel(
            resultado.Value.Id,
            resultado.Value.Descricao
        );

        return View(excluirVM);
    }

    [HttpPost("excluir/{id:guid}")]
    [ValidateAntiForgeryToken]
    public IActionResult Excluir(ExcluirGeneroFilmeViewModel excluirVm)
    {
        var resultado = generoFilmeAppService.Excluir(excluirVm.Id);

        if (resultado.IsFailed)
            return this.RedirecionarParaNotificacao(resultado);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("detalhes/{id:guid}")]
    public IActionResult Detalhes(Guid id)
    {
        var resultado = generoFilmeAppService.SelecionarPorId(id);

        if (resultado.IsFailed)
            return this.RedirecionarParaNotificacao(resultado.ToResult());

        var detalhesVm = DetalhesGeneroFilmeViewModel.ParaDetalhesVm(resultado.Value);

        return View(detalhesVm);
    }
}
