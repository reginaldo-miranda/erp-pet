using ERP.Domain.Core.Entities.Pet;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Data.Configurations.Pet;

public class TabelaPrecoBanhoConfiguration : IEntityTypeConfiguration<TabelaPrecoBanho>
{
    public void Configure(EntityTypeBuilder<TabelaPrecoBanho> builder)
    {
        builder.ToTable("TabelasPrecoBanho");
        builder.Property(t => t.NomeServico).HasMaxLength(150).IsRequired();
        builder.Property(t => t.Valor).HasPrecision(18, 2);
        builder.Property(t => t.ComissaoBanhoPercentual).HasPrecision(5, 2);
        builder.Property(t => t.ComissaoTosaPercentual).HasPrecision(5, 2);
        builder.HasIndex(t => new { t.EmpresaId, t.Porte, t.TipoPelagem });
    }
}

public class RecursoBanhoTosaConfiguration : IEntityTypeConfiguration<RecursoBanhoTosa>
{
    public void Configure(EntityTypeBuilder<RecursoBanhoTosa> builder)
    {
        builder.ToTable("RecursosBanhoTosa");
        builder.Property(r => r.Nome).HasMaxLength(100).IsRequired();
    }
}

public class PacoteBanhoPetConfiguration : IEntityTypeConfiguration<PacoteBanhoPet>
{
    public void Configure(EntityTypeBuilder<PacoteBanhoPet> builder)
    {
        builder.ToTable("PacotesBanhoPet");
        builder.Property(p => p.NomePacote).HasMaxLength(150).IsRequired();
        builder.Property(p => p.ValorTotal).HasPrecision(18, 2);

        builder.HasOne(p => p.Cliente).WithMany().HasForeignKey(p => p.ClienteId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(p => p.Pet).WithMany().HasForeignKey(p => p.PetId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class AtendimentoBanhoTosaConfiguration : IEntityTypeConfiguration<AtendimentoBanhoTosa>
{
    public void Configure(EntityTypeBuilder<AtendimentoBanhoTosa> builder)
    {
        builder.ToTable("AtendimentosBanhoTosa");
        builder.Property(a => a.Numero).HasMaxLength(40).IsRequired();
        builder.HasIndex(a => new { a.EmpresaId, a.Numero }).IsUnique();
        builder.Property(a => a.ValorServico).HasPrecision(18, 2);
        builder.Property(a => a.ValorAdicionais).HasPrecision(18, 2);
        builder.Property(a => a.ValorTotal).HasPrecision(18, 2);
        builder.Property(a => a.BanhistaNome).HasMaxLength(100);
        builder.Property(a => a.TosadorNome).HasMaxLength(100);

        builder.HasOne(a => a.Pet).WithMany().HasForeignKey(a => a.PetId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(a => a.Cliente).WithMany().HasForeignKey(a => a.ClienteId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(a => a.TabelaPrecoBanho).WithMany().HasForeignKey(a => a.TabelaPrecoBanhoId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(a => a.PacoteBanhoPet).WithMany().HasForeignKey(a => a.PacoteBanhoPetId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(a => a.RecursoBanhoTosa).WithMany().HasForeignKey(a => a.RecursoBanhoTosaId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(a => a.ComandaAtendimento).WithMany().HasForeignKey(a => a.ComandaAtendimentoId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class ComissaoBanhoTosaConfiguration : IEntityTypeConfiguration<ComissaoBanhoTosa>
{
    public void Configure(EntityTypeBuilder<ComissaoBanhoTosa> builder)
    {
        builder.ToTable("ComissoesBanhoTosa");
        builder.Property(c => c.ProfissionalNome).HasMaxLength(100).IsRequired();
        builder.Property(c => c.BaseCalculo).HasPrecision(18, 2);
        builder.Property(c => c.Percentual).HasPrecision(5, 2);
        builder.Property(c => c.ValorComissao).HasPrecision(18, 2);

        builder.HasOne(c => c.AtendimentoBanhoTosa)
            .WithMany(a => a.Comissoes)
            .HasForeignKey(c => c.AtendimentoBanhoTosaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
