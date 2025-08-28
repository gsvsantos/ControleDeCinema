using ControledeCinema.Dominio.Compartilhado;
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

namespace ControleDeCinema.Testes.Unidades.ModuloSessao;

[TestClass]
[TestCategory("Testes de Unidade de SessaoAppService")]
public class SessaoAppServiceTestes
{
    private SessaoAppService sessaoAppService;
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

    // Mocks
    private Mock<ITenantProvider> tenantProviderMock;
    private Mock<IRepositorioSessao> repositorioSessaoMock;
    private Mock<IUnitOfWork> unitOfWorkMock;
    private Mock<ILogger<SessaoAppService>> loggerMock;

    [TestInitialize]
    public void Setup()
    {
        tenantProviderMock = new Mock<ITenantProvider>();
        repositorioSessaoMock = new Mock<IRepositorioSessao>();
        unitOfWorkMock = new Mock<IUnitOfWork>();
        loggerMock = new Mock<ILogger<SessaoAppService>>();

        sessaoAppService = new SessaoAppService(
            tenantProviderMock.Object,
            repositorioSessaoMock.Object,
            unitOfWorkMock.Object,
            loggerMock.Object
        );
    }

    #region Testes Cadastro
    [TestMethod]
    public void Cadastrar_Sessao_Deve_Retornar_Sucesso()
    {
        // Arrange
        Sessao novaSessao = new(inicioPadrao, 30, filmePadrao, salaPadrao);

        repositorioSessaoMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sessao>());

        // Act
        Result resultadoCadastro = sessaoAppService.Cadastrar(novaSessao);

        // Assert
        repositorioSessaoMock.Verify(r => r.Cadastrar(novaSessao), Times.Once);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Once);

        Assert.IsNotNull(resultadoCadastro);
        Assert.IsTrue(resultadoCadastro.IsSuccess);
    }

    [TestMethod]
    public void Cadastrar_Sessao_Com_Capacidade_Excedida_Deve_Retornar_Falha()
    {
        // Arrange (salaPadrao capacidade = 30)
        Sessao novaSessao = new(inicioPadrao, 40, filmePadrao, salaPadrao);

        repositorioSessaoMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sessao>());

        // Act
        Result resultadoCadastro = sessaoAppService.Cadastrar(novaSessao);

        // Assert
        repositorioSessaoMock.Verify(r => r.Cadastrar(novaSessao), Times.Never);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Never);

        string mensagemErro = resultadoCadastro.Errors[0].Message;

        Assert.IsNotNull(resultadoCadastro);
        Assert.IsTrue(resultadoCadastro.IsFailed);
        Assert.AreEqual("Registro duplicado", mensagemErro);
    }

    [TestMethod]
    public void Cadastrar_Sessao_Duplicada_Mesma_Sala_E_Horario_Deve_Retornar_Falha()
    {
        // Arrange  (filmePadrao tem 1h57 minutos, inicioPadrao é as 14:30 | 16:26 nega cadastro, 16:27 aceita cadastro)
        repositorioSessaoMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sessao>() { new(inicioPadrao, 30, filmePadrao, salaPadrao) });

        DateTime novoInicio = new(2025, 08, 09, 16, 26, 00);
        Sessao novaSessao = new(novoInicio, 30, filmePadrao, salaPadrao);

        // Act
        Result resultadoCadastro = sessaoAppService.Cadastrar(novaSessao);

        // Assert
        repositorioSessaoMock.Verify(r => r.Cadastrar(novaSessao), Times.Never);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Never);

        string mensagemErro = resultadoCadastro.Errors[0].Message;

        Assert.IsNotNull(resultadoCadastro);
        Assert.IsTrue(resultadoCadastro.IsFailed);
        Assert.AreEqual("Registro duplicado", mensagemErro);
    }

    [TestMethod]
    public void Cadastrar_Sessao_Com_Excecao_Lancada_Deve_Retornar_Falha()
    {
        // Arrange 
        Sessao novaSessao = new(inicioPadrao, 30, filmePadrao, salaPadrao);

        repositorioSessaoMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sessao>());

        repositorioSessaoMock
            .Setup(r => r.Cadastrar(novaSessao))
            .Throws(new Exception("Erro inesperado"));

        unitOfWorkMock
            .Setup(u => u.Commit())
            .Throws(new Exception("Erro no cadastro"));

        // Act
        Result resultadoCadastro = sessaoAppService.Cadastrar(novaSessao);

        // Assert
        unitOfWorkMock.Verify(u => u.Rollback(), Times.Once);

        string mensagemErro = resultadoCadastro.Errors[0].Message;

        Assert.IsNotNull(resultadoCadastro);
        Assert.IsTrue(resultadoCadastro.IsFailed);
        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
    }
    #endregion
}
