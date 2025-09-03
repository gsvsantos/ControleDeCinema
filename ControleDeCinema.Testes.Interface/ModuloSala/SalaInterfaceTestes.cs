using ControleDeCinema.Testes.Interface.Compartilhado;

namespace ControleDeCinema.Testes.Interface.ModuloSala;

[TestClass]
[TestCategory("Testes de Interface de Sala")]
public sealed class SalaInterfaceTestes : TestFixture
{
    [TestInitialize]
    public override void InicializarTeste()
    {
        base.InicializarTeste();

        RegistrarContaEmpresarial();
    }

    [TestMethod]
    public void Deve_Cadastrar_Sala_Corretamente()
    {
        // Arrange
        SalaIndexPageObject salaIndex = new(driver);

        // Act
        SalaFormPageObject salaForm = salaIndex
            .IrPara(enderecoBase)
            .ClickCadastrar();

        salaForm
            .PreencherNumero(5)
            .PreencherCapacidade(50)
            .ClickSubmit();

        // Assert
        Assert.IsTrue(salaIndex.ContemSalaNumero(5));
    }

    [TestMethod]
    public void Deve_Editar_Sala_Corretamente()
    {
        // Arrange
        SalaIndexPageObject salaIndex = new(driver);

        salaIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherNumero(5)
            .PreencherCapacidade(50)
            .ClickSubmit();

        // Act
        SalaFormPageObject salaForm = salaIndex
            .IrPara(enderecoBase)
            .ClickEditar();

        salaForm
            .PreencherNumero(7)
            .PreencherCapacidade(60)
            .ClickSubmit();

        // Assert
        Assert.IsTrue(salaIndex.ContemSalaNumero(7));
    }

    [TestMethod]
    public void Deve_Excluir_Sala_Corretamente()
    {
        // Arrange
        SalaIndexPageObject salaIndex = new(driver);

        salaIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherNumero(9)
            .PreencherCapacidade(100)
            .ClickSubmit();

        // Act
        SalaFormPageObject salaForm = salaIndex
            .IrPara(enderecoBase)
            .ClickExcluir();

        salaForm
            .ClickSubmitExcluir("# 9");

        // Assert
        Assert.IsFalse(salaIndex.ContemSalaNumero(9));
    }

    [TestMethod]
    public void Deve_Visualizar_Todas_As_Salas_Corretamente()
    {
        // Arrange
        SalaIndexPageObject salaIndex = new(driver);

        salaIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherNumero(1)
            .PreencherCapacidade(30)
            .ClickSubmit();

        salaIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherNumero(2)
            .PreencherCapacidade(40)
            .ClickSubmit();

        // Act
        salaIndex.IrPara(enderecoBase);

        // Assert
        Assert.IsTrue(salaIndex.ContemSalaNumero(1));
        Assert.IsTrue(salaIndex.ContemSalaNumero(2));
    }

    [TestMethod]
    public void Nao_Deve_Cadastrar_Sala_Com_Campos_Vazios()
    {
        // Arrange
        SalaIndexPageObject salaIndex = new(driver);

        // Act
        SalaFormPageObject salaForm = salaIndex
            .IrPara(enderecoBase)
            .ClickCadastrar();

        salaForm
            .ClickSubmitEsperandoErros();

        // Assert
        Assert.IsTrue(salaForm.EstourouValidacaoSpan("Numero"));
        Assert.IsTrue(salaForm.EstourouValidacaoSpan("Capacidade"));
    }

    [TestMethod]
    public void Nao_Deve_Cadastrar_Sala_Com_Capacidade_Nao_Positiva()
    {
        // Arrange
        SalaIndexPageObject salaIndex = new(driver);

        // Act
        SalaFormPageObject salaForm = salaIndex
            .IrPara(enderecoBase)
            .ClickCadastrar();

        salaForm
            .PreencherNumero(10)
            .PreencherCapacidade(0)
            .ClickSubmitEsperandoErros();

        // Assert
        Assert.IsTrue(salaForm.EstourouValidacaoSpan("Capacidade"));
    }

    [TestMethod]
    public void Nao_Deve_Cadastrar_Sala_Com_Numero_Duplicado()
    {
        // Arrange
        SalaIndexPageObject salaIndex = new(driver);

        salaIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherNumero(3)
            .PreencherCapacidade(30)
            .ClickSubmit();

        // Act
        SalaFormPageObject salaForm = salaIndex
            .IrPara(enderecoBase)
            .ClickCadastrar();

        salaForm
            .PreencherNumero(3)
            .PreencherCapacidade(50)
            .ClickSubmitEsperandoErros();

        // Assert
        Assert.IsTrue(salaForm.EstourouValidacaoAlert());
    }
}
