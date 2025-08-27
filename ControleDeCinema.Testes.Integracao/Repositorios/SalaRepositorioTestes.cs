using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Testes.Integracao.Compartilhado;

namespace ControleDeCinema.Testes.Integracao.Repositorios;

[TestClass]
[TestCategory("Testes de Integração de RepositorioSalaEmOrm")]
public sealed class SalaRepositorioTestes : TestFixture
{
    [TestMethod]
    public void Deve_Cadastrar_Sala_Corretamente()
    {
        // Arrange
        Sala novaSala = new(1, 24);

        // Act
        RepositorioSalaEmOrm.Cadastrar(novaSala);

        dbContext.SaveChanges();

        // Assert
        Sala? salaSelecionada = RepositorioSalaEmOrm.SelecionarRegistroPorId(novaSala.Id);

        Assert.IsNotNull(salaSelecionada);
        Assert.AreEqual(novaSala, salaSelecionada);
    }

    [TestMethod]
    public void Deve_Editar_Sala_Corretamente()
    {
        // Arrange
        Sala novaSala = new(1, 12);

        RepositorioSalaEmOrm.Cadastrar(novaSala);

        dbContext.SaveChanges();

        // Act
        Sala salaEditada = new(1, 24);

        bool conseguiuEditar = RepositorioSalaEmOrm.Editar(novaSala.Id, salaEditada);

        dbContext.SaveChanges();

        // Assert
        Sala? salaSelecionada = RepositorioSalaEmOrm.SelecionarRegistroPorId(novaSala.Id);

        Assert.IsTrue(conseguiuEditar);
        Assert.IsNotNull(salaSelecionada);
        Assert.AreEqual(novaSala, salaSelecionada);
    }

    [TestMethod]
    public void Deve_Excluir_Sala_Corretamente()
    {
        // Arrange
        Sala novaSala = new(1, 24);

        RepositorioSalaEmOrm.Cadastrar(novaSala);

        dbContext.SaveChanges();

        // Act
        bool conseguiuExcluir = RepositorioSalaEmOrm.Excluir(novaSala.Id);

        dbContext.SaveChanges();

        // Assert
        Sala? salaSelecionada = RepositorioSalaEmOrm.SelecionarRegistroPorId(novaSala.Id);

        Assert.IsTrue(conseguiuExcluir);
        Assert.IsNull(salaSelecionada);
    }

    [TestMethod]
    public void Deve_Selecionar_Sala_Por_Id_Corretamente()
    {
        // Arrange
        Sala novaSala = new(1, 24);

        RepositorioSalaEmOrm.Cadastrar(novaSala);

        dbContext.SaveChanges();

        // Act
        Sala? salaSelecionada = RepositorioSalaEmOrm.SelecionarRegistroPorId(novaSala.Id);

        // Assert
        Assert.IsNotNull(salaSelecionada);
        Assert.AreEqual(novaSala, salaSelecionada);
    }

    [TestMethod]
    public void Deve_Selecionar_Todas_Salas_Corretamente()
    {
        // Arrange
        List<Sala> novasSalas = new()
        {
            new(1, 24),
            new(2, 12),
            new(3, 36)
        };

        RepositorioSalaEmOrm.CadastrarEntidades(novasSalas);

        dbContext.SaveChanges();

        // Act
        List<Sala> salasExistentes = RepositorioSalaEmOrm.SelecionarRegistros();
        List<Sala> salasEsperadas = novasSalas;

        // Assert
        Assert.AreEqual(salasEsperadas.Count, salasExistentes.Count);
        CollectionAssert.AreEquivalent(salasEsperadas, salasExistentes);
    }
}
