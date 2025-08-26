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
}
