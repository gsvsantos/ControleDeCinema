using ControledeCinema.Dominio.Compartilhado;
using ControleDeCinema.Aplicacao.ModuloSala;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloSala;
using FluentResults;
using Microsoft.Extensions.Logging;
using Moq;

namespace ControleDeCinema.Testes.Unidades.ModuloSala;

[TestClass]
[TestCategory("Testes de Unidade de SalaAppService")]
public class SalaAppServiceTestes
{
    // SUT
    private SalaAppService salaAppService;

    // MOCK
    private Mock<ITenantProvider> tenantProviderMock;
    private Mock<IRepositorioSala> repositorioSalaMock;
    private Mock<IUnitOfWork> unitOfWorkMock;
    private Mock<ILogger<SalaAppService>> loggerMock;

    [TestInitialize]
    public void Setup()
    {
        tenantProviderMock = new Mock<ITenantProvider>();
        repositorioSalaMock = new Mock<IRepositorioSala>();
        unitOfWorkMock = new Mock<IUnitOfWork>();
        loggerMock = new Mock<ILogger<SalaAppService>>();

        salaAppService = new SalaAppService(
            tenantProviderMock.Object,
            repositorioSalaMock.Object,
            unitOfWorkMock.Object,
            loggerMock.Object
        );
    }

    #region Testes Cadastro
    [TestMethod]
    public void Cadastrar_Sala_Deve_Retornar_Sucesso()
    {
        // Arrange
        Sala novaSala = new(1, 15);

        repositorioSalaMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sala>());

        // Act
        Result resultadoCadastro = salaAppService.Cadastrar(novaSala);

        // Assert
        repositorioSalaMock.Verify(r => r.Cadastrar(novaSala), Times.Once);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Once);

        Assert.IsNotNull(resultadoCadastro);
        Assert.IsTrue(resultadoCadastro.IsSuccess);
    }

    [TestMethod]
    public void Cadastrar_Sala_Duplicada_Deve_Retornar_Falha()
    {
        // Arrange
        repositorioSalaMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sala>() { new(1, 30) });

        Sala novaSala = new(1, 15);

        // Act
        Result resultadoCadastro = salaAppService.Cadastrar(novaSala);

        // Assert
        repositorioSalaMock.Verify(r => r.Cadastrar(novaSala), Times.Never);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Never);

        string mensagemErro = resultadoCadastro.Errors[0].Message;

        Assert.IsNotNull(resultadoCadastro);
        Assert.IsTrue(resultadoCadastro.IsFailed);
        Assert.AreEqual("Registro duplicado", mensagemErro);
    }

    [TestMethod]
    public void Cadastrar_Sala_Com_Excecao_Lancada_Deve_Retornar_Falha()
    {
        // Arrange
        Sala novaSala = new(1, 15);

        repositorioSalaMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sala>());

        repositorioSalaMock
            .Setup(r => r.Cadastrar(novaSala))
            .Throws(new Exception("Erro inesperado"));

        unitOfWorkMock
            .Setup(u => u.Commit())
            .Throws(new Exception("Erro no cadastro"));

        // Act
        Result resultadoCadastro = salaAppService.Cadastrar(novaSala);

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
    public void Editar_Sala_Deve_Retornar_Sucesso()
    {
        // Arrange
        Sala novaSala = new(1, 15);

        repositorioSalaMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sala>() { novaSala });

        repositorioSalaMock
            .Setup(r => r.SelecionarRegistroPorId(novaSala.Id))
            .Returns(novaSala);

        Sala salaEditada = new(1, 30);

        // Act
        Result resultadoEdicao = salaAppService.Editar(novaSala.Id, salaEditada);

        // Assert
        repositorioSalaMock.Verify(r => r.Editar(novaSala.Id, salaEditada), Times.Once);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Once);

        Assert.IsNotNull(resultadoEdicao);
        Assert.IsTrue(resultadoEdicao.IsSuccess);
    }

    [TestMethod]
    public void Editar_Sala_Duplicada_Deve_Retornar_Falha()
    {
        // Arrange
        Sala novaSala = new(1, 15);

        List<Sala> salasExistentes = new()
        {
            novaSala,
            new(2, 30)
        };

        repositorioSalaMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(salasExistentes);

        Sala salaEditada = new(2, 15);

        // Act
        Result resultadoEdicao = salaAppService.Editar(novaSala.Id, salaEditada);

        // Assert
        repositorioSalaMock.Verify(r => r.Editar(novaSala.Id, salaEditada), Times.Never);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Never);

        string mensagemErro = resultadoEdicao.Errors[0].Message;

        Assert.IsNotNull(resultadoEdicao);
        Assert.IsTrue(resultadoEdicao.IsFailed);
        Assert.AreEqual("Registro duplicado", mensagemErro);
    }

    [TestMethod]
    public void Editar_Sala_Com_Excecao_Lancada_Deve_Retornar_Falha()
    {
        // Arrange
        Sala novaSala = new(1, 15);

        repositorioSalaMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sala>() { novaSala });

        Sala salaEditada = new(1, 30);

        repositorioSalaMock
            .Setup(r => r.Editar(novaSala.Id, salaEditada))
            .Throws(new Exception("Erro inesperado"));

        unitOfWorkMock
            .Setup(u => u.Commit())
            .Throws(new Exception("Erro na edição"));

        // Act
        Result resultadoEdicao = salaAppService.Editar(novaSala.Id, salaEditada);

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
    public void Excluir_Sala_Deve_Retornar_Sucesso()
    {
        // Arrange
        Sala novaSala = new(1, 15);

        repositorioSalaMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sala>() { novaSala });

        repositorioSalaMock
            .Setup(r => r.SelecionarRegistroPorId(novaSala.Id))
            .Returns(novaSala);

        // Act
        Result resultadoExclusao = salaAppService.Excluir(novaSala.Id);

        // Assert
        repositorioSalaMock.Verify(r => r.Excluir(novaSala.Id), Times.Once);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Once);

        Assert.IsNotNull(resultadoExclusao);
        Assert.IsTrue(resultadoExclusao.IsSuccess);
    }

    [TestMethod]
    public void Excluir_Sala_Inexistente_Deve_Retornar_Falha()
    {
        // Arrange
        Sala novaSala = new(1, 15);

        repositorioSalaMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sala>() { novaSala });

        repositorioSalaMock
            .Setup(r => r.Excluir(Guid.NewGuid()))
            .Returns(false);

        // Act
        Result resultadoExclusao = salaAppService.Excluir(novaSala.Id);

        // Assert
        unitOfWorkMock.Verify(u => u.Commit(), Times.Never);

        string mensagemErro = resultadoExclusao.Errors[0].Message;

        Assert.IsNotNull(resultadoExclusao);
        Assert.IsTrue(resultadoExclusao.IsFailed);
        Assert.AreEqual("Registro não encontrado", mensagemErro);
    }

    [TestMethod]
    public void Excluir_Sala_Com_Excecao_Lancada_Deve_Retornar_Falha()
    {
        // Arrange
        Sala novaSala = new(1, 15);

        repositorioSalaMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sala>() { novaSala });

        repositorioSalaMock
            .Setup(r => r.Excluir(novaSala.Id))
            .Throws(new Exception("Erro inesperado"));

        unitOfWorkMock
            .Setup(r => r.Commit())
            .Throws(new Exception("Erro na exclusão"));

        // Act
        Result resultadoExclusao = salaAppService.Excluir(novaSala.Id);

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
    public void Selecionar_Sala_Por_Id_Deve_Retornar_Sucesso()
    {
        // Arrange
        Sala novaSala = new(1, 15);

        repositorioSalaMock
            .Setup(r => r.SelecionarRegistroPorId(novaSala.Id))
            .Returns(novaSala);

        // Act
        Result<Sala> resultadoSelecao = salaAppService.SelecionarPorId(novaSala.Id);

        Sala salaSelecionada = resultadoSelecao.ValueOrDefault;

        // Assert
        repositorioSalaMock.Verify(r => r.SelecionarRegistroPorId(novaSala.Id), Times.Once);

        Assert.IsNotNull(resultadoSelecao);
        Assert.IsTrue(resultadoSelecao.IsSuccess);
        Assert.IsNotNull(salaSelecionada);
        Assert.AreEqual(novaSala, salaSelecionada);
    }

    [TestMethod]
    public void Selecionar_Sala_Por_Id_Inexistente_Deve_Retornar_Falha()
    {
        // Arrange
        Sala novaSala = new(1, 15);

        repositorioSalaMock
            .Setup(r => r.SelecionarRegistroPorId(novaSala.Id))
            .Returns(novaSala);

        // Act
        Result<Sala> resultadoSelecao = salaAppService.SelecionarPorId(Guid.NewGuid());

        Sala salaSelecionada = resultadoSelecao.ValueOrDefault;

        // Assert
        repositorioSalaMock.Verify(r => r.SelecionarRegistroPorId(novaSala.Id), Times.Never);

        string mensagemErro = resultadoSelecao.Errors[0].Message;

        Assert.IsNotNull(resultadoSelecao);
        Assert.IsTrue(resultadoSelecao.IsFailed);
        Assert.IsNull(salaSelecionada);
        Assert.AreNotEqual(novaSala, salaSelecionada);
        Assert.AreEqual("Registro não encontrado", mensagemErro);
    }

    [TestMethod]
    public void Selecionar_Sala_Por_Id_Com_Excecao_Lancada_Deve_Retornar_Falha()
    {
        // Arrange
        Sala novaSala = new(1, 15);

        repositorioSalaMock
            .Setup(r => r.SelecionarRegistroPorId(novaSala.Id))
            .Throws(new Exception("Erro inesperado"));

        // Act
        Result<Sala> resultadoSelecao = salaAppService.SelecionarPorId(novaSala.Id);

        Sala salaSelecionada = resultadoSelecao.ValueOrDefault;

        // Assert
        repositorioSalaMock.Verify(r => r.SelecionarRegistroPorId(novaSala.Id), Times.Once);

        string mensagemErro = resultadoSelecao.Errors[0].Message;

        Assert.IsNotNull(resultadoSelecao);
        Assert.IsTrue(resultadoSelecao.IsFailed);
        Assert.IsNull(salaSelecionada);
        Assert.AreNotEqual(novaSala, salaSelecionada);
        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
    }
    #endregion

    #region Testes Seleção de Todos
    [TestMethod]
    public void Selecionar_Todas_Salas_Deve_Retornar_Sucesso()
    {
        // Arrange
        Sala novaSala = new(1, 15);

        List<Sala> salasExistentes = new()
        {
            novaSala,
            new(2, 30)
        };

        repositorioSalaMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(salasExistentes);

        // Act
        Result<List<Sala>> resultadosSelecao = salaAppService.SelecionarTodos();

        List<Sala> salasSelecionadas = resultadosSelecao.ValueOrDefault;

        // Assert
        repositorioSalaMock.Verify(r => r.SelecionarRegistros(), Times.Once);

        Assert.IsNotNull(resultadosSelecao);
        Assert.IsTrue(resultadosSelecao.IsSuccess);
        Assert.IsNotNull(salasSelecionadas);
        CollectionAssert.AreEquivalent(salasExistentes, salasSelecionadas);
    }

    [TestMethod]
    public void Selecionar_Todas_Salas_Com_Excecao_Lancada_Deve_Retornar_Falha()
    {
        // Arrange
        Sala novaSala = new(1, 15);

        List<Sala> salasExistentes = new()
        {
            novaSala,
            new(2, 30)
        };

        repositorioSalaMock
            .Setup(r => r.SelecionarRegistros())
            .Throws(new Exception("Erro inesperado"));

        // Act
        Result<List<Sala>> resultadosSelecao = salaAppService.SelecionarTodos();

        List<Sala> salasSelecionadas = resultadosSelecao.ValueOrDefault;

        // Assert
        repositorioSalaMock.Verify(r => r.SelecionarRegistros(), Times.Once);

        string mensagemErro = resultadosSelecao.Errors[0].Message;

        Assert.IsNotNull(resultadosSelecao);
        Assert.IsTrue(resultadosSelecao.IsFailed);
        Assert.IsNull(salasSelecionadas);
        CollectionAssert.AreNotEquivalent(salasExistentes, salasSelecionadas);
        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
    }
    #endregion
}
