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

    [TestMethod]
    public void Deve_Excluir_Filme_Corretamente()
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
            .ClickExcluir();

        filmeForm
            .ClickSubmitExcluir("Esposa de Mentirinha");

        // Assert
        Assert.IsFalse(filmeIndex.ContemFilme("Esposa de Mentirinha"));
    }

    [TestMethod]
    public void Deve_Visualizar_Filmes_Cadastrados_Corretamente()
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

        filmeIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherTitulo("Cada Um Tem a Gêmea que Merece")
            .PreencherDuracao(91)
            .MarcarLancamento()
            .SelecionarGenero("Comédia")
            .ClickSubmit();

        // Act
        filmeIndex
            .IrPara(enderecoBase);

        // Assert
        Assert.IsTrue(filmeIndex.ContemFilme("Esposa de Mentirinha"));
        Assert.IsTrue(filmeIndex.ContemFilme("Cada Um Tem a Gêmea que Merece"));
    }

    [TestMethod]
    public void Nao_Deve_Cadastrar_Filme_Com_Campos_Vazios()
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
            .ClickSubmitEsperandoErros();

        // Assert
        Assert.IsTrue(filmeForm.EstourouValidacao("Titulo"));
        Assert.IsTrue(filmeForm.EstourouValidacao("Duracao"));
    }

    [TestMethod]
    public void Nao_Deve_Cadastrar_Filme_Com_Duracao_Invalida()
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
            .PreencherDuracao(0)
            .MarcarLancamento()
            .SelecionarGenero("Comédia")
            .ClickSubmitEsperandoErros();

        // Assert
        Assert.IsTrue(filmeForm.EstourouValidacao("Duracao"));
    }

    [TestMethod]
    public void Nao_Deve_Cadastrar_Filme_Com_Titulo_Duplicado()
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
            .ClickCadastrar();

        filmeForm
            .PreencherTitulo("Esposa de Mentirinha")
            .PreencherDuracao(117)
            .MarcarLancamento()
            .SelecionarGenero("Comédia")
            .ClickSubmitEsperandoErros();

        // Assert
        Assert.IsTrue(filmeForm.EstourouValidacao("Titulo"));
    }
}
