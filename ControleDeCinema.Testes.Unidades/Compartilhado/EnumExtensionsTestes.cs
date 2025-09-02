using ControledeCinema.Dominio.Extensions;
using System.ComponentModel.DataAnnotations;

namespace ControleDeCinema.Testes.Unidades.Compartilhado;

[TestClass]
[TestCategory("Testes de Unidade de EnumExtensions (Domínio)")]
public class EnumExtensionsTestes
{
    private enum EnumGeneroFilme
    {
        [Display(Name = "Comédia")]
        Comedia,
        [Display(Name = "Românce")]
        Romance,
        Terror
    }

    [TestMethod]
    public void Deve_Pegar_Nome_De_Display_Do_Enum_Corretamente()
    {
        // Arrange
        EnumGeneroFilme generoComedia = EnumGeneroFilme.Comedia;
        EnumGeneroFilme generoRomance = EnumGeneroFilme.Romance;

        // Act
        string? displayComedia = generoComedia.GetDisplayName();
        string? displayRomance = generoRomance.GetDisplayName();

        // Assert
        Assert.AreEqual("Comédia", displayComedia);
        Assert.AreEqual("Românce", displayRomance);
    }

    [TestMethod]
    public void Deve_Pegar_Nome_Do_Enum_Sem_Display_Corretamente()
    {
        // Arrange
        EnumGeneroFilme generoTerror = EnumGeneroFilme.Terror;

        // Act
        string? displayTerror = generoTerror.GetDisplayName();
        string? stringTerror = generoTerror.ToString();

        // Assert
        Assert.IsNull(displayTerror);
        Assert.AreNotEqual("Terror", displayTerror);
        Assert.AreEqual("Terror", stringTerror);
    }
}
