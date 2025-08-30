using ControleDeCinema.Aplicacao.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace ControleDeCinema.Testes.Unidades.ModuloAutenticacao;

[TestClass]
[TestCategory("Testes de Unidade de AutenticacaoAppService")]
public class AutenticacaoAppServiceTestes
{
    // SUT
    private AutenticacaoAppService autenticacaoAppService;

    // TDB
    private const string emailPadrao = "emailTeste@teste.com";
    private const string senhaPadrao = "Teste123!";
    private const string userNamePadrao = emailPadrao;
    private const TipoUsuario tipoUsuarioPadrao = TipoUsuario.Cliente;
    private readonly string tipoUsuarioPadraoString = tipoUsuarioPadrao.ToString();
    private readonly Cargo cargoPadrao = new()
    { Name = TipoUsuario.Cliente.ToString(), NormalizedName = tipoUsuarioPadrao.ToString().ToUpper(), ConcurrencyStamp = Guid.NewGuid().ToString() };

    // MOCK
    private Mock<UserManager<Usuario>> userManagerMock;
    private Mock<SignInManager<Usuario>> signInManagerMock;
    private Mock<RoleManager<Cargo>> roleManagerMock;

    [TestInitialize]
    public void Setup()
    {
        userManagerMock = new Mock<UserManager<Usuario>>(
            new Mock<IUserStore<Usuario>>().Object, null!, null!, null!,
            null!, null!, null!, null!, null!
        );


        signInManagerMock = new Mock<SignInManager<Usuario>>(
            userManagerMock.Object, new Mock<IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<Usuario>>().Object, null!,
            null!, null!
        );

        roleManagerMock = new Mock<RoleManager<Cargo>>(
            new Mock<IRoleStore<Cargo>>().Object, null!, null!, null!, null!
        );

        autenticacaoAppService = new AutenticacaoAppService(
            userManagerMock.Object,
            signInManagerMock.Object,
            roleManagerMock.Object
        );
    }

    #region Testes Registro
    [TestMethod]
    public async Task Registrar_Deve_Retornar_Sucesso()
    {
        // Arrange
        Usuario novoUsuario = new()
        {
            UserName = userNamePadrao,
            Email = emailPadrao
        };

        userManagerMock
            .Setup(u => u.CreateAsync(novoUsuario, senhaPadrao))
            .ReturnsAsync(IdentityResult.Success);

        roleManagerMock
            .Setup(r => r.FindByNameAsync(tipoUsuarioPadraoString))
            .ReturnsAsync(cargoPadrao);

        userManagerMock
            .Setup(u => u.AddToRoleAsync(novoUsuario, tipoUsuarioPadraoString))
            .ReturnsAsync(IdentityResult.Success);

        signInManagerMock
            .Setup(s => s.PasswordSignInAsync(novoUsuario.Email, senhaPadrao, true, false))
            .ReturnsAsync(SignInResult.Success);

        // Act
        Result? resultadoRegistro = await autenticacaoAppService.RegistrarAsync(novoUsuario, senhaPadrao, tipoUsuarioPadrao);

        // Assert
        userManagerMock.Verify(u => u.CreateAsync(novoUsuario, senhaPadrao), Times.Once);
        roleManagerMock.Verify(r => r.FindByNameAsync(tipoUsuarioPadraoString), Times.Once);
        userManagerMock.Verify(u => u.AddToRoleAsync(novoUsuario, tipoUsuarioPadraoString), Times.Once);
        signInManagerMock.Verify(s => s.PasswordSignInAsync(novoUsuario.Email, senhaPadrao, true, false), Times.Once);

        Assert.IsNotNull(resultadoRegistro);
        Assert.IsTrue(resultadoRegistro.IsSuccess);
    }

    [TestMethod]
    public async Task Registrar_Com_Usuario_Duplicado_Deve_Retornar_Falha()
    {
        // Arrange 
        Usuario novoUsuario = new()
        {
            UserName = userNamePadrao,
            Email = "outroemail@teste.com"
        };

        IdentityError userDuplicateError = new()
        {
            Code = "DuplicateUserName",
            Description = "Já existe um usuário com esse nome."
        };

        userManagerMock
            .Setup(u => u.CreateAsync(novoUsuario, senhaPadrao))
            .ReturnsAsync(IdentityResult.Failed(userDuplicateError));

        // Act
        Result? resultadoRegistro = await autenticacaoAppService.RegistrarAsync(novoUsuario, senhaPadrao, tipoUsuarioPadrao);

        // Assert
        userManagerMock.Verify(u => u.CreateAsync(novoUsuario, senhaPadrao), Times.Once);
        roleManagerMock.Verify(r => r.FindByNameAsync(tipoUsuarioPadraoString), Times.Never);
        userManagerMock.Verify(u => u.AddToRoleAsync(novoUsuario, tipoUsuarioPadraoString), Times.Never);
        signInManagerMock.Verify(s => s.PasswordSignInAsync(novoUsuario.Email, senhaPadrao, true, false), Times.Never);

        string mensagemEsperada = MensagensErroAutenticacao.UsuarioJaExiste;
        List<string> mensagemDoResult = resultadoRegistro.Errors
            .SelectMany(e => e.Reasons.OfType<Error>())
            .Select(r => r.Message)
            .ToList();

        Assert.IsNotNull(resultadoRegistro);
        Assert.IsTrue(resultadoRegistro.IsFailed);
        Assert.AreEqual("Requisição inválida", resultadoRegistro.Errors[0].Message);
        Assert.IsNotNull(mensagemDoResult);
        Assert.IsTrue(mensagemDoResult.Contains(mensagemEsperada));
    }

    [TestMethod]
    public async Task Registrar_Com_Email_Duplicado_Deve_Retornar_Falha()
    {
        // Arrange
        Usuario novoUsuario = new()
        {
            UserName = "outroUserName",
            Email = emailPadrao
        };

        IdentityError emailDuplicateError = new()
        {
            Code = "DuplicateEmail",
            Description = "Já existe um usuário com esse e-mail."
        };

        userManagerMock
            .Setup(u => u.CreateAsync(novoUsuario, senhaPadrao))
            .ReturnsAsync(IdentityResult.Failed(emailDuplicateError));

        // Act
        Result? resultadoRegistro = await autenticacaoAppService.RegistrarAsync(novoUsuario, senhaPadrao, tipoUsuarioPadrao);

        // Assert
        userManagerMock.Verify(u => u.CreateAsync(novoUsuario, senhaPadrao), Times.Once);
        roleManagerMock.Verify(r => r.FindByNameAsync(tipoUsuarioPadraoString), Times.Never);
        userManagerMock.Verify(u => u.AddToRoleAsync(novoUsuario, tipoUsuarioPadraoString), Times.Never);
        signInManagerMock.Verify(s => s.PasswordSignInAsync(novoUsuario.Email, senhaPadrao, true, false), Times.Never);

        string mensagemEsperada = MensagensErroAutenticacao.EmailJaExiste;
        string? mensagemDoResult = resultadoRegistro.Errors.SelectMany(e => e.Reasons.OfType<Error>())
            .Select(r => r.Message).FirstOrDefault();

        Assert.IsNotNull(resultadoRegistro);
        Assert.IsTrue(resultadoRegistro.IsFailed);
        Assert.AreEqual("Requisição inválida", resultadoRegistro.Errors[0].Message);
        Assert.IsNotNull(mensagemDoResult);
        Assert.AreEqual(mensagemEsperada, mensagemDoResult);
    }

    [TestMethod]
    public async Task Registrar_Com_Senha_Invalida_Deve_Retornar_Falha()
    {
        // Arrange
        Usuario novoUsuario = new()
        {
            UserName = "outroUserName",
            Email = emailPadrao
        };

        IdentityError[] passwordErrors =
        {
            new() { Code = "PasswordTooShort", Description = "A senha é muito curta." },
            new() { Code = "PasswordRequiresNonAlphanumeric", Description = "A senha deve conter pelo menos um caractere especial." },
            new() { Code = "PasswordRequiresDigit", Description = "A senha deve conter pelo menos um número." },
            new() { Code = "PasswordRequiresUpper", Description = "A senha deve conter pelo menos uma letra maiúscula." },
            new() { Code = "PasswordRequiresLower", Description = "A senha deve conter pelo menos uma letra minúscula." }
        };

        userManagerMock
            .Setup(u => u.CreateAsync(novoUsuario, senhaPadrao))
            .ReturnsAsync(IdentityResult.Failed(passwordErrors));

        // Act
        Result? resultadoRegistro = await autenticacaoAppService.RegistrarAsync(novoUsuario, senhaPadrao, tipoUsuarioPadrao);

        // Assert
        userManagerMock.Verify(u => u.CreateAsync(novoUsuario, senhaPadrao), Times.Once);
        roleManagerMock.Verify(r => r.FindByNameAsync(tipoUsuarioPadraoString), Times.Never);
        userManagerMock.Verify(u => u.AddToRoleAsync(novoUsuario, tipoUsuarioPadraoString), Times.Never);
        signInManagerMock.Verify(s => s.PasswordSignInAsync(novoUsuario.Email, senhaPadrao, true, false), Times.Never);

        string[] mensagensEsperadas =
        {
            MensagensErroAutenticacao.SenhaMuitoCurta,
            MensagensErroAutenticacao.SenhaRequerCaracterEspecial,
            MensagensErroAutenticacao.SenhaRequerNumero,
            MensagensErroAutenticacao.SenhaRequerMaiuscula,
            MensagensErroAutenticacao.SenhaRequerMinuscula
        };
        List<string> mensagemDoResult = resultadoRegistro.Errors
            .SelectMany(e => e.Reasons.OfType<Error>())
            .Select(r => r.Message)
            .ToList();

        Assert.IsNotNull(resultadoRegistro);
        Assert.IsTrue(resultadoRegistro.IsFailed);
        Assert.AreEqual("Requisição inválida", resultadoRegistro.Errors[0].Message);
        Assert.IsNotNull(mensagemDoResult);
        CollectionAssert.AreEquivalent(mensagensEsperadas, mensagemDoResult);
    }

    [TestMethod]
    public async Task Registrar_Cria_Cargo_Quando_Inexistente_E_Atribui_Ao_Usuario()
    {
        // Arrange
        Usuario novoUsuario = new()
        {
            UserName = userNamePadrao,
            Email = emailPadrao
        };

        userManagerMock
            .Setup(u => u.CreateAsync(novoUsuario, senhaPadrao))
            .ReturnsAsync(IdentityResult.Success);

        roleManagerMock
            .Setup(r => r.FindByNameAsync(tipoUsuarioPadraoString))
            .ReturnsAsync((Cargo?)null!);

        roleManagerMock
            .Setup(r => r.CreateAsync(It.IsAny<Cargo>()))
            .ReturnsAsync(IdentityResult.Success);

        userManagerMock
            .Setup(u => u.AddToRoleAsync(novoUsuario, tipoUsuarioPadraoString))
            .ReturnsAsync(IdentityResult.Success);

        signInManagerMock
            .Setup(s => s.PasswordSignInAsync(novoUsuario.Email, senhaPadrao, true, false))
            .ReturnsAsync(SignInResult.Success);

        // Act
        Result? resultadoRegistro = await autenticacaoAppService.RegistrarAsync(novoUsuario, senhaPadrao, tipoUsuarioPadrao);

        // Assert
        userManagerMock.Verify(u => u.CreateAsync(novoUsuario, senhaPadrao), Times.Once);
        roleManagerMock.Verify(r => r.FindByNameAsync(tipoUsuarioPadraoString), Times.Once);
        roleManagerMock.Verify(r => r.CreateAsync(It.Is<Cargo>(c => c.Name == tipoUsuarioPadraoString &&
            c.NormalizedName == tipoUsuarioPadraoString.ToUpper())), Times.Once);
        userManagerMock.Verify(u => u.AddToRoleAsync(novoUsuario, tipoUsuarioPadraoString), Times.Once);
        signInManagerMock.Verify(s => s.PasswordSignInAsync(novoUsuario.Email, senhaPadrao, true, false), Times.Once);

        Assert.IsNotNull(resultadoRegistro);
        Assert.IsTrue(resultadoRegistro.IsSuccess);
    }

    [TestMethod]
    public async Task Registrar_Chama_Login_Apos_Criar_Usuario()
    {
        // Arrange
        Usuario novoUsuario = new()
        {
            UserName = userNamePadrao,
            Email = emailPadrao
        };

        userManagerMock
            .Setup(u => u.CreateAsync(novoUsuario, senhaPadrao))
            .ReturnsAsync(IdentityResult.Success);

        roleManagerMock
            .Setup(r => r.FindByNameAsync(tipoUsuarioPadraoString))
            .ReturnsAsync(cargoPadrao);

        userManagerMock
            .Setup(u => u.AddToRoleAsync(novoUsuario, tipoUsuarioPadraoString))
            .ReturnsAsync(IdentityResult.Success);

        signInManagerMock
            .Setup(s => s.PasswordSignInAsync(novoUsuario.Email, senhaPadrao, true, false))
            .ReturnsAsync(SignInResult.Success);

        // Act
        Result? resultadoRegistro = await autenticacaoAppService.RegistrarAsync(novoUsuario, senhaPadrao, tipoUsuarioPadrao);

        // Assert
        userManagerMock.Verify(u => u.CreateAsync(novoUsuario, senhaPadrao), Times.Once);
        roleManagerMock.Verify(r => r.FindByNameAsync(tipoUsuarioPadraoString), Times.Once);
        userManagerMock.Verify(u => u.AddToRoleAsync(novoUsuario, tipoUsuarioPadraoString), Times.Once);
        signInManagerMock.Verify(s => s.PasswordSignInAsync(novoUsuario.Email, senhaPadrao, true, false), Times.Once);

        Assert.IsNotNull(resultadoRegistro);
        Assert.IsTrue(resultadoRegistro.IsSuccess);
    }
    #endregion

    #region Testes Login
    [TestMethod]
    public async Task Login_Deve_Retornar_Sucesso()
    {
        // Arrange
        signInManagerMock
            .Setup(p => p.PasswordSignInAsync(emailPadrao, senhaPadrao, true, false))
            .ReturnsAsync(SignInResult.Success);

        // Act
        Result? resultadoLogin = await autenticacaoAppService.LoginAsync(emailPadrao, senhaPadrao);

        // Assert
        Assert.IsNotNull(resultadoLogin);
        Assert.IsTrue(resultadoLogin.IsSuccess);
    }

    [TestMethod]
    public async Task Login_Com_Conta_Bloqueada_Deve_Retornar_Falha()
    {
        // Arrange
        signInManagerMock
            .Setup(p => p.PasswordSignInAsync(emailPadrao, senhaPadrao, true, false))
            .ReturnsAsync(SignInResult.LockedOut);

        // Act
        Result? resultadoLogin = await autenticacaoAppService.LoginAsync(emailPadrao, senhaPadrao);

        // Assert
        string mensagemEsperada = MensagensErroAutenticacao.ContaBloqueada;
        string? mensagemDoResult = resultadoLogin.Errors.SelectMany(e => e.Reasons.OfType<Error>())
            .Select(r => r.Message).FirstOrDefault();

        Assert.IsNotNull(resultadoLogin);
        Assert.IsTrue(resultadoLogin.IsFailed);
        Assert.AreEqual("Requisição inválida", resultadoLogin.Errors[0].Message);
        Assert.IsNotNull(mensagemDoResult);
        Assert.AreEqual(mensagemEsperada, mensagemDoResult);
    }

    [TestMethod]
    public async Task Login_Nao_Permitido_Deve_Retornar_Falha()
    {
        // Arrange
        signInManagerMock
            .Setup(p => p.PasswordSignInAsync(emailPadrao, senhaPadrao, true, false))
            .ReturnsAsync(SignInResult.NotAllowed);

        // Act
        Result? resultadoLogin = await autenticacaoAppService.LoginAsync(emailPadrao, senhaPadrao);

        // Assert
        string mensagemEsperada = MensagensErroAutenticacao.LoginNaoPermitido;
        string? mensagemDoResult = resultadoLogin.Errors.SelectMany(e => e.Reasons.OfType<Error>())
            .Select(r => r.Message).FirstOrDefault();

        Assert.IsNotNull(resultadoLogin);
        Assert.IsTrue(resultadoLogin.IsFailed);
        Assert.AreEqual("Requisição inválida", resultadoLogin.Errors[0].Message);
        Assert.IsNotNull(mensagemDoResult);
        Assert.AreEqual(mensagemEsperada, mensagemDoResult);
    }

    [TestMethod]
    public async Task Login_Requer_Dois_Fatores_Deve_Retornar_Falha()
    {
        // Arrange
        signInManagerMock
            .Setup(p => p.PasswordSignInAsync(emailPadrao, senhaPadrao, true, false))
            .ReturnsAsync(SignInResult.TwoFactorRequired);

        // Act
        Result? resultadoLogin = await autenticacaoAppService.LoginAsync(emailPadrao, senhaPadrao);

        // Assert
        string mensagemEsperada = MensagensErroAutenticacao.RequerAutenticacaoDoisFatores;
        string? mensagemDoResult = resultadoLogin.Errors.SelectMany(e => e.Reasons.OfType<Error>())
            .Select(r => r.Message).FirstOrDefault();

        Assert.IsNotNull(resultadoLogin);
        Assert.IsTrue(resultadoLogin.IsFailed);
        Assert.AreEqual("Requisição inválida", resultadoLogin.Errors[0].Message);
        Assert.IsNotNull(mensagemDoResult);
        Assert.AreEqual(mensagemEsperada, mensagemDoResult);
    }

    [TestMethod]
    public async Task Login_Com_Credenciais_Invalidas_Deve_Retornar_Falha()
    {
        // Arrange
        signInManagerMock
            .Setup(p => p.PasswordSignInAsync(emailPadrao, senhaPadrao, true, false))
            .ReturnsAsync(SignInResult.Failed);

        // Act
        Result? resultadoLogin = await autenticacaoAppService.LoginAsync(emailPadrao, senhaPadrao);

        // Assert
        string mensagemEsperada = MensagensErroAutenticacao.DadosInvalidos;
        string? mensagemDoResult = resultadoLogin.Errors.SelectMany(e => e.Reasons.OfType<Error>())
            .Select(r => r.Message).FirstOrDefault();

        Assert.IsNotNull(resultadoLogin);
        Assert.IsTrue(resultadoLogin.IsFailed);
        Assert.AreEqual("Requisição inválida", resultadoLogin.Errors[0].Message);
        Assert.IsNotNull(mensagemDoResult);
        Assert.AreEqual(mensagemEsperada, mensagemDoResult);
    }
    #endregion

    #region Testes Logout
    [TestMethod]
    public async Task Logout_Deve_Retornar_Sucesso()
    {
        // Arrange
        signInManagerMock
            .Setup(p => p.SignOutAsync())
            .Returns(Task.CompletedTask);

        // Act
        Result resultadoLogout = await autenticacaoAppService.LogoutAsync();

        // Assert
        signInManagerMock.Verify(s => s.SignOutAsync(), Times.Once);

        Assert.IsNotNull(resultadoLogout);
        Assert.IsTrue(resultadoLogout.IsSuccess);
    }
    #endregion
}
