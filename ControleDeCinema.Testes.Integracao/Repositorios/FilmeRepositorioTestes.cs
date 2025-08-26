using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Testes.Integracao.Compartilhado;
using FizzWare.NBuilder;

namespace ControleDeCinema.Testes.Integracao.Repositorios;

[TestClass]
[TestCategory("Testes de Integração de RepositorioFilmeEmOrm")]
public sealed class FilmeRepositorioTestes : TestFixture
{
    private GeneroFilme generoFilmePadrao = null!;

    [TestInitialize]
    public override void ConfigurarTestes()
    {
        base.ConfigurarTestes();

        generoFilmePadrao = Builder<GeneroFilme>.CreateNew()
            .With(d => d.Descricao = "Comedia").Persist();
    }

    [TestMethod]
    public void Deve_Cadastrar_Filme_Corretamente()
    {
        // Arrange
        Filme novoFilme = new("Esposa de Mentirinha", 117, true, generoFilmePadrao);

        // Act
        RepositorioFilmeEmOrm.Cadastrar(novoFilme);

        dbContext.SaveChanges();

        // Assert
        Filme? filmeSelecionado = RepositorioFilmeEmOrm.SelecionarRegistroPorId(novoFilme.Id);

        Assert.IsNotNull(filmeSelecionado);
        Assert.AreEqual(novoFilme, filmeSelecionado);
        Assert.AreEqual(generoFilmePadrao, filmeSelecionado.Genero);
    }

    [TestMethod]
    public void Deve_Editar_Filme_Corretamente()
    {
        // Arrange
        Filme novoFilme = new("Como Treinar Seu Dragão", 12, true, generoFilmePadrao);

        RepositorioFilmeEmOrm.Cadastrar(novoFilme);

        GeneroFilme novoGenero = Builder<GeneroFilme>.CreateNew()
            .With(g => g.Id = Guid.NewGuid())
            .With(d => d.Descricao = "Comedia Romantica").Persist();

        dbContext.SaveChanges();

        // Act
        Filme filmeEditado = new("Esposa de Mentirinha", 117, true, novoGenero);

        bool conseguiuEditar = RepositorioFilmeEmOrm.Editar(novoFilme.Id, filmeEditado);

        dbContext.SaveChanges();

        // Assert
        Filme? filmeSelecionado = RepositorioFilmeEmOrm.SelecionarRegistroPorId(novoFilme.Id);

        Assert.IsTrue(conseguiuEditar);
        Assert.IsNotNull(filmeSelecionado);
        Assert.AreEqual(novoFilme, filmeSelecionado);
    }

    [TestMethod]
    public void Deve_Excluir_Filme_Corretamente()
    {
        // Arrange
        Filme novoFilme = new("Esposa de Mentirinha", 117, true, generoFilmePadrao);

        RepositorioFilmeEmOrm.Cadastrar(novoFilme);

        dbContext.SaveChanges();

        // Act
        bool conseguiuExcluir = RepositorioFilmeEmOrm.Excluir(novoFilme.Id);

        dbContext.SaveChanges();

        // Assert
        Filme? filmeSelecionado = RepositorioFilmeEmOrm.SelecionarRegistroPorId(novoFilme.Id);

        Assert.IsTrue(conseguiuExcluir);
        Assert.IsNull(filmeSelecionado);
    }

    [TestMethod]
    public void Deve_Selecionar_Filme_Por_Id_Corretamente()
    {
        // Arrange
        Filme novoFilme = new("Esposa de Mentirinha", 117, true, generoFilmePadrao);

        RepositorioFilmeEmOrm.Cadastrar(novoFilme);

        dbContext.SaveChanges();

        // Act
        Filme? filmeSelecionado = RepositorioFilmeEmOrm.SelecionarRegistroPorId(novoFilme.Id);

        // Assert
        Assert.IsNotNull(filmeSelecionado);
        Assert.AreEqual(novoFilme, filmeSelecionado);
        Assert.AreEqual(generoFilmePadrao, filmeSelecionado.Genero);
    }

    [TestMethod]
    public void Deve_Selecionar_Todos_Filmes_Corretamente()
    {
        // Arrange
        List<GeneroFilme> novosGeneros = Builder<GeneroFilme>.CreateListOfSize(3)
            .All().With(g => g.Id = Guid.NewGuid()).Persist().ToList();

        List<Filme> novosFilmes = new()
        {
            new("Esposa de Mentirinha", 117, true, novosGeneros[0]),
            new("Cada um Tem A Gemea que Merece", 117, true, novosGeneros[1]),
            new("Gente Grande 2", 117, true, novosGeneros[2])
        };

        RepositorioFilmeEmOrm.CadastrarEntidades(novosFilmes);

        dbContext.SaveChanges();

        // Act
        List<Filme> filmesExistentes = RepositorioFilmeEmOrm.SelecionarRegistros();
        List<Filme> filmesEsperados = novosFilmes;

        // Assert
        Assert.AreEqual(filmesEsperados.Count, filmesExistentes.Count);
        CollectionAssert.AreEquivalent(filmesEsperados, filmesExistentes);
    }
}
