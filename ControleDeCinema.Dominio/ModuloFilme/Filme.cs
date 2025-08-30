using ControledeCinema.Dominio.Compartilhado;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSessao;

namespace ControleDeCinema.Dominio.ModuloFilme;

public class Filme : EntidadeBase<Filme>
{
    public string Titulo { get; set; }
    public int Duracao { get; set; }
    public bool Lancamento { get; set; }
    public GeneroFilme Genero { get; set; }
    public List<Sessao> Sessoes { get; set; }

    protected Filme()
    {
        Sessoes = new List<Sessao>();
    }

    public Filme(string titulo, int duracao, bool lancamento, GeneroFilme genero) : this()
    {
        Id = Guid.NewGuid();
        Titulo = titulo;
        Duracao = duracao;
        Lancamento = lancamento;
        Genero = genero;
    }

    public override void AtualizarRegistro(Filme registroEditado)
    {
        Titulo = registroEditado.Titulo;
        Duracao = registroEditado.Duracao;
        Lancamento = registroEditado.Lancamento;
        Genero = registroEditado.Genero;
    }
}
