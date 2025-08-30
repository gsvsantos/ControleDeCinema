using ControleDeCinema.Dominio.ModuloSala;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleDeCinema.Infraestrutura.Orm.ModuloSala;

public class MapeadorSalaEmOrm : IEntityTypeConfiguration<Sala>
{
    public void Configure(EntityTypeBuilder<Sala> builder)
    {
        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.Numero)
            .IsRequired();

        builder.Property(x => x.Capacidade)
            .IsRequired();

        builder.HasIndex(x => x.Id)
            .IsUnique();
    }
}
