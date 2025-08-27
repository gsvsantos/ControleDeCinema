using ControleDeCinema.Testes.Interface.Compartilhado;
using ControleDeCinema.Testes.Interface.ModuloAutenticacao;

namespace ControleDeCinema.Testes.Interface;

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

        FazerLogout();

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

        FazerLogout();

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
}
