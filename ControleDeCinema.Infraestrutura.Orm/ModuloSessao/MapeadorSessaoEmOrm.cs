using ControleDeCinema.Dominio.ModuloSessao;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleDeCinema.Infraestrutura.Orm.ModuloSessao;

public class MapeadorSessaoEmOrm : IEntityTypeConfiguration<Sessao>
{
    public void Configure(EntityTypeBuilder<Sessao> builder)
    {
        builder.Property(x => x.Id)
               .ValueGeneratedNever()
               .IsRequired();

        builder.Property(x => x.Inicio)
               .IsRequired();

        builder.Property(x => x.NumeroMaximoIngressos)
               .IsRequired();

        builder.Property(x => x.Encerrada)
               .IsRequired();

        builder.HasOne(x => x.Filme)
               .WithMany(f => f.Sessoes)
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Sala)
               .WithMany()
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(x => x.Ingressos)
               .WithOne(i => i.Sessao)
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => x.Id)
            .IsUnique();

        builder.HasIndex(x => new { x.Id, x.Inicio });
    }
}
