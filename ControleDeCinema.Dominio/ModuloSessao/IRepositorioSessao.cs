using ControledeCinema.Dominio.Compartilhado;

namespace ControleDeCinema.Dominio.ModuloSessao;

public interface IRepositorioSessao : IRepositorio<Sessao>
{
    List<Sessao> SelecionarRegistrosDoUsuario(Guid guid);
}
