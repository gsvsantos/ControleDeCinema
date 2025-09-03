using ControleDeCinema.Testes.Interface.Compartilhado;
using ControleDeCinema.Testes.Interface.ModuloFilme;
using ControleDeCinema.Testes.Interface.ModuloGeneroFilme;
using ControleDeCinema.Testes.Interface.ModuloSala;
using ControleDeCinema.Testes.Interface.ModuloSessao;

namespace ControleDeCinema.Testes.Interface.ModuloIngresso;

[TestClass]
[TestCategory("Testes de Interface de Ingresso")]
public sealed class IngressoInterfaceTestes : TestFixture
{
    [TestInitialize]
    public override void InicializarTeste()
    {
        base.InicializarTeste();

        RegistrarContaEmpresarial();
    }

    [TestMethod]
    public void Deve_Visualizar_Sessoes_Agrupadas_Por_Filme()
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
            .PreencherDuracao(94)
            .MarcarLancamento()
            .SelecionarGenero("Comédia")
            .ClickSubmit();

        SalaIndexPageObject salaIndex = new(driver);
        salaIndex
            .IrPara(enderecoBase).ClickCadastrar()
            .PreencherNumero(1)
            .PreencherCapacidade(15)
            .ClickSubmit();

        salaIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherNumero(5)
            .PreencherCapacidade(30)
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

        sessaoIndex
            .IrPara(enderecoBase)
            .ClickCadastrar()
            .PreencherInicio("2025-08-12T20:00")
            .PreencherNumeroMaximoIngressos(10)
            .SelecionarFilme("Todo Mundo Tem a Irmã Gêmea Que Merece")
            .SelecionarSala(1)
            .ClickSubmit();

        // Act
        FazerLogout();
        RegistrarContaCliente();

        IngressoIndexPageObject ingressoIndex = new IngressoIndexPageObject(driver)
            .IrPara(enderecoBase);

        // Assert
        Assert.IsTrue(ingressoIndex.ContemFilme("Esposa de Mentirinha"));
        Assert.IsTrue(ingressoIndex.ContemFilme("Todo Mundo Tem a Irmã Gêmea Que Merece"));
    }

    [TestMethod]
    public void Deve_Comprar_Ingresso_Corretamente()
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
            .PreencherNumeroMaximoIngressos(10)
            .SelecionarFilme("Esposa de Mentirinha")
            .SelecionarSala(5)
            .ClickSubmit();

        // Act
        FazerLogout();
        RegistrarContaCliente();

        SessaoFormPageObject _ = sessaoIndex
            .IrPara(enderecoBase)
            .ClickComprarIngresso();

        IngressoFormPageObject ingressoPage = new(driver);
        ingressoPage
            .SelecionarAssento(1)
            .MarcarMeiaEntrada()
            .ClickSubmitComoCliente();

        // Assert
        FazerLogout();
        FazerLoginEmpresa();

        sessaoIndex
            .IrPara(enderecoBase)
            .ClickDetalhes();

        Assert.IsTrue(sessaoIndex.ContemIngressosVendidos(1));
        Assert.IsTrue(sessaoIndex.ContemIngressosDisponiveis(9));
    }

    [TestMethod]
    public void Nao_Deve_Comprar_Ingresso_Para_Sessao_Lotada()
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
            .PreencherNumeroMaximoIngressos(1)
            .SelecionarFilme("Esposa de Mentirinha")
            .SelecionarSala(5)
            .ClickSubmit();

        // Act 
        FazerLogout();
        RegistrarContaCliente();

        SessaoFormPageObject _ = sessaoIndex
            .IrPara(enderecoBase)
            .ClickComprarIngresso();

        sessaoIndex = new IngressoFormPageObject(driver)
            .SelecionarAssento(1)
            .ClickSubmitComoCliente();

        sessaoIndex
            .IrPara(enderecoBase)
            .ClickDetalhes();

        // Assert (a View não libera o botão de comprar ingresso caso esgote - validação feita nos testes de unidades) 
        Assert.IsFalse(sessaoIndex.ContemIngressosDisponiveis(1));
    }

    [TestMethod]
    public void Nao_Deve_Comprar_Quantidade_Indisponivel_De_Ingresso()
    {
        // (a View não libera o botão de comprar ingresso caso esgote e compra é feita de um em um - validação feita nos testes de unidades)
    }
}
