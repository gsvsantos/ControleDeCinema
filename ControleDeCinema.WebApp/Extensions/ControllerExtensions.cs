using ControleDeCinema.WebApp.Controllers;
using ControleDeCinema.WebApp.Models;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ControleDeCinema.WebApp.Extensions;

public static class ControllerExtensions
{
    public static void ObterNotificacaoPendente(this Controller controller)
    {
        var existeNotificacao = controller.TempData.TryGetValue(nameof(NotificacaoViewModel), out var valor);

        if (existeNotificacao && valor is string jsonString)
        {
            var notificacaoVm = JsonSerializer.Deserialize<NotificacaoViewModel>(jsonString);

            controller.ViewData.Add(nameof(NotificacaoViewModel), notificacaoVm);
        }
    }

    public static IActionResult RedirecionarParaNotificacao(this Controller controller, Result resultado)
    {
        foreach (var error in resultado.Errors)
        {
            var notificacaoJson = NotificacaoViewModel.GerarNotificacaoSerializada(
                error.Message,
                error.Reasons[0].Message
            );

            controller.TempData.Add(nameof(NotificacaoViewModel), notificacaoJson);
        }

        return controller.RedirectToAction("Index");
    }

    public static IActionResult RedirecionarParaNotificacaoHome(this Controller controller, Result resultado)
    {
        foreach (var error in resultado.Errors)
        {
            var notificacaoJson = NotificacaoViewModel.GerarNotificacaoSerializada(
                error.Message,
                error.Reasons[0].Message
            );

            controller.TempData.TryAdd(nameof(NotificacaoViewModel), notificacaoJson);
        }

        return controller.RedirectToAction(nameof(HomeController.Index), "Home");
    }

    public static IActionResult PreencherErrosModelState(this Controller controller, Result resultado, object viewModel)
    {
        foreach (var erro in resultado.Errors)
        {
            var chave = erro.Metadata.TryGetValue("TipoErro", out var tipo) ?
                tipo.ToString() ?? "ErroInesperado" : "ErroInesperado";

            foreach (var reason in erro.Reasons)
                controller.ModelState.AddModelError(chave, reason.Message);
        }

        return controller.View(viewModel);
    }
}
