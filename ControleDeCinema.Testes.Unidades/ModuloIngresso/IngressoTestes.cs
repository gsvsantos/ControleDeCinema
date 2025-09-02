using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Dominio.ModuloSessao;
using FizzWare.NBuilder;

namespace ControleDeCinema.Testes.Unidades.ModuloIngresso;

[TestClass]
[TestCategory("Testes de Unidade de Filme (Domínio)")]
public class IngressoTestes
{
    private Ingresso? ingresso;

    private static readonly GeneroFilme generoPadrao = Builder<GeneroFilme>.CreateNew()
        .WithFactory(() => new("Comédia")).Build();

    private static readonly Filme filmePadrao = Builder<Filme>.CreateNew()
        .WithFactory(() => new("Gente Grande 2", 101, false, generoPadrao)).Build();

    private static readonly Sala salaPadrao = Builder<Sala>.CreateNew()
        .WithFactory(() => new(1, 30)).Build();

    private static readonly DateTime dataPadrao = new(2012, 12, 12, 0, 0, 0, DateTimeKind.Utc);

    private static readonly Sessao sessaoPadrao = Builder<Sessao>.CreateNew()
        .WithFactory(() => new(dataPadrao, 30, filmePadrao, salaPadrao)).Build();

    [TestMethod]
    public void Deve_Atualizar_Registro_Ingresso_Corretamente()
    {
        // Arrange
        ingresso = new(2, true, sessaoPadrao);

        Ingresso ingressoEditado = new(12, false, sessaoPadrao);

        // Act
        ingresso.AtualizarRegistro(ingressoEditado);

        // Assert
        Assert.AreEqual(ingressoEditado.NumeroAssento, ingresso.NumeroAssento);
        Assert.AreEqual(ingressoEditado.MeiaEntrada, ingresso.MeiaEntrada);
        Assert.AreEqual(ingressoEditado.Sessao, ingresso.Sessao);
    }
}
