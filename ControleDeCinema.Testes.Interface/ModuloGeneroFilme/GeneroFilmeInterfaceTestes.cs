using ControleDeCinema.Testes.Interface.Compartilhado;

namespace ControleDeCinema.Testes.Interface.ModuloGeneroFilme;

[TestClass]
[TestCategory("Testes de Interface de Gênero de Filme")]
public sealed class GeneroFilmeInterfaceTestes : TestFixture
{
    [TestInitialize]
    public override void InicializarTeste()
    {
        base.InicializarTeste();

        RegistrarContaEmpresarial();
    }

    [TestMethod]
    public void Deve_Cadastrar_GeneroFilme_Corretamente()
    {
        // Arrange
        GeneroFilmeIndexPageObject generoFilmeIndex = new(driver);

        generoFilmeIndex
            .IrPara(enderecoBase);

        // Act
        GeneroFilmeFormPageObject generoFilmeForm = generoFilmeIndex
            .ClickCadastrar();

        generoFilmeForm
            .PreencherDescricao("Comédia")
            .ClickSubmit();

        // Assert
        Assert.IsTrue(generoFilmeIndex.ContemGenero("Comédia"));
    }

    [TestMethod]
    public void Deve_Editar_GeneroFilme_Corretamente()
    {
        // Arrange
        GeneroFilmeIndexPageObject generoFilmeIndex = new(driver);

        generoFilmeIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherDescricao("Comédia")
            .ClickSubmit();

        // Act
        GeneroFilmeFormPageObject generoFilmeForm = generoFilmeIndex
            .IrPara(enderecoBase)
            .ClickEditar();

        generoFilmeForm
            .PreencherDescricao("Românce")
            .ClickSubmit();

        // Assert
        Assert.IsTrue(generoFilmeIndex.ContemGenero("Românce"));
    }

    [TestMethod]
    public void Deve_Excluir_GeneroFilme_Corretamente()
    {
        // Arrange
        GeneroFilmeIndexPageObject generoFilmeIndex = new(driver);

        generoFilmeIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherDescricao("Comédia")
            .ClickSubmit();

        // Act
        GeneroFilmeFormPageObject generoFilmeForm = generoFilmeIndex
            .IrPara(enderecoBase)
            .ClickExcluir();

        generoFilmeForm
            .ClickSubmitExcluir("Românce");

        // Assert
        Assert.IsFalse(generoFilmeIndex.ContemGenero("Românce"));
    }

    [TestMethod]
    public void Deve_Visualizar_GenerosFilme_Cadastrados_Corretamente()
    {
        // Arrange
        GeneroFilmeIndexPageObject generoFilmeIndex = new(driver);

        generoFilmeIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherDescricao("Comédia")
            .ClickSubmit();

        generoFilmeIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherDescricao("Românce")
            .ClickSubmit();

        // Act
        generoFilmeIndex
            .IrPara(enderecoBase);

        // Assert
        Assert.IsTrue(generoFilmeIndex.ContemGenero("Comédia"));
        Assert.IsTrue(generoFilmeIndex.ContemGenero("Românce"));
    }

    [TestMethod]
    public void Nao_Deve_Cadastrar_GeneroFilme_Com_Campos_Vazios()
    {
        // Arrange
        GeneroFilmeIndexPageObject generoFilmeIndex = new(driver);

        generoFilmeIndex
            .IrPara(enderecoBase);

        // Act
        GeneroFilmeFormPageObject generoFilmeForm = generoFilmeIndex
            .ClickCadastrar();

        generoFilmeForm
            .ClickSubmitEsperandoErros();

        // Assert
        Assert.IsTrue(generoFilmeForm.EstourouValidacao("Descricao"));
    }

    [TestMethod]
    public void Nao_Deve_Cadastrar_GeneroFilme_Com_Descricao_Duplicada()
    {
        // Arrange
        GeneroFilmeIndexPageObject generoFilmeIndex = new(driver);

        generoFilmeIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherDescricao("Comédia")
            .ClickSubmit();

        // Act
        GeneroFilmeFormPageObject generoFilmeForm = generoFilmeIndex
            .IrPara(enderecoBase)
            .ClickCadastrar();

        generoFilmeForm
            .PreencherDescricao("Comédia")
            .ClickSubmitEsperandoErros();

        // Assert
        Assert.IsTrue(generoFilmeForm.EstourouValidacao());
    }
}
