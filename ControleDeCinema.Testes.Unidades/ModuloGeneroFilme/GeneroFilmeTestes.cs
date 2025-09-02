using ControleDeCinema.Dominio.ModuloGeneroFilme;

namespace ControleDeCinema.Testes.Unidades.ModuloGeneroFilme;

[TestClass]
[TestCategory("Testes de Unidade de Gênero de Filme (Domínio)")]
public class GeneroFilmeTestes
{
    private GeneroFilme? generoFilme;

    [TestMethod]
    public void Deve_Atualizar_Registro_GeneroFilme_Corretamente()
    {
        // Arrange
        generoFilme = new("Enmgrasadu");

        GeneroFilme generoEditado = new("Comédia");

        // Act
        generoFilme.AtualizarRegistro(generoEditado);

        // Assert
        Assert.AreEqual(generoEditado.Descricao, generoFilme.Descricao);
    }
}
