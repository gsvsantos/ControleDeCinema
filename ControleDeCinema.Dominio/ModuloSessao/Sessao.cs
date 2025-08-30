using ControledeCinema.Dominio.Compartilhado;
using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloSala;
using System.Diagnostics.CodeAnalysis;

namespace ControleDeCinema.Dominio.ModuloSessao;

public class Sessao : EntidadeBase<Sessao>
{
    public DateTime Inicio { get; set; }
    public int NumeroMaximoIngressos { get; set; }
    public bool Encerrada { get; private set; }

    public Filme Filme { get; set; }
    public Sala Sala { get; set; }

    public List<Ingresso> Ingressos { get; set; }

    [ExcludeFromCodeCoverage]
    protected Sessao()
    {
        Ingressos = new List<Ingresso>();
    }

    public Sessao(DateTime inicio, int numeroMaximoIngressos, Filme filme, Sala sala) : this()
    {
        Id = Guid.NewGuid();
        Inicio = inicio;
        NumeroMaximoIngressos = numeroMaximoIngressos;
        Filme = filme;
        Sala = sala;
        Encerrada = false;
    }

    public Ingresso GerarIngresso(int assentoSelecionado, bool meiaEntrada)
    {
        var ingresso = new Ingresso(assentoSelecionado, meiaEntrada, this);

        Ingressos.Add(ingresso);

        return ingresso;
    }

    public int[] ObterAssentosDisponiveis()
    {
        var assentosDisponiveis = Enumerable.Range(1, NumeroMaximoIngressos);

        var assentosOcupados = Ingressos.Select(i => i.NumeroAssento).ToArray();

        return assentosDisponiveis.Except(assentosOcupados).ToArray();
    }

    public int ObterQuantidadeIngressosDisponiveis()
    {
        return NumeroMaximoIngressos - Ingressos.Count;
    }

    public void Encerrar()
    {
        Encerrada = true;
    }

    public override void AtualizarRegistro(Sessao registroEditado)
    {
        Inicio = registroEditado.Inicio;
        NumeroMaximoIngressos = registroEditado.NumeroMaximoIngressos;
        Filme = registroEditado.Filme;
        Sala = registroEditado.Sala;
    }
}