using ControleDeCinema.Aplicacao.ModuloSessao;
using ControleDeCinema.WebApp.Extensions;
using ControleDeCinema.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeCinema.WebApp.Controllers;

[Route("ingressos")]
[Authorize(Roles = "Cliente")]
public class IngressoController : Controller
{
    private readonly SessaoAppService sessaoAppService;

    public IngressoController(SessaoAppService sessaoAppService)
    {
        this.sessaoAppService = sessaoAppService;
    }

    [HttpGet("comprar-ingresso/sessao/{sessaoId:guid}")]
    public IActionResult ComprarIngresso(Guid sessaoId)
    {
        var resultadoSessao = sessaoAppService.SelecionarPorId(sessaoId);

        if (resultadoSessao.IsFailed)
            return RedirectToAction(nameof(SessaoController.Index), "Sessao");

        var comprarIngressoVm = new ComprarIngressoViewModel(resultadoSessao.Value);

        return View(comprarIngressoVm);
    }

    [HttpPost("comprar-ingresso/sessao/{sessaoId:guid}")]
    [ValidateAntiForgeryToken]
    public IActionResult ComprarIngresso(ComprarIngressoViewModel venderIngressoVm)
    {
        var resultado = sessaoAppService.VenderIngresso(
            venderIngressoVm.SessaoId,
            venderIngressoVm.Assento,
            venderIngressoVm.MeiaEntrada
        );

        if (resultado.IsFailed)
            return this.RedirecionarParaNotificacao(resultado.ToResult());

        return RedirectToAction(nameof(SessaoController.Index), "Sessao");
    }
}
