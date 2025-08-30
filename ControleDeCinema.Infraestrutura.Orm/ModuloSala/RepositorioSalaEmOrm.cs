using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Infraestrutura.Orm.Compartilhado;

namespace ControleDeCinema.Infraestrutura.Orm.ModuloSala;

public class RepositorioSalaEmOrm : RepositorioBaseEmOrm<Sala>, IRepositorioSala
{
    public RepositorioSalaEmOrm(ControleDeCinemaDbContext contexto) : base(contexto) { }
}
