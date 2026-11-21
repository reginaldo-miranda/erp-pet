using ERP.Domain.Core.Entities.Pet;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Data.Configurations.Pet;

public class BaiaInternacaoConfiguration : IEntityTypeConfiguration<BaiaInternacao>
{
    public void Configure(EntityTypeBuilder<BaiaInternacao> builder)
    {
        builder.ToTable("BaiasInternacao");
        builder.Property(b => b.Codigo).HasMaxLength(30).IsRequired();
        builder.Property(b => b.Nome).HasMaxLength(100).IsRequired();
        builder.Property(b => b.ValorDiaria).HasPrecision(18, 2);
        builder.HasIndex(b => new { b.EmpresaId, b.Codigo }).IsUnique();
    }
}

public class InternacaoPetConfiguration : IEntityTypeConfiguration<InternacaoPet>
{
    public void Configure(EntityTypeBuilder<InternacaoPet> builder)
    {
        builder.ToTable("InternacoesPet");
        builder.Property(i => i.Numero).HasMaxLength(40).IsRequired();
        builder.HasIndex(i => new { i.EmpresaId, i.Numero }).IsUnique();
        builder.Property(i => i.ValorDiaria).HasPrecision(18, 2);

        builder.HasOne(i => i.Pet).WithMany().HasForeignKey(i => i.PetId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(i => i.Cliente).WithMany().HasForeignKey(i => i.ClienteId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(i => i.Veterinario).WithMany().HasForeignKey(i => i.VeterinarioId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(i => i.BaiaInternacao).WithMany().HasForeignKey(i => i.BaiaInternacaoId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(i => i.ComandaAtendimento).WithMany().HasForeignKey(i => i.ComandaAtendimentoId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class PrescricaoInternacaoConfiguration : IEntityTypeConfiguration<PrescricaoInternacao>
{
    public void Configure(EntityTypeBuilder<PrescricaoInternacao> builder)
    {
        builder.ToTable("PrescricoesInternacao");
        builder.Property(p => p.DescricaoMedicamento).HasMaxLength(200).IsRequired();
        builder.Property(p => p.QuantidadePorAplicacao).HasPrecision(18, 4);
        builder.Property(p => p.ValorUnitarioCobranca).HasPrecision(18, 2);

        builder.HasOne(p => p.InternacaoPet).WithMany(i => i.Prescricoes).HasForeignKey(p => p.InternacaoPetId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class ChecagemEnfermagemConfiguration : IEntityTypeConfiguration<ChecagemEnfermagem>
{
    public void Configure(EntityTypeBuilder<ChecagemEnfermagem> builder)
    {
        builder.ToTable("ChecagensEnfermagem");
        builder.HasOne(c => c.PrescricaoInternacao).WithMany(p => p.Checagens).HasForeignKey(c => c.PrescricaoInternacaoId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class EvolucaoInternacaoConfiguration : IEntityTypeConfiguration<EvolucaoInternacao>
{
    public void Configure(EntityTypeBuilder<EvolucaoInternacao> builder)
    {
        builder.ToTable("EvolucoesInternacao");
        builder.Property(e => e.TemperaturaC).HasPrecision(4, 1);
        builder.HasOne(e => e.InternacaoPet).WithMany(i => i.Evolucoes).HasForeignKey(e => e.InternacaoPetId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class CirurgiaVeterinariaConfiguration : IEntityTypeConfiguration<CirurgiaVeterinaria>
{
    public void Configure(EntityTypeBuilder<CirurgiaVeterinaria> builder)
    {
        builder.ToTable("CirurgiasVeterinarias");
        builder.Property(c => c.Numero).HasMaxLength(40).IsRequired();
        builder.HasIndex(c => new { c.EmpresaId, c.Numero }).IsUnique();
        builder.Property(c => c.ValorProcedimento).HasPrecision(18, 2);

        builder.HasOne(c => c.Pet).WithMany().HasForeignKey(c => c.PetId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(c => c.Cliente).WithMany().HasForeignKey(c => c.ClienteId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(c => c.VeterinarioCirurgiao).WithMany().HasForeignKey(c => c.VeterinarioCirurgiaoId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class TermoConsentimentoPetConfiguration : IEntityTypeConfiguration<TermoConsentimentoPet>
{
    public void Configure(EntityTypeBuilder<TermoConsentimentoPet> builder)
    {
        builder.ToTable("TermosConsentimentoPet");
        builder.Property(t => t.Titulo).HasMaxLength(200).IsRequired();
        builder.HasOne(t => t.Pet).WithMany().HasForeignKey(t => t.PetId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.Cliente).WithMany().HasForeignKey(t => t.ClienteId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class ExameVeterinarioConfiguration : IEntityTypeConfiguration<ExameVeterinario>
{
    public void Configure(EntityTypeBuilder<ExameVeterinario> builder)
    {
        builder.ToTable("ExamesVeterinarios");
        builder.Property(e => e.NomeExame).HasMaxLength(150).IsRequired();
        builder.Property(e => e.CodigoAmostraLis).HasMaxLength(60).IsRequired();
        builder.HasIndex(e => new { e.EmpresaId, e.CodigoAmostraLis });
        builder.HasOne(e => e.Pet).WithMany().HasForeignKey(e => e.PetId).OnDelete(DeleteBehavior.Restrict);
    }
}
