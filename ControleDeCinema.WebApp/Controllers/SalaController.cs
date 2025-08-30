using ControleDeCinema.Aplicacao.ModuloSala;
using ControleDeCinema.WebApp.Extensions;
using ControleDeCinema.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeCinema.WebApp.Controllers;

[Route("salas")]
[Authorize(Roles = "Empresa")]
public class SalaController : Controller
{
    private readonly SalaAppService salaAppService;

    public SalaController(SalaAppService salaAppService)
    {
        this.salaAppService = salaAppService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var resultado = salaAppService.SelecionarTodos();

        if (resultado.IsFailed)
            return this.RedirecionarParaNotificacaoHome(resultado.ToResult());

        var visualizarVM = new VisualizarSalasViewModel(resultado.Value);

        this.ObterNotificacaoPendente();

        return View(visualizarVM);
    }

    [HttpGet("cadastrar")]
    public IActionResult Cadastrar()
    {
        var cadastrarVM = new CadastrarSalaViewModel();

        return View(cadastrarVM);
    }

    [HttpPost("cadastrar")]
    [ValidateAntiForgeryToken]
    public IActionResult Cadastrar(CadastrarSalaViewModel cadastrarVM)
    {
        var entidade = FormularioSalaViewModel.ParaEntidade(cadastrarVM);

        var resultado = salaAppService.Cadastrar(entidade);

        if (resultado.IsFailed)
            return this.PreencherErrosModelState(resultado, cadastrarVM);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("editar/{id:guid}")]
    public IActionResult Editar(Guid id)
    {
        var resultado = salaAppService.SelecionarPorId(id);

        if (resultado.IsFailed)
            return this.RedirecionarParaNotificacao(resultado.ToResult());

        var editarVM = new EditarSalaViewModel(
            id,
            resultado.Value.Numero,
            resultado.Value.Capacidade
        );

        return View(editarVM);
    }

    [HttpPost("editar/{id:guid}")]
    [ValidateAntiForgeryToken]
    public IActionResult Editar(Guid id, EditarSalaViewModel editarVM)
    {
        var entidadeEditada = FormularioSalaViewModel.ParaEntidade(editarVM);

        var resultado = salaAppService.Editar(id, entidadeEditada);

        if (resultado.IsFailed)
            return this.PreencherErrosModelState(resultado, editarVM);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("excluir/{id:guid}")]
    public IActionResult Excluir(Guid id)
    {
        var resultado = salaAppService.SelecionarPorId(id);

        if (resultado.IsFailed)
            return this.RedirecionarParaNotificacao(resultado.ToResult());

        var excluirVM = new ExcluirSalaViewModel(
            resultado.Value.Id,
            resultado.Value.Numero
        );

        return View(excluirVM);
    }

    [HttpPost("excluir/{id:guid}")]
    [ValidateAntiForgeryToken]
    public IActionResult Excluir(ExcluirSalaViewModel excluirVm)
    {
        var resultado = salaAppService.Excluir(excluirVm.Id);

        if (resultado.IsFailed)
            return this.RedirecionarParaNotificacao(resultado);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("detalhes/{id:guid}")]
    public IActionResult Detalhes(Guid id)
    {
        var resultado = salaAppService.SelecionarPorId(id);

        if (resultado.IsFailed)
            return this.RedirecionarParaNotificacao(resultado.ToResult());

        var detalhesVm = DetalhesSalaViewModel.ParaDetalhesVm(resultado.Value);

        return View(detalhesVm);
    }
}
