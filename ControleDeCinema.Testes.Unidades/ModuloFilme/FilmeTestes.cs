using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using FizzWare.NBuilder;

namespace ControleDeCinema.Testes.Unidades.ModuloFilme;

[TestClass]
[TestCategory("Testes de Unidade de Filme (Domínio)")]
public class FilmeTestes
{
    private Filme? filme;

    private GeneroFilme generoPadrao = Builder<GeneroFilme>.CreateNew()
        .WithFactory(() => new("Comédia")).Build();

    [TestMethod]
    public void Deve_Atualizar_Registro_Filme_Corretamente()
    {
        // Arrange
        filme = new("Gentih Grander 5", 121, true, new("Enmgrasadu"));

        Filme filmeEditado = new("Gente Grande 2", 101, false, generoPadrao);

        // Act
        filme.AtualizarRegistro(filmeEditado);

        // Assert
        Assert.AreEqual(filmeEditado.Titulo, filme.Titulo);
        Assert.AreEqual(filmeEditado.Duracao, filme.Duracao);
        Assert.AreEqual(filmeEditado.Lancamento, filme.Lancamento);
        Assert.AreEqual(filmeEditado.Genero, filme.Genero);
    }
}
