using ERP.Domain.Core.Entities.Pet;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Data.Configurations.Pet;

public class ConfiguracaoWhatsAppEmpresaConfiguration : IEntityTypeConfiguration<ConfiguracaoWhatsAppEmpresa>
{
    public void Configure(EntityTypeBuilder<ConfiguracaoWhatsAppEmpresa> builder)
    {
        builder.ToTable("ConfiguracoesWhatsAppEmpresa");
        builder.Property(c => c.Provedor).HasMaxLength(50).IsRequired();
        builder.HasIndex(c => c.EmpresaId).IsUnique();
    }
}

public class NotificacaoWhatsAppOutboxConfiguration : IEntityTypeConfiguration<NotificacaoWhatsAppOutbox>
{
    public void Configure(EntityTypeBuilder<NotificacaoWhatsAppOutbox> builder)
    {
        builder.ToTable("NotificacoesWhatsAppOutbox");
        builder.Property(n => n.TelefoneDestino).HasMaxLength(30).IsRequired();
        builder.HasIndex(n => new { n.EmpresaId, n.Status });
    }
}

public class AcessoPortalTutorConfiguration : IEntityTypeConfiguration<AcessoPortalTutor>
{
    public void Configure(EntityTypeBuilder<AcessoPortalTutor> builder)
    {
        builder.ToTable("AcessosPortalTutor");
        builder.Property(a => a.TokenAcesso).HasMaxLength(40).IsRequired();
        builder.HasIndex(a => a.TokenAcesso).IsUnique();
        builder.HasOne(a => a.Cliente).WithMany().HasForeignKey(a => a.ClienteId).OnDelete(DeleteBehavior.Cascade);
    }
}
