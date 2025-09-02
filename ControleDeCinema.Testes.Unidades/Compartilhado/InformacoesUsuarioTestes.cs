using ControleDeCinema.Dominio.ModuloAutenticacao;

namespace ControleDeCinema.Testes.Unidades.Compartilhado;

[TestClass]
[TestCategory("Testes de Unidade de Informações de Usuário (Domínio)")]
public class InformacoesUsuarioTestes
{
    [TestMethod]
    public void Deve_Criar_InformacoesUsuario_Com_Valores_Validos()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        const string email = "usuario@teste.com";

        // Act
        InformacoesUsuario usuario = new()
        {
            Id = id,
            Email = email
        };

        // Assert
        Assert.AreEqual(id, usuario.Id);
        Assert.AreEqual(email, usuario.Email);
    }
}
