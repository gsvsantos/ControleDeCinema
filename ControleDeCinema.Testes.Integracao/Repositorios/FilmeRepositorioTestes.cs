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
}
