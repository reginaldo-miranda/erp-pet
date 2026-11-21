using ERP.Domain.Core.Entities.Pet;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Data.Configurations.Pet;

public class VeterinarioConfiguration : IEntityTypeConfiguration<Veterinario>
{
    public void Configure(EntityTypeBuilder<Veterinario> builder)
    {
        builder.ToTable("Veterinarios");
        builder.Property(v => v.Nome).HasMaxLength(150).IsRequired();
        builder.Property(v => v.Crmv).HasMaxLength(30).IsRequired();
        builder.Property(v => v.UfCrmv).HasMaxLength(5).IsRequired();
        builder.Property(v => v.Especialidade).HasMaxLength(100);
        builder.Property(v => v.ValorConsultaPadrao).HasPrecision(18, 2);
    }
}

public class ConsultaVeterinariaConfiguration : IEntityTypeConfiguration<ConsultaVeterinaria>
{
    public void Configure(EntityTypeBuilder<ConsultaVeterinaria> builder)
    {
        builder.ToTable("ConsultasVeterinarias");
        builder.Property(c => c.Numero).HasMaxLength(40).IsRequired();
        builder.HasIndex(c => new { c.EmpresaId, c.Numero }).IsUnique();
        builder.Property(c => c.PesoKg).HasPrecision(8, 3);
        builder.Property(c => c.TemperaturaC).HasPrecision(4, 1);
        builder.Property(c => c.ValorConsulta).HasPrecision(18, 2);

        builder.HasOne(c => c.Pet).WithMany().HasForeignKey(c => c.PetId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(c => c.Cliente).WithMany().HasForeignKey(c => c.ClienteId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(c => c.Veterinario).WithMany().HasForeignKey(c => c.VeterinarioId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(c => c.ComandaAtendimento).WithMany().HasForeignKey(c => c.ComandaAtendimentoId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class ReceitaVeterinariaConfiguration : IEntityTypeConfiguration<ReceitaVeterinaria>
{
    public void Configure(EntityTypeBuilder<ReceitaVeterinaria> builder)
    {
        builder.ToTable("ReceitasVeterinarias");
        builder.HasOne(r => r.ConsultaVeterinaria).WithMany().HasForeignKey(r => r.ConsultaVeterinariaId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(r => r.Pet).WithMany().HasForeignKey(r => r.PetId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(r => r.Veterinario).WithMany().HasForeignKey(r => r.VeterinarioId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class ReceitaVeterinariaItemConfiguration : IEntityTypeConfiguration<ReceitaVeterinariaItem>
{
    public void Configure(EntityTypeBuilder<ReceitaVeterinariaItem> builder)
    {
        builder.ToTable("ReceitaVeterinariaItens");
        builder.Property(i => i.Medicamento).HasMaxLength(150).IsRequired();
        builder.Property(i => i.Posologia).HasMaxLength(300).IsRequired();

        builder.HasOne(i => i.ReceitaVeterinaria)
            .WithMany(r => r.Itens)
            .HasForeignKey(i => i.ReceitaVeterinariaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class VacinacaoVermifugoPetConfiguration : IEntityTypeConfiguration<VacinacaoVermifugoPet>
{
    public void Configure(EntityTypeBuilder<VacinacaoVermifugoPet> builder)
    {
        builder.ToTable("VacinacoesVermifugosPet");
        builder.Property(v => v.NomeProduto).HasMaxLength(150).IsRequired();
        builder.Property(v => v.NumeroLote).HasMaxLength(60);

        builder.HasOne(v => v.Pet).WithMany().HasForeignKey(v => v.PetId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(v => v.Veterinario).WithMany().HasForeignKey(v => v.VeterinarioId).OnDelete(DeleteBehavior.SetNull);
    }
}
