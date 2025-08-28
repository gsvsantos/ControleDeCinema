using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Dominio.ModuloSessao;
using FizzWare.NBuilder;

namespace ControleDeCinema.Testes.Unidades.ModuloSessao;

[TestClass]
[TestCategory("Testes de Unidade de Sessao (Domínio)")]
public class SessaoTestes
{
    private static readonly GeneroFilme generoPadrao = Builder<GeneroFilme>.CreateNew()
        .WithFactory(() => new("Comédia") { Id = Guid.NewGuid() })
        .Build();
    private static readonly Filme filmePadrao = Builder<Filme>.CreateNew()
        .WithFactory(() => new Filme("Esposa de Mentirinha", 117, true, generoPadrao))
        .Build();
    private static readonly Sala salaPadrao = Builder<Sala>.CreateNew()
        .WithFactory(() => new Sala(1, 30) { Id = Guid.NewGuid() })
        .Build();
    private static readonly DateTime inicioPadrao = new(2025, 08, 09, 14, 30, 00);

    private Sessao sessao;

    #region GerarIngresso
    [TestMethod]
    public void GerarIngresso_Deve_Adicionar_Ingresso_A_Lista()
    {
        // Arrange
        sessao = new(inicioPadrao, 10, filmePadrao, salaPadrao);

        // Act
        Ingresso novoIngresso = sessao.GerarIngresso(1, true);

        // Assert
        bool sessaoContemIngressoGerado = sessao.Ingressos.Contains(novoIngresso);

        Assert.IsNotNull(novoIngresso);
        Assert.IsTrue(sessaoContemIngressoGerado);
    }

    [TestMethod]
    public void GerarIngresso_Deve_Definir_Referencia_Da_Sessao_No_Ingresso()
    {
        // Arrange
        sessao = new(inicioPadrao, 10, filmePadrao, salaPadrao);

        // Act
        Ingresso novoIngresso = sessao.GerarIngresso(1, true);

        // Assert
        bool sessaoContemIngressoGerado = sessao.Ingressos.Contains(novoIngresso);

        Assert.IsNotNull(novoIngresso);
        Assert.IsTrue(sessaoContemIngressoGerado);
        Assert.AreEqual(sessao, novoIngresso.Sessao);
    }
    #endregion

    #region ObterAssentosDisponiveis
    [TestMethod]
    public void ObterAssentosDisponiveis_Deve_Retornar_Todos_Quando_Sem_Vendas()
    {
        // Arrange
        sessao = new(inicioPadrao, 10, filmePadrao, salaPadrao);

        // Act
        int[] assentosDisponiveis = sessao.ObterAssentosDisponiveis(); ;

        // Assert
        int[] assentosEsperados = Enumerable.Range(1, sessao.NumeroMaximoIngressos).ToArray();

        Assert.IsNotNull(assentosDisponiveis);
        Assert.AreEqual(sessao.NumeroMaximoIngressos, assentosDisponiveis.Length);
        CollectionAssert.AreEqual(assentosEsperados, assentosDisponiveis);
    }

    [TestMethod]
    public void ObterAssentosDisponiveis_Deve_Excluir_Assentos_Ja_Vendidos()
    {
        // Arrange
        sessao = new(inicioPadrao, 10, filmePadrao, salaPadrao);

        for (int i = 1; i <= 5; i++)
            sessao.GerarIngresso(i, false);

        // Act
        int[] assentosDisponiveis = sessao.ObterAssentosDisponiveis();

        // Assert
        Assert.IsNotNull(assentosDisponiveis);
        Assert.AreEqual(5, assentosDisponiveis.Length);
        for (int i = 1; i <= 5; i++)
            CollectionAssert.DoesNotContain(assentosDisponiveis, i);
    }
    #endregion

    #region ObterQuantidadeIngressosDisponíveis
    [TestMethod]
    public void ObterQuantidadeIngressosDisponiveis_Deve_Retornar_Diferenca_Entre_Maximo_E_Vendidos()
    {
        // Arrange
        sessao = new(inicioPadrao, 10, filmePadrao, salaPadrao);

        for (int i = 1; i <= 5; i++)
            sessao.GerarIngresso(i, false);

        // Act
        int quantidadeDisponivel = sessao.ObterQuantidadeIngressosDisponiveis();

        // Assert
        Assert.AreEqual(5, quantidadeDisponivel);
    }
    #endregion
}
