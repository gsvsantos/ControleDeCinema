using ControleDeCinema.Testes.Interface.Compartilhado;

namespace ControleDeCinema.Testes.Interface.ModuloAutenticacao;

[TestClass]
[TestCategory("Testes de Interface de Autenticação")]
public sealed class AutenticacaoInterfaceTestes : TestFixture
{
    [TestMethod]
    public void Deve_Cadastrar_Empresa_Corretamente()
    {
        // Act
        AutenticacaoIndexPageObject autenticacaoIndex = new(driver);

        AutenticacaoFormPageObject autenticacaoForm = autenticacaoIndex
            .IrParaRegistro(enderecoBase);

        autenticacaoForm
            .PreencherEmail(emailEmpresa)
            .PreencherSenha(senhaPadrao)
            .PreencherConfirmarSenha(senhaPadrao)
            .SelecionarTipoUsuario("Empresa")
            .ClickSubmitRegistro();

        // Assert
        Assert.IsTrue(autenticacaoIndex.EstaLogado());
    }

    [TestMethod]
    public void Deve_Cadastrar_Cliente_Corretamente()
    {
        // Act
        AutenticacaoIndexPageObject autenticacaoIndex = new(driver);

        AutenticacaoFormPageObject autenticacaoForm = autenticacaoIndex
            .IrParaRegistro(enderecoBase);

        autenticacaoForm
            .PreencherEmail(emailCliente)
            .PreencherSenha(senhaPadrao)
            .PreencherConfirmarSenha(senhaPadrao)
            .SelecionarTipoUsuario("Cliente")
            .ClickSubmitRegistro();

        // Assert
        Assert.IsTrue(autenticacaoIndex.EstaLogado());
    }

    [TestMethod]
    public void Deve_Realizar_Login_Corretamente()
    {
        // Arrange
        AutenticacaoIndexPageObject autenticacaoIndex = new(driver);
        autenticacaoIndex
            .IrParaRegistro(enderecoBase)
            .PreencherEmail(emailEmpresa)
            .PreencherSenha(senhaPadrao)
            .PreencherConfirmarSenha(senhaPadrao)
            .SelecionarTipoUsuario("Empresa")
            .ClickSubmitRegistro();

        autenticacaoIndex
             .FazerLogout(enderecoBase);

        // Act
        AutenticacaoFormPageObject autenticacaoForm = autenticacaoIndex
             .IrParaLogin(enderecoBase);

        autenticacaoForm
            .PreencherEmail(emailEmpresa)
            .PreencherSenha(senhaPadrao)
            .ClickSubmitLogin();

        // Assert
        Assert.IsTrue(autenticacaoIndex.EstaLogado());
    }

    [TestMethod]
    public void Nao_Deve_Realizar_Login_Com_Credenciais_Invalidas()
    {
        // Arrange
        AutenticacaoIndexPageObject autenticacaoIndex = new(driver);
        autenticacaoIndex
            .IrParaRegistro(enderecoBase)
            .PreencherEmail(emailEmpresa)
            .PreencherSenha(senhaPadrao)
            .PreencherConfirmarSenha(senhaPadrao)
            .SelecionarTipoUsuario("Empresa")
            .ClickSubmitRegistro();

        autenticacaoIndex
             .FazerLogout(enderecoBase);

        // Act
        AutenticacaoFormPageObject autenticacaoForm = autenticacaoIndex
             .IrParaLogin(enderecoBase);

        autenticacaoForm
            .PreencherEmail(emailCliente)
            .PreencherSenha("2312")
            .ClickSubmitEsperandoErros();

        // Assert
        Assert.IsTrue(autenticacaoForm.EstourouValidacao());
    }

    [TestMethod]
    public void Deve_Realizar_Logout_Corretamente()
    {
        // Arrange
        AutenticacaoIndexPageObject autenticacaoIndex = new(driver);
        autenticacaoIndex
            .IrParaRegistro(enderecoBase)
            .PreencherEmail(emailEmpresa)
            .PreencherSenha(senhaPadrao)
            .PreencherConfirmarSenha(senhaPadrao)
            .SelecionarTipoUsuario("Empresa")
            .ClickSubmitRegistro();

        // Act
        autenticacaoIndex
             .FazerLogout(enderecoBase);

        // Assert
        Assert.IsFalse(autenticacaoIndex.EstaLogado());
    }

    [TestMethod]
    public void Nao_Deve_Cadastrar_Com_Campos_Vazios()
    {
        // Act
        AutenticacaoIndexPageObject autenticacaoIndex = new(driver);

        AutenticacaoFormPageObject autenticacaoForm = autenticacaoIndex
            .IrParaRegistro(enderecoBase);

        autenticacaoForm
            .ClickSubmitEsperandoErros();

        // Assert
        Assert.IsTrue(autenticacaoForm.EstourouValidacao("Email"));
        Assert.IsTrue(autenticacaoForm.EstourouValidacao("Senha"));
        Assert.IsTrue(autenticacaoForm.EstourouValidacao("ConfirmarSenha"));
    }

    [TestMethod]
    public void Nao_Deve_Cadastrar_Usuario_Com_Email_Duplicado()
    {
        // Arrange
        AutenticacaoIndexPageObject autenticacaoIndex = new(driver);
        autenticacaoIndex
            .IrParaRegistro(enderecoBase)
            .PreencherEmail(emailEmpresa)
            .PreencherSenha(senhaPadrao)
            .PreencherConfirmarSenha(senhaPadrao)
            .SelecionarTipoUsuario("Empresa")
            .ClickSubmitRegistro();

        autenticacaoIndex
             .FazerLogout(enderecoBase);

        // Act
        AutenticacaoFormPageObject autenticacaoForm = autenticacaoIndex
             .IrParaRegistro(enderecoBase);

        autenticacaoForm
            .PreencherEmail(emailEmpresa)
            .PreencherSenha("outraSenhaPadrao123!")
            .PreencherConfirmarSenha("outraSenhaPadrao123!")
            .SelecionarTipoUsuario("Empresa")
            .ClickSubmitEsperandoErros();

        // Assert
        Assert.IsTrue(autenticacaoForm.EstourouValidacao());
    }

    [TestMethod]
    public void Nao_Deve_Cadastrar_Usuario_Com_Email_Invalido()
    {
        // Act
        AutenticacaoIndexPageObject autenticacaoIndex = new(driver);

        AutenticacaoFormPageObject autenticacaoForm = autenticacaoIndex
            .IrParaRegistro(enderecoBase);

        autenticacaoForm
            .PreencherEmail("esseemailEvalido.com@serteza")
            .PreencherSenha(senhaPadrao)
            .PreencherConfirmarSenha(senhaPadrao)
            .SelecionarTipoUsuario("Empresa")
            .ClickSubmitEsperandoErros();

        // Assert
        Assert.IsTrue(autenticacaoForm.EstourouValidacao("Email"));
    }

    [TestMethod]
    public void Nao_Deve_Cadastrar_Usuario_Com_Senha_Invalida()
    {
        // Act
        AutenticacaoIndexPageObject autenticacaoIndex = new(driver);

        AutenticacaoFormPageObject autenticacaoForm = autenticacaoIndex
            .IrParaRegistro(enderecoBase);

        autenticacaoForm
            .PreencherEmail(emailCliente)
            .PreencherSenha("senhacemporsentofuncional")
            .PreencherConfirmarSenha("senhacemporsentofuncional")
            .SelecionarTipoUsuario("Cliente")
            .ClickSubmitEsperandoErros();

        // Assert
        Assert.IsTrue(autenticacaoForm.EstourouValidacao("Senha"));
        Assert.IsTrue(autenticacaoForm.EstourouValidacao("ConfirmarSenha"));
    }
}
