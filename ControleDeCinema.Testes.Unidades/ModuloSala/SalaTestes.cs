using ControleDeCinema.Dominio.ModuloSala;

namespace ControleDeCinema.Testes.Unidades.ModuloSala;

[TestClass]
[TestCategory("Testes de Unidade de Sala (Domínio)")]
public class SalaTestes
{
    private Sala? sala;

    [TestMethod]
    public void Deve_Atualizar_Registro_Filme_Corretamente()
    {
        // Arrange
        sala = new(1, 15);

        Sala salaEditado = new(2, 30);

        // Act
        sala.AtualizarRegistro(salaEditado);

        // Assert
        Assert.AreEqual(salaEditado.Numero, sala.Numero);
        Assert.AreEqual(salaEditado.Capacidade, sala.Capacidade);
    }
}
