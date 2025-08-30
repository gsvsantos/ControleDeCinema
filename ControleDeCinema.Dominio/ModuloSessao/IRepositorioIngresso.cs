namespace ControleDeCinema.Dominio.ModuloSessao;

public interface IRepositorioIngresso
{
    List<Ingresso> SelecionarRegistros(Guid usuarioId);
}
