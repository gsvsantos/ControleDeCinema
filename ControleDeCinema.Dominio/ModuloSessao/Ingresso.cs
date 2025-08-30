using ControledeCinema.Dominio.Compartilhado;
using System.Diagnostics.CodeAnalysis;


namespace ControleDeCinema.Dominio.ModuloSessao;

public class Ingresso : EntidadeBase<Ingresso>
{
    public bool MeiaEntrada { get; set; }
    public int NumeroAssento { get; set; }
    public Sessao Sessao { get; set; }

    [ExcludeFromCodeCoverage]
    protected Ingresso() { }

    public Ingresso(int numeroAssento, bool meiaEntrada, Sessao sessao)
    {
        Id = Guid.NewGuid();
        NumeroAssento = numeroAssento;
        MeiaEntrada = meiaEntrada;
        Sessao = sessao;
    }

    public override void AtualizarRegistro(Ingresso registroEditado)
    {
        NumeroAssento = registroEditado.NumeroAssento;
        MeiaEntrada = registroEditado.MeiaEntrada;
    }
}
