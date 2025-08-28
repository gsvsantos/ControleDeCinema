using ControledeCinema.Dominio.Compartilhado;
using ControleDeCinema.Aplicacao.ModuloFilme;
using ControleDeCinema.Aplicacao.ModuloSala;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using FizzWare.NBuilder;
using FluentResults;
using Microsoft.Extensions.Logging;
using Moq;

namespace ControleDeCinema.Testes.Unidades.ModuloFilme;

[TestClass]
[TestCategory("Testes de Unidade de FilmeAppService")]
public class FilmeAppServiceTestes
{
    // SUT
    private FilmeAppService filmeAppService;
    private static GeneroFilme generoPadrao = Builder<GeneroFilme>.CreateNew()
        .With(d => d.Id = Guid.NewGuid())
        .With(d => d.Descricao = "Comédia")
        .Build();
    private static Sala salaPadrao = Builder<Sala>.CreateNew()
        .With(s => s.Id = Guid.NewGuid())
        .With(s => s.Numero = 1)
        .With(s => s.Capacidade = 30)
        .Build();

    // Mocks
    private Mock<ITenantProvider> tenantProviderMock;
    private Mock<IRepositorioFilme> repositorioFilmeMock;
    private Mock<IUnitOfWork> unitOfWorkMock;
    private Mock<ILogger<FilmeAppService>> loggerMock;

    [TestInitialize]
    public void Setup()
    {
        tenantProviderMock = new Mock<ITenantProvider>();
        repositorioFilmeMock = new Mock<IRepositorioFilme>();
        unitOfWorkMock = new Mock<IUnitOfWork>();
        loggerMock = new Mock<ILogger<FilmeAppService>>();

        filmeAppService = new FilmeAppService(
            tenantProviderMock.Object,
            repositorioFilmeMock.Object,
            unitOfWorkMock.Object,
            loggerMock.Object
        );
    }

    #region Testes Cadastro
    [TestMethod]
    public void Cadastrar_Filme_Deve_Retornar_Sucesso()
    {
        // Arrange
        Filme novoFilme = new("Esposa de Mentirinha", 117, true, generoPadrao);

        repositorioFilmeMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Filme>());

        // Act
        Result resultadoCadastro = filmeAppService.Cadastrar(novoFilme);

        // Assert
        repositorioFilmeMock.Verify(r => r.Cadastrar(novoFilme), Times.Once);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Once);

        Assert.IsNotNull(resultadoCadastro);
        Assert.IsTrue(resultadoCadastro.IsSuccess);
    }

    [TestMethod]
    public void Cadastrar_Filme_Duplicado_Deve_Retornar_Falha()
    {
        // Arrange
        repositorioFilmeMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Filme>() { new("Esposa de Mentirinha", 117, true, generoPadrao) });

        Filme novoFilme = new("Esposa de Mentirinha", 94, true, generoPadrao);

        // Act
        Result resultadoCadastro = filmeAppService.Cadastrar(novoFilme);

        // Assert
        repositorioFilmeMock.Verify(r => r.Cadastrar(novoFilme), Times.Never);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Never);

        string mensagemErro = resultadoCadastro.Errors[0].Message;

        Assert.IsNotNull(resultadoCadastro);
        Assert.IsTrue(resultadoCadastro.IsFailed);
        Assert.AreEqual("Registro duplicado", mensagemErro);
    }

    [TestMethod]
    public void Cadastrar_Filme_Com_Excecao_Lancada_Deve_Retornar_Falha()
    {
        // Arrange
        Filme novoFilme = new("Esposa de Mentirinha", 117, true, generoPadrao);

        repositorioFilmeMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Filme>());

        repositorioFilmeMock
            .Setup(r => r.Cadastrar(novoFilme))
            .Throws(new Exception("Erro inesperado"));

        unitOfWorkMock
            .Setup(u => u.Commit())
            .Throws(new Exception("Erro no cadastro"));

        // Act
        Result resultadoCadastro = filmeAppService.Cadastrar(novoFilme);

        // Assert
        unitOfWorkMock.Verify(u => u.Rollback(), Times.Once);

        string mensagemErro = resultadoCadastro.Errors[0].Message;

        Assert.IsNotNull(resultadoCadastro);
        Assert.IsTrue(resultadoCadastro.IsFailed);
        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
    }
    #endregion

}
