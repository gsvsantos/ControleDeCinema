using ControleDeCinema.Dominio.ModuloSessao;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleDeCinema.Infraestrutura.Orm.ModuloSessao;

public class MapeadorIngressoEmOrm : IEntityTypeConfiguration<Ingresso>
{
    public void Configure(EntityTypeBuilder<Ingresso> builder)
    {
        builder.Property(x => x.Id)
               .ValueGeneratedNever()
               .IsRequired();

        builder.Property(x => x.NumeroAssento)
               .IsRequired();

        builder.Property(x => x.MeiaEntrada)
               .IsRequired();

        builder.HasIndex(x => x.Id)
            .IsUnique();
    }
}
