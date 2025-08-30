using ControleDeCinema.Dominio.ModuloFilme;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleDeCinema.Infraestrutura.Orm.ModuloFilme;

public class MapeadorFilmeEmOrm : IEntityTypeConfiguration<Filme>
{
    public void Configure(EntityTypeBuilder<Filme> builder)
    {
        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.Titulo)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Duracao)
            .IsRequired();

        builder.Property(x => x.Lancamento)
            .IsRequired();

        builder.HasOne(f => f.Genero)
            .WithMany(g => g.Filmes)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => x.Id)
            .IsUnique();
    }
}
