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
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

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

    #region Testes Edição
    [TestMethod]
    public void Editar_Sessao_Deve_Retornar_Sucesso()
    {
        // Arrange
        Sala novaSala = Builder<Sala>.CreateNew()
        .WithFactory(() => new Sala(2, 15) { Id = Guid.NewGuid() })
        .Build();

        DateTime novoInicio = new(2025, 08, 09, 16, 26, 00);
        Sessao novaSessao = new(novoInicio, 15, filmePadrao, novaSala);

        repositorioSessaoMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sessao>() { novaSessao });

        repositorioSessaoMock
            .Setup(r => r.SelecionarRegistroPorId(novaSessao.Id))
            .Returns(novaSessao);

        Sessao sessaoEditada = new(inicioPadrao, 30, filmePadrao, salaPadrao);

        repositorioSessaoMock
            .Setup(r => r.Editar(novaSessao.Id, sessaoEditada))
            .Returns(true);

        // Act
        Result resultadoEdicao = sessaoAppService.Editar(novaSessao.Id, sessaoEditada);

        // Assert
        repositorioSessaoMock.Verify(r => r.Editar(novaSessao.Id, sessaoEditada), Times.Once);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Once);

        Assert.IsNotNull(resultadoEdicao);
        Assert.IsTrue(resultadoEdicao.IsSuccess);
    }

    [TestMethod]
    public void Editar_Sessao_Com_Capacidade_Excedida_Deve_Retornar_Falha()
    {
        // Arrange  (salaPadrao capacidade = 30)
        Sessao novaSessao = new(inicioPadrao, 15, filmePadrao, salaPadrao);

        repositorioSessaoMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sessao>() { novaSessao });

        Sessao sessaoEditada = new(inicioPadrao, 40, filmePadrao, salaPadrao);

        // Act
        Result resultadoEdicao = sessaoAppService.Editar(novaSessao.Id, sessaoEditada);

        // Assert
        repositorioSessaoMock.Verify(r => r.Editar(novaSessao.Id, sessaoEditada), Times.Never);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Never);

        string mensagemErro = resultadoEdicao.Errors[0].Message;

        Assert.IsNotNull(resultadoEdicao);
        Assert.IsTrue(resultadoEdicao.IsFailed);
        Assert.AreEqual("Registro duplicado", mensagemErro);
    }

    [TestMethod]
    public void Editar_Sessao_Duplicada_Mesma_Sala_E_Horario_Deve_Retornar_Falha()
    {
        // Arrange  (filmePadrao tem 1h57 minutos, inicioPadrao é as 14:30 | 16:26 nega edição, 16:27 aceita edição)
        Sessao novaSessao = new(inicioPadrao, 15, filmePadrao, salaPadrao);

        DateTime novoInicio = new(2025, 08, 09, 16, 26, 00);

        List<Sessao> sessoesExistentes = new()
        {
            novaSessao,
            new Sessao(novoInicio, 30, filmePadrao, salaPadrao)
        };

        repositorioSessaoMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(sessoesExistentes);

        Sessao sessaoEditada = new(novoInicio, 30, filmePadrao, salaPadrao);

        // Act
        Result resultadoEdicao = sessaoAppService.Editar(novaSessao.Id, sessaoEditada);

        // Assert
        repositorioSessaoMock.Verify(r => r.Editar(novaSessao.Id, sessaoEditada), Times.Never);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Never);

        string mensagemErro = resultadoEdicao.Errors[0].Message;

        Assert.IsNotNull(resultadoEdicao);
        Assert.IsTrue(resultadoEdicao.IsFailed);
        Assert.AreEqual("Registro duplicado", mensagemErro);
    }

    [TestMethod]
    public void Editar_Sessao_Inexistente_Deve_Retornar_Falha()
    {
        // Arrange

        DateTime novoInicio = new(2025, 08, 09, 16, 27, 00);
        Sessao novaSessao = new(novoInicio, 15, filmePadrao, salaPadrao);

        repositorioSessaoMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sessao>() { novaSessao });

        Sessao sessaoEditada = new(inicioPadrao, 30, filmePadrao, salaPadrao);

        repositorioSessaoMock
            .Setup(r => r.Editar(Guid.NewGuid(), sessaoEditada))
            .Returns(false);

        // Act
        Result resultadoEdicao = sessaoAppService.Editar(Guid.NewGuid(), sessaoEditada);

        // Assert
        unitOfWorkMock.Verify(u => u.Commit(), Times.Never);

        string mensagemErro = resultadoEdicao.Errors[0].Message;

        Assert.IsNotNull(resultadoEdicao);
        Assert.IsTrue(resultadoEdicao.IsFailed);
        Assert.AreEqual("Registro não encontrado", mensagemErro);
    }

    [TestMethod]
    public void Editar_Sessao_Com_Excecao_Lancada_Deve_Retornar_Falha()
    {
        // Arrange
        Sessao novaSessao = new(inicioPadrao, 15, filmePadrao, salaPadrao);

        repositorioSessaoMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sessao>() { novaSessao });

        Sessao sessaoEditada = new(inicioPadrao, 30, filmePadrao, salaPadrao);

        repositorioSessaoMock
            .Setup(r => r.Editar(novaSessao.Id, sessaoEditada))
            .Throws(new Exception("Erro inesperado"));

        unitOfWorkMock
            .Setup(u => u.Commit())
            .Throws(new Exception("Erro na edição"));

        // Act
        Result resultadoEdicao = sessaoAppService.Editar(novaSessao.Id, sessaoEditada);

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
    public void Excluir_Sessao_Deve_Retornar_Sucesso()
    {
        // Arrange
        Sessao novaSessao = new(inicioPadrao, 30, filmePadrao, salaPadrao);

        repositorioSessaoMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sessao>() { novaSessao });

        repositorioSessaoMock
            .Setup(r => r.SelecionarRegistroPorId(novaSessao.Id))
            .Returns(novaSessao);

        repositorioSessaoMock
            .Setup(r => r.Excluir(novaSessao.Id))
            .Returns(true);

        // Act
        Result resultadoExclusao = sessaoAppService.Excluir(novaSessao.Id);

        // Assert
        repositorioSessaoMock.Verify(r => r.Excluir(novaSessao.Id), Times.Once);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Once);

        Assert.IsNotNull(resultadoExclusao);
        Assert.IsTrue(resultadoExclusao.IsSuccess);
    }

    [TestMethod]
    public void Excluir_Sessao_Inexistente_Deve_Retornar_Falha()
    {
        // Arrange
        Sessao novaSessao = new(inicioPadrao, 30, filmePadrao, salaPadrao);

        repositorioSessaoMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sessao>() { novaSessao });

        repositorioSessaoMock
            .Setup(r => r.Excluir(Guid.NewGuid()))
            .Returns(false);

        // Act
        Result resultadoExclusao = sessaoAppService.Excluir(Guid.NewGuid());

        // Assert
        unitOfWorkMock.Verify(u => u.Commit(), Times.Never);

        string mensagemErro = resultadoExclusao.Errors[0].Message;

        Assert.IsNotNull(resultadoExclusao);
        Assert.IsTrue(resultadoExclusao.IsFailed);
        Assert.AreEqual("Registro não encontrado", mensagemErro);
    }

    [TestMethod]
    public void Excluir_Sessao_Com_Excecao_Lancada_Deve_Retornar_Falha()
    {
        // Arrange
        Sessao novaSessao = new(inicioPadrao, 30, filmePadrao, salaPadrao);

        repositorioSessaoMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sessao>() { novaSessao });

        repositorioSessaoMock
            .Setup(r => r.Excluir(novaSessao.Id))
            .Throws(new Exception("Erro inesperado"));

        unitOfWorkMock
            .Setup(u => u.Commit())
            .Throws(new Exception("Erro na exclusão"));

        // Act
        Result resultadoExclusao = sessaoAppService.Excluir(Guid.NewGuid());

        // Assert
        unitOfWorkMock.Verify(u => u.Commit(), Times.Never);

        string mensagemErro = resultadoExclusao.Errors[0].Message;

        Assert.IsNotNull(resultadoExclusao);
        Assert.IsTrue(resultadoExclusao.IsFailed);
        Assert.AreEqual("Registro não encontrado", mensagemErro);
    }
    #endregion

    #region Testes Seleção por Id
    [TestMethod]
    public void Selecionar_Sessao_Por_Id_Deve_Retornar_Sucesso()
    {
        // Arrange
        Sessao novaSessao = new(inicioPadrao, 30, filmePadrao, salaPadrao);

        repositorioSessaoMock
            .Setup(r => r.SelecionarRegistroPorId(novaSessao.Id))
            .Returns(novaSessao);

        // Act
        Result<Sessao> resultadoSelecao = sessaoAppService.SelecionarPorId(novaSessao.Id);

        Sessao sessaoSelecionada = resultadoSelecao.ValueOrDefault;

        // Assert
        repositorioSessaoMock.Verify(r => r.SelecionarRegistroPorId(novaSessao.Id), Times.Once);

        Assert.IsNotNull(resultadoSelecao);
        Assert.IsTrue(resultadoSelecao.IsSuccess);
        Assert.IsNotNull(sessaoSelecionada);
        Assert.AreEqual(novaSessao, sessaoSelecionada);
    }

    [TestMethod]
    public void Selecionar_Sessao_Por_Id_Inexistente_Deve_Retornar_Falha()
    {
        // Arrange
        Sessao novaSessao = new(inicioPadrao, 30, filmePadrao, salaPadrao);

        repositorioSessaoMock
            .Setup(r => r.SelecionarRegistroPorId(novaSessao.Id))
            .Returns(novaSessao);

        // Act
        Result<Sessao> resultadoSelecao = sessaoAppService.SelecionarPorId(Guid.NewGuid());

        Sessao sessaoSelecionada = resultadoSelecao.ValueOrDefault;

        // Assert
        repositorioSessaoMock.Verify(r => r.SelecionarRegistroPorId(novaSessao.Id), Times.Never);

        string mensagemErro = resultadoSelecao.Errors[0].Message;

        Assert.IsNotNull(resultadoSelecao);
        Assert.IsTrue(resultadoSelecao.IsFailed);
        Assert.IsNull(sessaoSelecionada);
        Assert.AreNotEqual(novaSessao, sessaoSelecionada);
        Assert.AreEqual("Registro não encontrado", mensagemErro);
    }

    [TestMethod]
    public void Selecionar_Sessao_Por_Id_Com_Excecao_Lancada_Deve_Retornar_Falha()
    {
        // Arrange
        Sessao novaSessao = new(inicioPadrao, 30, filmePadrao, salaPadrao);

        repositorioSessaoMock
            .Setup(r => r.SelecionarRegistroPorId(novaSessao.Id))
            .Throws(new Exception("Erro inesperado"));

        // Act
        Result<Sessao> resultadoSelecao = sessaoAppService.SelecionarPorId(novaSessao.Id);

        Sessao sessaoSelecionada = resultadoSelecao.ValueOrDefault;

        // Assert
        repositorioSessaoMock.Verify(r => r.SelecionarRegistroPorId(novaSessao.Id), Times.Once);

        string mensagemErro = resultadoSelecao.Errors[0].Message;

        Assert.IsNotNull(resultadoSelecao);
        Assert.IsTrue(resultadoSelecao.IsFailed);
        Assert.IsNull(sessaoSelecionada);
        Assert.AreNotEqual(novaSessao, sessaoSelecionada);
        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
    }
    #endregion

    #region Testes Seleção de Todos (Cargos)
    [TestMethod]
    public void Selecionar_Todas_Sessoes_Para_Role_Empresa_Deve_Trazer_Apenas_Do_Usuario()
    {
        // Arrange 
        Guid usuarioId = Guid.NewGuid();
        Guid outroUsuarioId = Guid.NewGuid();

        tenantProviderMock
            .Setup(t => t.IsInRole("Empresa"))
            .Returns(true);

        tenantProviderMock
            .Setup(t => t.IsInRole("Cliente"))
            .Returns(false);

        tenantProviderMock
            .SetupGet(t => t.UsuarioId)
            .Returns(usuarioId);

        DateTime novoInicio = new(2025, 08, 09, 12, 12, 00);

        List<Sessao> sessoesExistentes = new()
        {
            new (inicioPadrao, 30, filmePadrao, salaPadrao) { UsuarioId = usuarioId },
            new Sessao(novoInicio, 30, filmePadrao, salaPadrao) { UsuarioId = outroUsuarioId }
        };

        List<Sessao> sessoesDoUsuario = sessoesExistentes.Where(s => s.UsuarioId.Equals(usuarioId)).ToList();

        repositorioSessaoMock
            .Setup(r => r.SelecionarRegistrosDoUsuario(usuarioId))
            .Returns(sessoesDoUsuario);

        // Act
        Result<List<Sessao>> resultadosSelecao = sessaoAppService.SelecionarTodos();

        List<Sessao> sessoesSelecionadas = resultadosSelecao.ValueOrDefault;

        // Assert
        repositorioSessaoMock.Verify(r => r.SelecionarRegistrosDoUsuario(usuarioId), Times.Once);
        repositorioSessaoMock.Verify(r => r.SelecionarRegistros(), Times.Never);

        Assert.IsNotNull(resultadosSelecao);
        Assert.IsTrue(resultadosSelecao.IsSuccess);
        Assert.IsTrue(sessoesSelecionadas.All(s => s.UsuarioId.Equals(usuarioId)));
        CollectionAssert.AreEquivalent(sessoesDoUsuario, sessoesSelecionadas);
    }

    [TestMethod]
    public void Selecionar_Todas_Sessoes_Para_Role_Cliente_Deve_Trazer_Todos()
    {
        // Arrange 
        Guid usuarioId = Guid.NewGuid();
        Guid outroUsuarioId = Guid.NewGuid();

        tenantProviderMock
            .Setup(t => t.IsInRole("Empresa"))
            .Returns(false);

        tenantProviderMock
            .Setup(t => t.IsInRole("Cliente"))
            .Returns(true);

        DateTime novoInicio = new(2025, 08, 09, 12, 12, 00);
        DateTime novoInicio2 = new(2025, 08, 09, 22, 22, 00);

        List<Sessao> sessoesExistentes = new()
        {
            new (inicioPadrao, 30, filmePadrao, salaPadrao) { UsuarioId = usuarioId },
            new (novoInicio2, 30, filmePadrao, salaPadrao) { UsuarioId = usuarioId },
            new Sessao(novoInicio, 30, filmePadrao, salaPadrao) { UsuarioId = outroUsuarioId }
        };

        repositorioSessaoMock
            .Setup(r => r.SelecionarRegistros())
            .Returns(sessoesExistentes);

        // Act
        Result<List<Sessao>> resultadosSelecao = sessaoAppService.SelecionarTodos();

        List<Sessao> sessoesSelecionadas = resultadosSelecao.ValueOrDefault;

        // Assert
        repositorioSessaoMock.Verify(r => r.SelecionarRegistrosDoUsuario(usuarioId), Times.Never);
        repositorioSessaoMock.Verify(r => r.SelecionarRegistros(), Times.Once);

        Assert.IsNotNull(resultadosSelecao);
        Assert.IsTrue(resultadosSelecao.IsSuccess);
        CollectionAssert.AreEquivalent(sessoesExistentes, sessoesSelecionadas);
    }

    [TestMethod]
    public void Selecionar_Todas_Sessoes_Com_Excecao_Lancada_Deve_Retornar_Falha()
    {
        // Arrange
        Guid usuarioId = Guid.NewGuid();
        Guid outroUsuarioId = Guid.NewGuid();

        tenantProviderMock
            .Setup(t => t.IsInRole("Empresa"))
            .Returns(false);

        tenantProviderMock
            .Setup(t => t.IsInRole("Cliente"))
            .Returns(true);

        repositorioSessaoMock
            .Setup(r => r.SelecionarRegistros())
            .Throws(new Exception("Erro inesperado"));

        DateTime novoInicio = new(2025, 08, 09, 12, 12, 00);
        DateTime novoInicio2 = new(2025, 08, 09, 22, 22, 00);

        List<Sessao> sessoesExistentes = new()
        {
            new (inicioPadrao, 30, filmePadrao, salaPadrao) { UsuarioId = usuarioId },
            new (novoInicio2, 30, filmePadrao, salaPadrao) { UsuarioId = usuarioId },
            new Sessao(novoInicio, 30, filmePadrao, salaPadrao) { UsuarioId = outroUsuarioId }
        };

        // Act
        Result<List<Sessao>> resultadosSelecao = sessaoAppService.SelecionarTodos();

        List<Sessao> sessoesSelecionadas = resultadosSelecao.ValueOrDefault;

        // Assert
        repositorioSessaoMock.Verify(r => r.SelecionarRegistros(), Times.Once);

        string mensagemErro = resultadosSelecao.Errors[0].Message;

        Assert.IsNotNull(resultadosSelecao);
        Assert.IsTrue(resultadosSelecao.IsFailed);
        Assert.IsNull(sessoesSelecionadas);
        CollectionAssert.AreNotEquivalent(sessoesExistentes, sessoesSelecionadas);
        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
    }
    #endregion

    #region Testes Encerrar
    [TestMethod]
    public void Encerrar_Sessao_Deve_Retornar_Sucesso()
    {
        // Arrange
        Sessao novaSessao = new(inicioPadrao, 30, filmePadrao, salaPadrao);

        repositorioSessaoMock
            .Setup(r => r.SelecionarRegistroPorId(novaSessao.Id))
            .Returns(novaSessao);

        // Act
        Result resultadoEncerramento = sessaoAppService.Encerrar(novaSessao.Id);

        // Assert
        repositorioSessaoMock.Verify(r => r.SelecionarRegistroPorId(novaSessao.Id), Times.Once);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Once);

        Assert.IsNotNull(resultadoEncerramento);
        Assert.IsTrue(resultadoEncerramento.IsSuccess);
        Assert.IsTrue(novaSessao.Encerrada);
    }

    [TestMethod]
    public void Encerrar_Sessao_Inexistente_Deve_Retornar_Falha()
    {
        // Arrange
        Sessao novaSessao = new(inicioPadrao, 30, filmePadrao, salaPadrao);

        repositorioSessaoMock
            .Setup(r => r.SelecionarRegistroPorId(novaSessao.Id))
            .Returns(novaSessao);

        // Act
        Result resultadoEncerramento = sessaoAppService.Encerrar(Guid.NewGuid());

        // Assert
        repositorioSessaoMock.Verify(r => r.SelecionarRegistroPorId(novaSessao.Id), Times.Never);
        unitOfWorkMock.Verify(u => u.Commit(), Times.Never);

        string mensagemErro = resultadoEncerramento.Errors[0].Message;

        Assert.IsNotNull(resultadoEncerramento);
        Assert.IsTrue(resultadoEncerramento.IsFailed);
        Assert.AreEqual("Registro não encontrado", mensagemErro);
    }

    [TestMethod]
    public void Encerrar_Sessao_Com_Excecao_Lancada_Deve_Retornar_Falha()
    {
        // Arrange
        Sessao novaSessao = new(inicioPadrao, 30, filmePadrao, salaPadrao);

        repositorioSessaoMock
            .Setup(r => r.SelecionarRegistroPorId(novaSessao.Id))
            .Throws(new Exception("Erro inesperado"));

        // Act
        Result resultadoEncerramento = sessaoAppService.Encerrar(novaSessao.Id);

        // Assert
        unitOfWorkMock.Verify(u => u.Rollback(), Times.Once);

        string mensagemErro = resultadoEncerramento.Errors[0].Message;

        Assert.IsNotNull(resultadoEncerramento);
        Assert.IsTrue(resultadoEncerramento.IsFailed);
        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
    }
    #endregion

    #region Testes Venda de Ingresso
    [TestMethod]
    public void VenderIngresso_Deve_Retornar_Sucesso()
    {
        // Arrange  (sessao válida, lugar livre, não encerrada, disponibilidade > 0)

        // Act

        // Assert
    }

    [TestMethod]
    public void VenderIngresso_Sessao_Inexistente_Deve_Retornar_Falha()
    {
        // Arrange  (repositório retorna null)

        // Act

        // Assert
    }

    [TestMethod]
    public void VenderIngresso_Sessao_Encerrada_Deve_Retornar_Falha()
    {
        // Arrange  (sessao.Encerrada == true)

        // Act

        // Assert
    }

    [TestMethod]
    public void VenderIngresso_Assento_Invalido_Deve_Retornar_Falha()
    {
        // Arrange  (assento < 1 ou > NumeroMaximoIngressos)

        // Act

        // Assert
    }

    [TestMethod]
    public void VenderIngresso_Assento_Ocupado_Deve_Retornar_Falha()
    {
        // Arrange  (sessao.Ingressos contém assento)

        // Act

        // Assert
    }

    [TestMethod]
    public void VenderIngresso_Sessao_Lotada_Deve_Retornar_Falha()
    {
        // Arrange  (ObterQuantidadeIngressosDisponiveis() == 0)

        // Act

        // Assert
    }

    [TestMethod]
    public void VenderIngresso_Com_Excecao_Lancada_Deve_Retornar_Falha()
    {
        // Arrange

        // Act

        // Assert
    }
    #endregion
}
