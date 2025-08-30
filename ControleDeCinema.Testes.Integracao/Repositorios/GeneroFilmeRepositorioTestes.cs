using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Testes.Integracao.Compartilhado;

namespace ControleDeCinema.Testes.Integracao.Repositorios;

[TestClass]
[TestCategory("Testes de Integração de RepositorioGeneroFilmeEmOrm")]
public sealed class GeneroFilmeRepositorioTestes : TestFixture
{
    [TestMethod]
    public void Deve_Cadastrar_GeneroFilme_Corretamente()
    {
        // Arrange
        GeneroFilme novoGenero = new("Comedia");

        // Act
        RepositorioGeneroFilmeEmOrm.Cadastrar(novoGenero);

        dbContext.SaveChanges();

        // Assert
        GeneroFilme? generoSelecionado = RepositorioGeneroFilmeEmOrm.SelecionarRegistroPorId(novoGenero.Id);

        Assert.IsNotNull(generoSelecionado);
        Assert.AreEqual(novoGenero, generoSelecionado);
    }

    [TestMethod]
    public void Deve_Editar_GeneroFilme_Corretamente()
    {
        // Arrange
        GeneroFilme novoGenero = new("Comedia");

        RepositorioGeneroFilmeEmOrm.Cadastrar(novoGenero);

        dbContext.SaveChanges();

        // Act
        GeneroFilme generoEditado = new("Romance");

        bool conseguiuEditar = RepositorioGeneroFilmeEmOrm.Editar(novoGenero.Id, generoEditado);

        dbContext.SaveChanges();

        // Assert
        GeneroFilme? generoSelecionado = RepositorioGeneroFilmeEmOrm.SelecionarRegistroPorId(novoGenero.Id);

        Assert.IsTrue(conseguiuEditar);
        Assert.IsNotNull(generoSelecionado);
        Assert.AreEqual(novoGenero, generoSelecionado);
    }

    [TestMethod]
    public void Deve_Excluir_GeneroFilme_Corretamente()
    {
        // Arrange
        GeneroFilme novoGenero = new("Comedia");

        RepositorioGeneroFilmeEmOrm.Cadastrar(novoGenero);

        dbContext.SaveChanges();

        // Act
        bool conseguiuExcluir = RepositorioGeneroFilmeEmOrm.Excluir(novoGenero.Id);

        dbContext.SaveChanges();

        // Assert
        GeneroFilme? generoSelecionado = RepositorioGeneroFilmeEmOrm.SelecionarRegistroPorId(novoGenero.Id);

        Assert.IsTrue(conseguiuExcluir);
        Assert.IsNull(generoSelecionado);
    }

    [TestMethod]
    public void Deve_Selecionar_GeneroFilme_Por_Id_Corretamente()
    {
        // Arrange
        GeneroFilme novoGenero = new("Comedia");

        RepositorioGeneroFilmeEmOrm.Cadastrar(novoGenero);

        dbContext.SaveChanges();

        // Act
        GeneroFilme? generoSelecionado = RepositorioGeneroFilmeEmOrm.SelecionarRegistroPorId(novoGenero.Id);

        // Assert
        Assert.IsNotNull(generoSelecionado);
        Assert.AreEqual(novoGenero, generoSelecionado);
    }

    [TestMethod]
    public void Deve_Selecionar_Todos_GeneroFilme_Corretamente()
    {
        // Arrange
        List<GeneroFilme> novosGeneros = new()
        {
            new("Comedia"),
            new("Romance"),
            new("Comedia Romantica")
        };

        RepositorioGeneroFilmeEmOrm.CadastrarEntidades(novosGeneros);

        dbContext.SaveChanges();

        // Act
        List<GeneroFilme> generosExistentes = RepositorioGeneroFilmeEmOrm.SelecionarRegistros();
        List<GeneroFilme> generosEsperados = novosGeneros;

        // Assert
        Assert.AreEqual(generosEsperados.Count, generosExistentes.Count);
        CollectionAssert.AreEquivalent(generosEsperados, generosExistentes);
    }
}
