using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Infraestrutura.Orm.Compartilhado;

namespace ControleDeCinema.Infraestrutura.Orm.ModuloGeneroFilme;

public class RepositorioGeneroFilmeEmOrm : RepositorioBaseEmOrm<GeneroFilme>, IRepositorioGeneroFilme
{
    public RepositorioGeneroFilmeEmOrm(ControleDeCinemaDbContext contexto) : base(contexto) { }
}
