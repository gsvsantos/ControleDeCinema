using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Dominio.ModuloSessao;
using ControleDeCinema.Testes.Integracao.Compartilhado;
using FizzWare.NBuilder;

namespace ControleDeCinema.Testes.Integracao.Repositorios;

[TestClass]
[TestCategory("Testes de Integração de RepositorioSessaoEmOrm")]
public sealed class SessaoRepositorioTestes : TestFixture
{
    private GeneroFilme generoFilmePadrao = null!;
    private Filme filmePadrao = null!;
    private Sala salaPadrao = null!;

    [TestInitialize]
    public override void ConfigurarTestes()
    {
        base.ConfigurarTestes();

        generoFilmePadrao = Builder<GeneroFilme>.CreateNew().Persist();

        filmePadrao = Builder<Filme>.CreateNew()
            .With(f => f.Genero = generoFilmePadrao).Persist();

        salaPadrao = Builder<Sala>.CreateNew().Persist();
    }

    [TestMethod]
    public void Deve_Cadastrar_Sessao_Corretamente()
    {
        // Arrange
        Sessao novaSessao = new(new DateTime(2002, 8, 9, 0, 0, 0, DateTimeKind.Utc),
            12, filmePadrao, salaPadrao);

        // Act
        RepositorioSessaoEmOrm.Cadastrar(novaSessao);

        dbContext.SaveChanges();

        // Assert
        Sessao? sessaoSelecionada = RepositorioSessaoEmOrm.SelecionarRegistroPorId(novaSessao.Id);

        Assert.IsNotNull(sessaoSelecionada);
        Assert.AreEqual(novaSessao, sessaoSelecionada);
        Assert.AreEqual(filmePadrao, sessaoSelecionada.Filme);
        Assert.AreEqual(salaPadrao, sessaoSelecionada.Sala);
    }

    [TestMethod]
    public void Deve_Editar_Sessao_Corretamente()
    {
        // Arrange
        Sessao novaSessao = new(new DateTime(2005, 2, 3, 0, 0, 0, DateTimeKind.Utc),
            6, filmePadrao, salaPadrao);

        RepositorioSessaoEmOrm.Cadastrar(novaSessao);

        Filme novoFilme = Builder<Filme>.CreateNew()
            .With(f => f.Id = Guid.NewGuid())
            .With(f => f.Genero = generoFilmePadrao).Persist();

        Sala novaSala = Builder<Sala>.CreateNew()
            .With(s => s.Id = Guid.NewGuid()).Persist();

        dbContext.SaveChanges();

        // Act
        Sessao sessaoEditada = new(new DateTime(2002, 8, 9, 0, 0, 0, DateTimeKind.Utc),
            12, novoFilme, novaSala);

        bool conseguiuEditar = RepositorioSessaoEmOrm.Editar(novaSessao.Id, sessaoEditada);

        dbContext.SaveChanges();

        // Assert
        Sessao? sessaoSelecionada = RepositorioSessaoEmOrm.SelecionarRegistroPorId(novaSessao.Id);

        Assert.IsTrue(conseguiuEditar);
        Assert.IsNotNull(sessaoSelecionada);
        Assert.AreEqual(novaSessao, sessaoSelecionada);
    }

    [TestMethod]
    public void Deve_Excluir_Sessao_Corretamente()
    {
        // Arrange
        Sessao novaSessao = new(new DateTime(2002, 8, 9, 0, 0, 0, DateTimeKind.Utc),
            12, filmePadrao, salaPadrao);

        RepositorioSessaoEmOrm.Cadastrar(novaSessao);

        dbContext.SaveChanges();

        // Act
        bool conseguiuExcluir = RepositorioSessaoEmOrm.Excluir(novaSessao.Id);

        dbContext.SaveChanges();

        // Assert
        Sessao? sessaoSelecionada = RepositorioSessaoEmOrm.SelecionarRegistroPorId(novaSessao.Id);

        Assert.IsTrue(conseguiuExcluir);
        Assert.IsNull(sessaoSelecionada);
    }

    [TestMethod]
    public void Deve_Selecionar_Sessao_Por_Id_Corretamente()
    {
        // Arrange
        Sessao novaSessao = new(new DateTime(2002, 8, 9, 0, 0, 0, DateTimeKind.Utc),
            12, filmePadrao, salaPadrao);

        RepositorioSessaoEmOrm.Cadastrar(novaSessao);

        dbContext.SaveChanges();

        // Act
        Sessao? sessaoSelecionada = RepositorioSessaoEmOrm.SelecionarRegistroPorId(novaSessao.Id);

        // Assert
        Assert.IsNotNull(sessaoSelecionada);
        Assert.AreEqual(novaSessao, sessaoSelecionada);
        Assert.IsNotNull(sessaoSelecionada.Filme);
        Assert.IsNotNull(sessaoSelecionada.Sala);
        Assert.AreEqual(filmePadrao, sessaoSelecionada.Filme);
        Assert.AreEqual(salaPadrao, sessaoSelecionada.Sala);
    }

    [TestMethod]
    public void Deve_Selecionar_Todas_Sessoes_Corretamente()
    {
        // Arrange
        List<GeneroFilme> novosGeneros = Builder<GeneroFilme>.CreateListOfSize(3)
            .All().With(g => g.Id = Guid.NewGuid()).Persist().ToList();

        List<Filme> novosFilmes = Builder<Filme>.CreateListOfSize(3)
            .All()
            .DoForEach((f, g) => f.Genero = g, novosGeneros)
            .With(f => f.Id = Guid.NewGuid()).Persist().ToList();

        List<Sala> novasSalas = Builder<Sala>.CreateListOfSize(3)
            .All().With(g => g.Id = Guid.NewGuid()).Persist().ToList();

        List<Sessao> novasSessoes = new()
        {
            new(new DateTime(2002, 8, 9, 0, 0, 0, DateTimeKind.Utc),
            12, novosFilmes[0], novasSalas[0]),
            new(new DateTime(2023, 4, 2, 0, 0, 0, DateTimeKind.Utc),
            15, novosFilmes[1], novasSalas[1]),
            new(new DateTime(2012, 1, 6, 0, 0, 0, DateTimeKind.Utc),
            21, novosFilmes[2], novasSalas[2])
        };

        RepositorioSessaoEmOrm.CadastrarEntidades(novasSessoes);

        dbContext.SaveChanges();

        // Act
        List<Sessao> sessoesExistentes = RepositorioSessaoEmOrm.SelecionarRegistros();
        List<Sessao> sessoesEsperadas = novasSessoes;

        // Assert
        Assert.AreEqual(sessoesEsperadas.Count, sessoesExistentes.Count);
        CollectionAssert.AreEquivalent(sessoesEsperadas, sessoesExistentes);
    }

    [TestMethod]
    public void Deve_Selecionar_Todas_Sessoes_Do_Usuario_Corretamente()
    {
        // Arrange
        Guid usuarioId = Guid.NewGuid();

        List<GeneroFilme> novosGeneros = Builder<GeneroFilme>.CreateListOfSize(3)
            .All().With(g => g.Id = Guid.NewGuid()).Persist().ToList();

        List<Filme> novosFilmes = Builder<Filme>.CreateListOfSize(3)
            .All()
            .DoForEach((f, g) => f.Genero = g, novosGeneros)
            .With(f => f.Id = Guid.NewGuid()).Persist().ToList();

        List<Sala> novasSalas = Builder<Sala>.CreateListOfSize(3)
            .All().With(g => g.Id = Guid.NewGuid()).Persist().ToList();

        List<Sessao> sessoesDoUsuario = new()
        {
            new(new DateTime(2002, 8, 9, 0, 0, 0, DateTimeKind.Utc),
            12, novosFilmes[0], novasSalas[0])
            { UsuarioId = usuarioId },
            new(new DateTime(2023, 4, 2, 0, 0, 0, DateTimeKind.Utc),
            15, novosFilmes[1], novasSalas[1])
            { UsuarioId = usuarioId },
            new(new DateTime(2012, 1, 6, 0, 0, 0, DateTimeKind.Utc),
            21, novosFilmes[2], novasSalas[2])
            { UsuarioId = usuarioId }
        };

        RepositorioSessaoEmOrm.CadastrarEntidades(sessoesDoUsuario);

        List<Sessao> sessoesDeOutros = Builder<Sessao>.CreateListOfSize(3)
            .All()
            .With(s => s.Inicio = new DateTime(2010, 1, 1, 0, 0, 0, DateTimeKind.Utc))
            .DoForEach((s, f) => s.Filme = f, novosFilmes)
            .DoForEach((s, sa) => s.Sala = sa, novasSalas)
            .With(s => s.UsuarioId = Guid.NewGuid()).Persist().ToList();

        dbContext.SaveChanges();

        // Act
        List<Sessao> sessoesExistentes = RepositorioSessaoEmOrm.SelecionarRegistros();
        List<Sessao> sessoesFiltradas = RepositorioSessaoEmOrm.SelecionarRegistrosDoUsuario(usuarioId);
        List<Sessao> sessoesEsperadas = sessoesDoUsuario;

        // Assert
        Assert.AreEqual(sessoesEsperadas.Count, sessoesFiltradas.Count);
        CollectionAssert.AreEquivalent(sessoesEsperadas, sessoesFiltradas);
    }
}
