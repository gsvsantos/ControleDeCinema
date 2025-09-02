using ControledeCinema.Dominio.Compartilhado;
using System.Diagnostics.CodeAnalysis;

namespace ControleDeCinema.Dominio.ModuloSala;

public class Sala : EntidadeBase<Sala>
{
    public int Numero { get; set; }
    public int Capacidade { get; set; }

    [ExcludeFromCodeCoverage]
    protected Sala() { }

    public Sala(int numero, int capacidade)
    {
        Id = Guid.NewGuid();
        Numero = numero;
        Capacidade = capacidade;
    }

    public override void AtualizarRegistro(Sala registroEditado)
    {
        Numero = registroEditado.Numero;
        Capacidade = registroEditado.Capacidade;
    }
}
