using ControleDeCinema.Testes.Interface.Compartilhado;
using ControleDeCinema.Testes.Interface.ModuloFilme;
using ControleDeCinema.Testes.Interface.ModuloGeneroFilme;
using ControleDeCinema.Testes.Interface.ModuloSala;

namespace ControleDeCinema.Testes.Interface.ModuloSessao;

[TestClass]
[TestCategory("Testes de Interface de Sessão")]
public class SessaoInterfaceTestes : TestFixture
{
    [TestInitialize]
    public override void InicializarTeste()
    {
        base.InicializarTeste();

        RegistrarContaEmpresarial();
    }

    [TestMethod]
    public void Deve_Cadastrar_Sessao_Corretamente()
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

        SalaIndexPageObject salaIndex = new(driver);
        salaIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherNumero(5)
            .PreencherCapacidade(50)
            .ClickSubmit();

        SessaoIndexPageObject sessaoIndex = new(driver);

        // Act
        SessaoFormPageObject sessaoForm = sessaoIndex
            .IrPara(enderecoBase)
            .ClickCadastrar();

        sessaoForm
            .PreencherInicio("2025-08-26T14:30")
            .PreencherNumeroMaximoIngressos(12)
            .SelecionarFilme("Esposa de Mentirinha")
            .SelecionarSala(5)
            .ClickSubmit();

        // Assert
        Assert.IsTrue(sessaoIndex.ContemSessao("Esposa de Mentirinha"));
    }

    [TestMethod]
    public void Deve_Editar_Sessao_Corretamente()
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
            .PreencherTitulo("Todo Mundo Tem a Irmã Gêmea Que Merece")
            .PreencherDuracao(117)
            .MarcarLancamento()
            .SelecionarGenero("Comédia")
            .ClickSubmit();

        SalaIndexPageObject salaIndex = new(driver);
        salaIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherNumero(5)
            .PreencherCapacidade(50)
            .ClickSubmit();

        SessaoIndexPageObject sessaoIndex = new(driver);
        sessaoIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherInicio("2025-08-12T14:15")
            .PreencherNumeroMaximoIngressos(6)
            .SelecionarFilme("Esposa de Mentirinha")
            .SelecionarSala(5)
            .ClickSubmit();

        // Act
        SessaoFormPageObject sessaoForm = sessaoIndex
            .IrPara(enderecoBase)
            .ClickEditar();

        sessaoForm
            .PreencherInicio("2025-08-22T14:30")
            .PreencherNumeroMaximoIngressos(12)
            .SelecionarFilme("Todo Mundo Tem a Irmã Gêmea Que Merece")
            .SelecionarSala(5)
            .ClickSubmit();

        // Assert
        Assert.IsTrue(sessaoIndex.ContemSessao("Todo Mundo Tem a Irmã Gêmea Que Merece"));
    }
}
