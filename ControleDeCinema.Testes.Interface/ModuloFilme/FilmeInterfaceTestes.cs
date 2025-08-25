using ControleDeCinema.Testes.Interface.Compartilhado;
using ControleDeCinema.Testes.Interface.ModuloGeneroFilme;

namespace ControleDeCinema.Testes.Interface.ModuloFilme;

[TestClass]
[TestCategory("Testes de Interface de Filme")]
public sealed class FilmeInterfaceTestes : TestFixture
{
    [TestInitialize]
    public override void InicializarTeste()
    {
        base.InicializarTeste();

        driver.Manage().Cookies.DeleteAllCookies();

        RegistrarContaEmpresarial();
    }

    [TestMethod]
    public void Deve_Cadastrar_Filme_Corretamente()
    {
        // Arrange
        GeneroFilmeIndexPageObject generoFilmeIndex = new(driver);

        generoFilmeIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherDescricao("Comédia")
            .ClickSubmit();

        FilmeIndexPageObject filmeIndex = new(driver);

        // Act

        FilmeFormPageObject filmeForm = filmeIndex
            .IrPara(enderecoBase)
            .ClickCadastrar();

        filmeForm
            .PreencherTitulo("Esposa de Mentirinha")
            .PreencherDuracao(117)
            .MarcarLancamento()
            .SelecionarGenero("Comédia")
            .ClickSubmit();

        // Assert
        Assert.IsTrue(filmeIndex.ContemFilme("Esposa de Mentirinha"));
    }

    [TestMethod]
    public void Deve_Editar_Filme_Corretamente()
    {
        // Arrange
        GeneroFilmeIndexPageObject generoFilmeIndex = new(driver);

        generoFilmeIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherDescricao("Comédia")
            .ClickSubmit();

        FilmeIndexPageObject filmeIndex = new(driver);

        filmeIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherTitulo("Esposa de Mentirinha")
            .PreencherDuracao(117)
            .MarcarLancamento()
            .SelecionarGenero("Comédia")
            .ClickSubmit();

        // Act
        FilmeFormPageObject filmeForm = filmeIndex
            .IrPara(enderecoBase)
            .ClickEditar();

        filmeForm
            .PreencherTitulo("Esposa de Mentirinha Editada")
            .PreencherDuracao(117)
            .MarcarLancamento()
            .SelecionarGenero("Comédia")
            .ClickSubmit();

        // Assert
        Assert.IsTrue(filmeIndex.ContemFilme("Esposa de Mentirinha Editada"));
    }
}
