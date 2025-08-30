using ControleDeCinema.Dominio.ModuloGeneroFilme;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleDeCinema.Infraestrutura.Orm.ModuloGeneroFilme;

public class MapeadorGeneroFilmeEmOrm : IEntityTypeConfiguration<GeneroFilme>
{
    public void Configure(EntityTypeBuilder<GeneroFilme> builder)
    {
        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.Descricao)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.Id)
            .IsUnique();
    }
}
