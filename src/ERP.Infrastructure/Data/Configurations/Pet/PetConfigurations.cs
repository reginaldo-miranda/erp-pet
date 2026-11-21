using ERP.Domain.Core.Entities;
using ERP.Domain.Core.Entities.Cadastros;
using ERP.Domain.Core.Entities.Pet;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Data.Configurations.Pet;

public class ModuloSistemaConfiguration : IEntityTypeConfiguration<ModuloSistema>
{
    public void Configure(EntityTypeBuilder<ModuloSistema> builder)
    {
        builder.ToTable("ModulosSistema");
        builder.HasIndex(m => m.Codigo).IsUnique();
        builder.Property(m => m.Codigo).HasMaxLength(60).IsRequired();
        builder.Property(m => m.Nome).HasMaxLength(120).IsRequired();
        builder.Property(m => m.Categoria).HasMaxLength(50).IsRequired();
    }
}

public class EmpresaModuloConfiguration : IEntityTypeConfiguration<EmpresaModulo>
{
    public void Configure(EntityTypeBuilder<EmpresaModulo> builder)
    {
        builder.ToTable("EmpresaModulos");
        builder.HasIndex(em => new { em.EmpresaId, em.ModuloCodigo }).IsUnique();
        builder.Property(em => em.ModuloCodigo).HasMaxLength(60).IsRequired();
    }
}

public class ConvenioPetConfiguration : IEntityTypeConfiguration<ConvenioPet>
{
    public void Configure(EntityTypeBuilder<ConvenioPet> builder)
    {
        builder.ToTable("ConveniosPet");
        builder.Property(c => c.Nome).HasMaxLength(150).IsRequired();
        builder.Property(c => c.Cnpj).HasMaxLength(20);
        builder.Property(c => c.PercentualCoberturaPadrao).HasPrecision(5, 2);
    }
}

public class PetConfiguration : IEntityTypeConfiguration<Domain.Core.Entities.Pet.Pet>
{
    public void Configure(EntityTypeBuilder<Domain.Core.Entities.Pet.Pet> builder)
    {
        builder.ToTable("Pets");
        builder.Property(p => p.Nome).HasMaxLength(120).IsRequired();
        builder.Property(p => p.Raca).HasMaxLength(100);
        builder.Property(p => p.CorPelagem).HasMaxLength(80);
        builder.Property(p => p.Microchip).HasMaxLength(60);
        builder.Property(p => p.PesoAtualKg).HasPrecision(8, 3);
        builder.HasIndex(p => new { p.EmpresaId, p.Nome });

        builder.HasOne(p => p.ConvenioPet)
            .WithMany()
            .HasForeignKey(p => p.ConvenioPetId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public class TutorPetConfiguration : IEntityTypeConfiguration<TutorPet>
{
    public void Configure(EntityTypeBuilder<TutorPet> builder)
    {
        builder.ToTable("TutorPets");
        builder.HasIndex(tp => new { tp.PetId, tp.ClienteId }).IsUnique();

        builder.HasOne(tp => tp.Pet)
            .WithMany(p => p.Tutores)
            .HasForeignKey(tp => tp.PetId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(tp => tp.Cliente)
            .WithMany(c => c.PetsVinculados)
            .HasForeignKey(tp => tp.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ComandaAtendimentoConfiguration : IEntityTypeConfiguration<ComandaAtendimento>
{
    public void Configure(EntityTypeBuilder<ComandaAtendimento> builder)
    {
        builder.ToTable("ComandasAtendimento");
        builder.Property(c => c.Numero).HasMaxLength(40).IsRequired();
        builder.HasIndex(c => new { c.EmpresaId, c.Numero }).IsUnique();
        builder.Property(c => c.ValorBruto).HasPrecision(18, 2);
        builder.Property(c => c.ValorDesconto).HasPrecision(18, 2);
        builder.Property(c => c.ValorCobertoConvenio).HasPrecision(18, 2);
        builder.Property(c => c.ValorLiquido).HasPrecision(18, 2);

        builder.HasOne(c => c.Cliente)
            .WithMany()
            .HasForeignKey(c => c.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Pet)
            .WithMany()
            .HasForeignKey(c => c.PetId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(c => c.ConvenioPet)
            .WithMany()
            .HasForeignKey(c => c.ConvenioPetId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public class ComandaItemConfiguration : IEntityTypeConfiguration<ComandaItem>
{
    public void Configure(EntityTypeBuilder<ComandaItem> builder)
    {
        builder.ToTable("ComandaItens");
        builder.Property(i => i.Descricao).HasMaxLength(200).IsRequired();
        builder.Property(i => i.Quantidade).HasPrecision(18, 4);
        builder.Property(i => i.ValorUnitario).HasPrecision(18, 4);
        builder.Property(i => i.ValorDesconto).HasPrecision(18, 2);
        builder.Property(i => i.ValorCobertoConvenio).HasPrecision(18, 2);
        builder.Property(i => i.ValorTotal).HasPrecision(18, 2);

        builder.HasOne(i => i.ComandaAtendimento)
            .WithMany(c => c.Itens)
            .HasForeignKey(i => i.ComandaAtendimentoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class AberturaSacoGranelConfiguration : IEntityTypeConfiguration<AberturaSacoGranel>
{
    public void Configure(EntityTypeBuilder<AberturaSacoGranel> builder)
    {
        builder.ToTable("AberturasSacoGranel");
        builder.Property(a => a.QuantidadeSacos).HasPrecision(18, 4);
        builder.Property(a => a.PesoTotalGeradoKg).HasPrecision(18, 4);
        builder.Property(a => a.CustoUnitarioGranel).HasPrecision(18, 4);

        builder.HasOne(a => a.ProdutoSaco)
            .WithMany()
            .HasForeignKey(a => a.ProdutoSacoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.ProdutoGranel)
            .WithMany()
            .HasForeignKey(a => a.ProdutoGranelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Deposito)
            .WithMany()
            .HasForeignKey(a => a.DepositoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
