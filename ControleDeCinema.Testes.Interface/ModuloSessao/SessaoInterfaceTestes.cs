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
            .PreencherInicio("2025-08-12T14:15")
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
            .PreencherInicio("2025-08-24T14:30")
            .PreencherNumeroMaximoIngressos(12)
            .SelecionarFilme("Todo Mundo Tem a Irmã Gêmea Que Merece")
            .SelecionarSala(5)
            .ClickSubmit();

        // Assert
        Assert.IsTrue(sessaoIndex.ContemSessao("Todo Mundo Tem a Irmã Gêmea Que Merece"));
    }

    [TestMethod]
    public void Deve_Excluir_Sessao_Corretamente()
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
            .ClickEncerrar()
            .ClickSubmitEncerrar()
            .ClickExcluir();

        sessaoForm
            .ClickSubmitExcluir("Esposa de Mentirinha");

        // Assert
        Assert.IsFalse(sessaoIndex.ContemSessao("Esposa de Mentirinha"));
    }

    [TestMethod]
    public void Deve_Visualizar_Detalhes_Sessao_Corretamente()
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
        sessaoIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherInicio("2025-08-12T14:15")
            .PreencherNumeroMaximoIngressos(6)
            .SelecionarFilme("Esposa de Mentirinha")
            .SelecionarSala(5)
            .ClickSubmit();

        // Act
        sessaoIndex
            .IrPara(enderecoBase)
            .ClickDetalhes();

        // Assert
        Assert.IsTrue(sessaoIndex.ContemInicio("2025-08-12T14:15"));
        Assert.IsTrue(sessaoIndex.ContemMaxIngressos(6));
        Assert.IsTrue(sessaoIndex.ContemFilme("Esposa de Mentirinha"));
        Assert.IsTrue(sessaoIndex.ContemSala(5));
        Assert.IsTrue(sessaoIndex.ContemStatus());
    }

    [TestMethod]
    public void Deve_Visualizar_Ingressos_Vendidos_Por_Sessao()
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
        sessaoIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherInicio("2025-08-12T20:00")
            .PreencherNumeroMaximoIngressos(10)
            .SelecionarFilme("Esposa de Mentirinha")
            .SelecionarSala(5)
            .ClickSubmit();

        // Act (cliente compra 2 ingressos)
        FazerLogout();
        RegistrarContaCliente();

        SessaoFormPageObject sessaoForm = sessaoIndex
            .IrPara(enderecoBase)
            .ClickComprarIngresso();

        IngressoFormPageObject ingressoPage = new(driver);
        ingressoPage
            .SelecionarAssento(1)
            .MarcarMeiaEntrada()
            .ClickSubmit();

        sessaoIndex
            .IrPara(enderecoBase)
            .ClickComprarIngresso();

        ingressoPage
            .SelecionarAssento(2)
            .ClickSubmit();

        // Assert 
        FazerLogout();
        FazerLogin("Empresa");

        sessaoIndex
            .IrPara(enderecoBase)
            .ClickDetalhes();

        Assert.IsTrue(sessaoIndex.ContemIngressosVendidos(2));
        Assert.IsTrue(sessaoIndex.ContemIngressosDisponiveis(8));
    }
}
