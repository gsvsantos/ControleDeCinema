using ControledeCinema.Dominio.Compartilhado;
using ControleDeCinema.Aplicacao.ModuloFilme;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using FizzWare.NBuilder;
using FluentResults;
using Microsoft.Extensions.Logging;
using Moq;

namespace ControleDeCinema.Testes.Unidades.ModuloFilme;

[TestClass]
[TestCategory("Testes de Unidade de FilmeAppService")]
public class FilmeAppServiceTestes
{
    private FilmeAppService filmeAppService;
    private static GeneroFilme generoPadrao = Builder<GeneroFilme>.CreateNew()
        .With(d => d.Id = Guid.NewGuid())
        .With(d => d.Descricao = "Comédia")
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

    #region Testes Edição
    [TestMethod]
    public void Editar_Filme_Deve_Retornar_Sucesso()
    {
        // Arrange
        GeneroFilme novoGenero = Builder<GeneroFilme>.CreateNew()
            .With(g => g.Id = Guid.NewGuid())
            .With(g => g.Descricao = "Românce")
            .Build();

        Filme novoFilme = new("Esposa de Mentirinha", 117, true, novoGenero);

        repositorioFilmeMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Filme>() { novoFilme });

        repositorioFilmeMock
            .Setup(r => r.SelecionarRegistroPorId(novoFilme.Id))
            .Returns(novoFilme);

        Filme filmeEditado = new("Todo Mundo Tem A Irmã Gêmea Que Merece", 94, true, generoPadrao);

        // Act
        Result resultadoEdicao = filmeAppService.Editar(novoFilme.Id, filmeEditado);

        // Assert
        repositorioFilmeMock.Verify(r => r.Editar(novoFilme.Id, filmeEditado), Times.Once);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Once);

        Assert.IsNotNull(resultadoEdicao);
        Assert.IsTrue(resultadoEdicao.IsSuccess);
    }

    [TestMethod]
    public void Editar_Filme_Duplicado_Deve_Retornar_Falha()
    {
        // Arrange
        Filme novoFilme = new("Esposa de Mentirinha", 117, true, generoPadrao);

        List<Filme> filmesExistentes = new()
        {
            novoFilme,
            new("Todo Mundo Tem A Irmã Gêmea Que Merece", 94, true, generoPadrao)
        };

        repositorioFilmeMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(filmesExistentes);

        Filme filmeEditado = new("Todo Mundo Tem A Irmã Gêmea Que Merece", 94, true, generoPadrao);

        // Act
        Result resultadoEdicao = filmeAppService.Editar(novoFilme.Id, filmeEditado);

        // Assert
        repositorioFilmeMock.Verify(r => r.Editar(novoFilme.Id, filmeEditado), Times.Never);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Never);

        string mensagemErro = resultadoEdicao.Errors[0].Message;

        Assert.IsNotNull(resultadoEdicao);
        Assert.IsTrue(resultadoEdicao.IsFailed);
        Assert.AreEqual("Registro duplicado", mensagemErro);
    }

    [TestMethod]
    public void Editar_Filme_Com_Excecao_Lancada_Deve_Retornar_Falha()
    {
        // Arrange
        Filme novoFilme = new("Esposa de Mentirinha", 117, true, generoPadrao);

        repositorioFilmeMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Filme>() { novoFilme });

        Filme filmeEditado = new("Todo Mundo Tem A Irmã Gêmea Que Merece", 94, true, generoPadrao);

        repositorioFilmeMock
            .Setup(r => r.Editar(novoFilme.Id, filmeEditado))
            .Throws(new Exception("Erro inesperado"));

        unitOfWorkMock
            .Setup(u => u.Commit())
            .Throws(new Exception("Erro na edição"));

        // Act
        Result resultadoEdicao = filmeAppService.Editar(novoFilme.Id, filmeEditado);

        // Assert
        unitOfWorkMock.Verify(u => u.Rollback(), Times.Once);

        string mensagemErro = resultadoEdicao.Errors[0].Message;

        Assert.IsNotNull(resultadoEdicao);
        Assert.IsTrue(resultadoEdicao.IsFailed);
        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
    }
    #endregion

    #region Testes Exclusão
    [TestMethod]
    public void Excluir_Filme_Deve_Retornar_Sucesso()
    {
        // Arrange
        Filme novoFilme = new("Esposa de Mentirinha", 117, true, generoPadrao);

        repositorioFilmeMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Filme>() { novoFilme });

        repositorioFilmeMock
            .Setup(r => r.SelecionarRegistroPorId(novoFilme.Id))
            .Returns(novoFilme);

        // Act
        Result resultadoExclusao = filmeAppService.Excluir(novoFilme.Id);

        // Assert
        repositorioFilmeMock.Verify(r => r.Excluir(novoFilme.Id), Times.Once);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Once);

        Assert.IsNotNull(resultadoExclusao);
        Assert.IsTrue(resultadoExclusao.IsSuccess);
    }

    [TestMethod]
    public void Excluir_Filme_Com_Excecao_Lancada_Deve_Retornar_Falha()
    {
        // Arrange
        Filme novoFilme = new("Esposa de Mentirinha", 117, true, generoPadrao);

        repositorioFilmeMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Filme>() { novoFilme });

        repositorioFilmeMock
            .Setup(r => r.Excluir(novoFilme.Id))
            .Throws(new Exception("Erro inesperado"));

        unitOfWorkMock
            .Setup(u => u.Commit())
            .Throws(new Exception("Erro na exclusão"));

        // Act
        Result resultadoExclusao = filmeAppService.Excluir(novoFilme.Id);

        // Assert
        unitOfWorkMock.Verify(u => u.Rollback(), Times.Once);

        string mensagemErro = resultadoExclusao.Errors[0].Message;

        Assert.IsNotNull(resultadoExclusao);
        Assert.IsTrue(resultadoExclusao.IsFailed);
        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
    }
    #endregion

    #region Testes Seleção por Id
    [TestMethod]
    public void Selecionar_Filme_Por_Id_Deve_Retornar_Sucesso()
    {
        // Arrange
        Filme novoFilme = new("Esposa de Mentirinha", 117, true, generoPadrao);

        repositorioFilmeMock
            .Setup(r => r.SelecionarRegistroPorId(novoFilme.Id))
            .Returns(novoFilme);

        // Act
        Result<Filme> resultadoSelecao = filmeAppService.SelecionarPorId(novoFilme.Id);

        Filme filmeSelecionado = resultadoSelecao.ValueOrDefault;

        // Assert
        repositorioFilmeMock.Verify(r => r.SelecionarRegistroPorId(novoFilme.Id), Times.Once);

        Assert.IsNotNull(resultadoSelecao);
        Assert.IsTrue(resultadoSelecao.IsSuccess);
        Assert.IsNotNull(filmeSelecionado);
        Assert.AreEqual(novoFilme, filmeSelecionado);
    }

    [TestMethod]
    public void Selecionar_Filme_Por_Id_Inexistente_Deve_Retornar_Falha()
    {
        // Arrange
        Filme novoFilme = new("Esposa de Mentirinha", 117, true, generoPadrao);

        repositorioFilmeMock
            .Setup(r => r.SelecionarRegistroPorId(novoFilme.Id))
            .Returns(novoFilme);

        // Act
        Result<Filme> resultadoSelecao = filmeAppService.SelecionarPorId(Guid.NewGuid());

        Filme filmeSelecionado = resultadoSelecao.ValueOrDefault;

        // Assert
        repositorioFilmeMock.Verify(r => r.SelecionarRegistroPorId(novoFilme.Id), Times.Never);

        string mensagemErro = resultadoSelecao.Errors[0].Message;

        Assert.IsNotNull(resultadoSelecao);
        Assert.IsTrue(resultadoSelecao.IsFailed);
        Assert.IsNull(filmeSelecionado);
        Assert.AreNotEqual(novoFilme, filmeSelecionado);
        Assert.AreEqual("Registro não encontrado", mensagemErro);
    }

    [TestMethod]
    public void Selecionar_Filme_Por_Id_Com_Excecao_Lancada_Deve_Retornar_Falha()
    {
        // Arrange
        Filme novoFilme = new("Esposa de Mentirinha", 117, true, generoPadrao);

        repositorioFilmeMock
            .Setup(r => r.SelecionarRegistroPorId(novoFilme.Id))
            .Throws(new Exception("Erro inesperado"));

        // Act
        Result<Filme> resultadoSelecao = filmeAppService.SelecionarPorId(novoFilme.Id);

        Filme filmeSelecionado = resultadoSelecao.ValueOrDefault;

        // Assert
        repositorioFilmeMock.Verify(r => r.SelecionarRegistroPorId(novoFilme.Id), Times.Once);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Never);

        string mensagemErro = resultadoSelecao.Errors[0].Message;

        Assert.IsNotNull(resultadoSelecao);
        Assert.IsTrue(resultadoSelecao.IsFailed);
        Assert.IsNull(filmeSelecionado);
        Assert.AreNotEqual(novoFilme, filmeSelecionado);
        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
    }
    #endregion

    #region Testes Seleção de Todos
    [TestMethod]
    public void Selecionar_Todos_Filmes_Deve_Retornar_Sucesso()
    {
        // Arrange
        Filme novoFilme = new("Esposa de Mentirinha", 117, true, generoPadrao);

        List<Filme> filmesExistentes = new()
        {
            novoFilme,
            new("Todo Mundo Tem A Irmã Gêmea Que Merece", 94, true, generoPadrao)
        };

        repositorioFilmeMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(filmesExistentes);

        // Act
        Result<List<Filme>> resultadosSelecao = filmeAppService.SelecionarTodos();

        List<Filme> filmesSelecionados = resultadosSelecao.ValueOrDefault;

        // Assert
        repositorioFilmeMock.Verify(r => r.SelecionarRegistros(), Times.Once);

        Assert.IsNotNull(resultadosSelecao);
        Assert.IsTrue(resultadosSelecao.IsSuccess);
        Assert.IsNotNull(filmesSelecionados);
        CollectionAssert.AreEquivalent(filmesExistentes, filmesSelecionados);
    }

    [TestMethod]
    public void Selecionar_Todos_Filmes_Com_Excecao_Lancada_Deve_Retornar_Falha()
    {
        // Arrange
        Filme novoFilme = new("Esposa de Mentirinha", 117, true, generoPadrao);

        List<Filme> filmesExistentes = new()
        {
            novoFilme,
            new("Todo Mundo Tem A Irmã Gêmea Que Merece", 94, true, generoPadrao)
        };

        repositorioFilmeMock
            .Setup(r => r.SelecionarRegistros())
            .Throws(new Exception("Erro inesperado"));

        // Act
        Result<List<Filme>> resultadosSelecao = filmeAppService.SelecionarTodos();

        List<Filme> filmesSelecionados = resultadosSelecao.ValueOrDefault;

        // Assert
        repositorioFilmeMock.Verify(r => r.SelecionarRegistros(), Times.Once);

        string mensagemErro = resultadosSelecao.Errors[0].Message;

        Assert.IsNotNull(resultadosSelecao);
        Assert.IsTrue(resultadosSelecao.IsFailed);
        Assert.IsNull(filmesSelecionados);
        CollectionAssert.AreNotEquivalent(filmesExistentes, filmesSelecionados);
        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
    }
    #endregion
}
