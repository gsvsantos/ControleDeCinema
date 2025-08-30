using ControleDeCinema.Aplicacao.Compartilhado;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using FluentResults;
using Microsoft.AspNetCore.Identity;

namespace ControleDeCinema.Aplicacao.ModuloAutenticacao;

public class AutenticacaoAppService
{
    private readonly UserManager<Usuario> userManager;
    private readonly SignInManager<Usuario> signInManager;
    private readonly RoleManager<Cargo> roleManager;

    public AutenticacaoAppService(
        UserManager<Usuario> userManager,
        SignInManager<Usuario> signInManager,
        RoleManager<Cargo> roleManager
    )
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.roleManager = roleManager;
    }

    public async Task<Result> RegistrarAsync(Usuario usuario, string senha, TipoUsuario tipo)
    {
        var usuarioResult = await userManager.CreateAsync(usuario, senha);

        if (!usuarioResult.Succeeded)
        {
            var erros = usuarioResult.Errors.Select(err =>
            {
                return err.Code switch
                {
                    "DuplicateUserName" => "Já existe um usuário com esse nome.",
                    "DuplicateEmail" => "Já existe um usuário com esse e-mail.",
                    "PasswordTooShort" => "A senha é muito curta.",
                    "PasswordRequiresNonAlphanumeric" => "A senha deve conter pelo menos um caractere especial.",
                    "PasswordRequiresDigit" => "A senha deve conter pelo menos um número.",
                    "PasswordRequiresUpper" => "A senha deve conter pelo menos uma letra maiúscula.",
                    "PasswordRequiresLower" => "A senha deve conter pelo menos uma letra minúscula.",
                    _ => err.Description
                };
            }).ToList();

            return Result.Fail(ResultadosErro.RequisicaoInvalidaErro(erros));
        }

        var tipoString = tipo.ToString();

        var resultadoBuscaCargo = await roleManager.FindByNameAsync(tipoString);

        if (resultadoBuscaCargo is null)
        {
            var cargo = new Cargo
            {
                Name = tipoString,
                NormalizedName = tipoString.ToUpper(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            await roleManager.CreateAsync(cargo);
        }

        await userManager.AddToRoleAsync(usuario, tipoString);

        return await LoginAsync(
            usuario.Email ?? string.Empty,
            senha
        );
    }

    public async Task<Result> LoginAsync(string email, string senha)
    {
        var resultadoLogin = await signInManager.PasswordSignInAsync(
            email,
            senha,
            isPersistent: true,
            lockoutOnFailure: false
        );

        if (resultadoLogin.Succeeded)
            return Result.Ok();

        if (resultadoLogin.IsLockedOut)
            return Result.Fail(ResultadosErro
                .RequisicaoInvalidaErro("Sua conta foi bloqueada temporariamente devido a muitas tentativas inválidas."));

        if (resultadoLogin.IsNotAllowed)
            return Result.Fail(ResultadosErro
                .RequisicaoInvalidaErro("Não é permitido efetuar login. Verifique se sua conta está confirmada."));

        if (resultadoLogin.RequiresTwoFactor)
            return Result.Fail(ResultadosErro
                .RequisicaoInvalidaErro("É necessário confirmar o login com autenticação de dois fatores."));

        return Result.Fail(ResultadosErro
            .RequisicaoInvalidaErro("Login ou senha incorretos."));
    }

    public async Task<Result> LogoutAsync()
    {
        await signInManager.SignOutAsync();

        return Result.Ok();
    }
}
