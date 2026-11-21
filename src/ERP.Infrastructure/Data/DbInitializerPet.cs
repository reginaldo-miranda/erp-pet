using ERP.Application.Common.Interfaces;
using ERP.Domain.Core.Entities;
using ERP.Domain.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Data;

public static class DbInitializerPet
{
    public static async Task SeedPetAsync(ApplicationDbContext context)
    {
        var modulos = new List<ModuloSistema>
        {
            new() { Codigo = ModulosPet.LojaGranel, Nome = "Loja Pet & Granel", Descricao = "Fracionamento de sacos para venda a granel e leitura de balança no PDV", Categoria = "Loja", Ordem = 1, Ativo = true },
            new() { Codigo = ModulosPet.BanhoTosa, Nome = "Banho & Tosa", Descricao = "Agenda visual, fila em tempo real, check-in/fotos, pacotes e comissão por etapa", Categoria = "BanhoTosa", Ordem = 2, Ativo = true },
            new() { Codigo = ModulosPet.VetAmbulatorio, Nome = "Hospital — Ambulatório & Vacinas", Descricao = "Agenda médica, triagem, prontuário SOAP, receituário e carteira de vacinas", Categoria = "Hospital", Ordem = 3, Ativo = true },
            new() { Codigo = ModulosPet.VetInternacao, Nome = "Hospital — Internação 24h", Descricao = "Mapa de baias, aprazamento de enfermagem, evolução e baixa automática de insumos", Categoria = "Hospital", Ordem = 4, Ativo = true },
            new() { Codigo = ModulosPet.VetCirurgico, Nome = "Hospital — Centro Cirúrgico & Exames", Descricao = "Agenda cirúrgica, ficha anestésica, termos assináveis e laudos de exames", Categoria = "Hospital", Ordem = 5, Ativo = true },
            new() { Codigo = ModulosPet.VetLabHl7, Nome = "Integração Laboratório (HL7/LIS)", Descricao = "Recepção automática de resultados de equipamentos e laboratórios externos", Categoria = "Hospital", Ordem = 6, Ativo = true },
            new() { Codigo = ModulosPet.ConveniosPet, Nome = "Convênios / Planos de Saúde Pet", Descricao = "Cobertura de procedimentos e abatimento automático na comanda", Categoria = "Financeiro", Ordem = 7, Ativo = true },
            new() { Codigo = ModulosPet.PortalTutor, Nome = "Portal Web do Tutor & Agendamento", Descricao = "Acesso externo do tutor para vacinas, exames, fotos e agendamento online", Categoria = "Tutor", Ordem = 8, Ativo = true },
            new() { Codigo = ModulosPet.WhatsApp, Nome = "Notificações WhatsApp", Descricao = "Avisos automáticos de banho pronto, vacinas, agenda e boletim de internação", Categoria = "Tutor", Ordem = 9, Ativo = true }
        };

        foreach (var mod in modulos)
        {
            if (!await context.ModulosSistema.AnyAsync(m => m.Codigo == mod.Codigo))
            {
                context.ModulosSistema.Add(mod);
            }
        }
        await context.SaveChangesAsync();

        // Ativar todos os módulos na empresa padrão inicial (Plano Grande) se ainda não configurados
        var empresas = await context.Empresas.IgnoreQueryFilters().ToListAsync();
        foreach (var empresa in empresas)
        {
            var jaTemModulos = await context.EmpresaModulos.IgnoreQueryFilters()
                .AnyAsync(em => em.EmpresaId == empresa.Id);

            if (!jaTemModulos)
            {
                foreach (var mod in modulos)
                {
                    context.EmpresaModulos.Add(new EmpresaModulo
                    {
                        EmpresaId = empresa.Id,
                        ModuloCodigo = mod.Codigo,
                        Ativo = true,
                        Origem = OrigemModuloEmpresa.Plano,
                        DataAtivacao = DateTime.UtcNow
                    });
                }
            }
        }
        await context.SaveChangesAsync();
    }
}
