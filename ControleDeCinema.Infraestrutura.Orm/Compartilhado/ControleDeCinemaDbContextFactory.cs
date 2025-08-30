using Microsoft.EntityFrameworkCore;

namespace ControleDeCinema.Infraestrutura.Orm.Compartilhado;

public static class ControleDeCinemaDbContextFactory
{
    public static ControleDeCinemaDbContext CriarDbContext(string connectionString)
    {
        var options = new DbContextOptionsBuilder<ControleDeCinemaDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        var dbContext = new ControleDeCinemaDbContext(options);

        return dbContext;
    }
}
