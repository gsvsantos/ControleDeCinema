using ControleDeCinema.Aplicacao.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.WebApp.Extensions;
using ControleDeCinema.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeCinema.WebApp.Controllers;

[Route("autenticacao")]
public class AutenticacaoController : Controller
{
    private readonly AutenticacaoAppService autenticacaoAppService;

    public AutenticacaoController(AutenticacaoAppService autenticacaoAppService)
    {
        this.autenticacaoAppService = autenticacaoAppService;
    }

    [HttpGet("registro")]
    public IActionResult Registro()
    {
        if (User.Identity?.IsAuthenticated ?? false)
            return RedirectToAction("Index", "Home");

        var registroVm = new RegistroViewModel();

        return View(registroVm);
    }

    [HttpPost("registro")]
    public async Task<IActionResult> Registro(RegistroViewModel registroVm)
    {
        var usuario = new Usuario()
        {
            UserName = registroVm.Email,
            Email = registroVm.Email,
        };

        var resultado = await autenticacaoAppService.RegistrarAsync(
            usuario,
            registroVm.Senha ?? string.Empty,
            registroVm.Tipo
        );

        if (resultado.IsFailed)
            return this.PreencherErrosModelState(resultado, registroVm);

        return RedirectToAction(nameof(HomeController.Index), "Home");
    }

    [HttpGet("login")]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated ?? false)
            return RedirectToAction(nameof(HomeController.Index), "Home");

        var loginVm = new LoginViewModel();

        ViewData["ReturnUrl"] = returnUrl;

        return View(loginVm);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginViewModel loginVm, string? returnUrl = null)
    {
        var resultado = await autenticacaoAppService.LoginAsync(
            loginVm.Email ?? string.Empty,
            loginVm.Senha ?? string.Empty
        );

        if (resultado.IsFailed)
            return this.PreencherErrosModelState(resultado, loginVm);

        if (Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);

        return RedirectToAction(nameof(HomeController.Index), "Home");
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await autenticacaoAppService.LogoutAsync();

        return RedirectToAction(nameof(Login));
    }
}
