using ControleDeCinema.Aplicacao.ModuloSessao;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Dominio.ModuloSessao;
using FizzWare.NBuilder;
using FluentResults;
using Microsoft.Extensions.Logging;
using Moq;

namespace ControleDeCinema.Testes.Unidades.ModuloIngresso;

[TestClass]
[TestCategory("Testes de Unidade de IngressoAppService")]
public class IngressoAppServiceTestes
{
    // SUT
    private IngressoAppService ingressoAppService;
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
    private static Sessao sessaoPadrao = Builder<Sessao>.CreateNew()
        .WithFactory(() => new(inicioPadrao, 30, filmePadrao, salaPadrao))
        .Build();

    // Mocks
    private Mock<ITenantProvider> tenantProviderMock;
    private Mock<IRepositorioIngresso> repositorioIngressoMock;
    private Mock<ILogger<IngressoAppService>> loggerMock;

    [TestInitialize]
    public void Setup()
    {
        tenantProviderMock = new Mock<ITenantProvider>();
        repositorioIngressoMock = new Mock<IRepositorioIngresso>();
        loggerMock = new Mock<ILogger<IngressoAppService>>();

        ingressoAppService = new IngressoAppService(
            tenantProviderMock.Object,
            repositorioIngressoMock.Object,
            loggerMock.Object
        );
    }

    #region Testes Seleção de Todos
    [TestMethod]
    public void Selecionar_Todos_Ingressos_Deve_Retornar_Sucesso()
    {
        // Arrange
        Guid usuarioId = Guid.NewGuid();

        List<Ingresso> ingressosDoUsuario = new()
        {
            sessaoPadrao.GerarIngresso(1, true),
            sessaoPadrao.GerarIngresso(3, true)
        };

        Ingresso outroIngresso = sessaoPadrao.GerarIngresso(5, true);

        tenantProviderMock
            .SetupGet(t => t.UsuarioId)
            .Returns(usuarioId);

        repositorioIngressoMock
            .Setup(r => r.SelecionarRegistros(usuarioId))
            .Returns(ingressosDoUsuario);

        // Act
        Result<List<Ingresso>> resultadosSelecao = ingressoAppService.SelecionarTodos();

        List<Ingresso> ingressosSelecionados = resultadosSelecao.ValueOrDefault;

        // Assert
        repositorioIngressoMock.Verify(r => r.SelecionarRegistros(usuarioId), Times.Once);

        Assert.IsNotNull(resultadosSelecao);
        Assert.IsTrue(resultadosSelecao.IsSuccess);
        CollectionAssert.AreEquivalent(ingressosDoUsuario, ingressosSelecionados);
        CollectionAssert.DoesNotContain(ingressosDoUsuario, outroIngresso);
    }

    [TestMethod]
    public void Selecionar_Todos_Ingressos_Com_Excecao_Lancada_Deve_Retornar_Falha()
    {
        // Arrange
        Guid usuarioId = Guid.NewGuid();

        List<Ingresso> ingressosDoUsuario = new()
        {
            sessaoPadrao.GerarIngresso(1, true),
            sessaoPadrao.GerarIngresso(3, true)
        };

        tenantProviderMock
            .SetupGet(t => t.UsuarioId)
            .Returns(usuarioId);

        repositorioIngressoMock
            .Setup(r => r.SelecionarRegistros(usuarioId))
            .Throws(new Exception("Erro inesperado"));

        // Act
        Result<List<Ingresso>> resultadosSelecao = ingressoAppService.SelecionarTodos();

        List<Ingresso> ingressosSelecionados = resultadosSelecao.ValueOrDefault;

        // Assert
        repositorioIngressoMock.Verify(r => r.SelecionarRegistros(usuarioId), Times.Once);

        string mensagemErro = resultadosSelecao.Errors[0].Message;

        Assert.IsNotNull(resultadosSelecao);
        Assert.IsTrue(resultadosSelecao.IsFailed);
        Assert.IsNull(ingressosSelecionados);
        CollectionAssert.AreNotEquivalent(ingressosDoUsuario, ingressosSelecionados);
        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
    }
    #endregion
}
