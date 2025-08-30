using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Dominio.ModuloSessao;
using ControleDeCinema.Testes.Integracao.Compartilhado;
using FizzWare.NBuilder;

namespace ControleDeCinema.Testes.Integracao.Repositorios;

[TestClass]
[TestCategory("Testes de Integração de RepositorioIngressoEmOrm")]
public class IngressoRepositorioTestes : TestFixture
{
    private GeneroFilme generoPadrao = null!;
    private Filme filmePadrao = null!;
    private Sala salaPadrao = null!;

    [TestInitialize]
    public override void ConfigurarTestes()
    {
        base.ConfigurarTestes();

        generoPadrao = Builder<GeneroFilme>.CreateNew().Persist();

        filmePadrao = Builder<Filme>.CreateNew()
            .With(f => f.Genero = generoPadrao).Persist();

        salaPadrao = Builder<Sala>.CreateNew().Persist();
    }

    [TestMethod]
    public void Deve_Selecionar_Ingressos_Do_Usuario_Corretamente()
    {
        // Arrange
        Guid usuarioAId = Guid.NewGuid();
        Guid usuarioBId = Guid.NewGuid();

        List<Sessao> sessoesUsuarioA = new()
        {
            new(new DateTime(2002, 8, 9, 0, 0, 0, DateTimeKind.Utc), 12, filmePadrao, salaPadrao)
            { UsuarioId = usuarioAId },
            new(new DateTime(2023, 4, 2, 0, 0, 0, DateTimeKind.Utc), 15, filmePadrao, salaPadrao)
            { UsuarioId = usuarioAId },
            new(new DateTime(2012, 1, 6, 0, 0, 0, DateTimeKind.Utc), 21, filmePadrao, salaPadrao)
            { UsuarioId = usuarioAId }
        };

        List<Sessao> sessoesUsuarioB = new()
        {
            new(new DateTime(2002, 8, 9, 0, 0, 0, DateTimeKind.Utc), 12, filmePadrao, salaPadrao)
            { UsuarioId = usuarioBId },
            new(new DateTime(2023, 4, 2, 0, 0, 0, DateTimeKind.Utc), 15, filmePadrao, salaPadrao)
            { UsuarioId = usuarioBId },
            new(new DateTime(2012, 1, 6, 0, 0, 0, DateTimeKind.Utc), 21, filmePadrao, salaPadrao)
            { UsuarioId = usuarioBId }
        };
        List<Ingresso> ingressosUsuarioA = new();

        List<Ingresso> ingressosUsuarioB = new();

        foreach (Sessao s in sessoesUsuarioA)
        {
            Ingresso novoIngresso = s.GerarIngresso(1, true);
            novoIngresso.UsuarioId = usuarioAId;
            ingressosUsuarioA.Add(novoIngresso);
        }

        foreach (Sessao s in sessoesUsuarioB)
        {
            Ingresso novoIngresso = s.GerarIngresso(1, false);
            novoIngresso.UsuarioId = usuarioBId;
            ingressosUsuarioB.Add(novoIngresso);
        }

        RepositorioSessaoEmOrm.CadastrarEntidades(sessoesUsuarioA);
        RepositorioSessaoEmOrm.CadastrarEntidades(sessoesUsuarioB);

        dbContext.SaveChanges();

        // Act
        List<Ingresso> ingressosSelecionados = RepositorioIngressoEmOrm.SelecionarRegistros(usuarioAId);
        List<Ingresso> ingressosEsperados = sessoesUsuarioA.SelectMany(s => s.Ingressos).ToList();

        // Assert
        Assert.AreEqual(ingressosEsperados.Count, ingressosSelecionados.Count);
        CollectionAssert.AreEquivalent(ingressosEsperados, ingressosSelecionados);
        foreach (Ingresso ingresso in ingressosSelecionados)
        {
            Assert.IsNotNull(ingresso.Sessao);
            Assert.IsNotNull(ingresso.Sessao.Filme);
            Assert.IsNotNull(ingresso.Sessao.Sala);
        }
    }
}
