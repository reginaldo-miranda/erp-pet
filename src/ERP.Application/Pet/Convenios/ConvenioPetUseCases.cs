using ERP.Application.Common.Behaviors;
using ERP.Application.Common.Interfaces;
using ERP.Application.Common.Models;
using ERP.Domain.Core.Entities.Pet;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Application.Pet.Convenios;

public record ConvenioPetDto(
    int Id,
    string Nome,
    string? Cnpj,
    string? Telefone,
    string? Email,
    decimal PercentualCoberturaPadrao,
    string? Observacoes,
    bool Ativo);

public record GetConveniosPetQuery : IRequest<Result<List<ConvenioPetDto>>>;

public class GetConveniosPetQueryHandler : IRequestHandler<GetConveniosPetQuery, Result<List<ConvenioPetDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetConveniosPetQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<ConvenioPetDto>>> Handle(GetConveniosPetQuery request, CancellationToken cancellationToken)
    {
        var lista = await _context.ConveniosPet
            .AsNoTracking()
            .OrderBy(c => c.Nome)
            .Select(c => new ConvenioPetDto(
                c.Id,
                c.Nome,
                c.Cnpj,
                c.Telefone,
                c.Email,
                c.PercentualCoberturaPadrao,
                c.Observacoes,
                c.Ativo))
            .ToListAsync(cancellationToken);

        return Result<List<ConvenioPetDto>>.Ok(lista);
    }
}

[RequerModulo(ModulosPet.ConveniosPet)]
public record CreateConvenioPetCommand(
    string Nome,
    string? Cnpj,
    string? Telefone,
    string? Email,
    decimal PercentualCoberturaPadrao,
    string? Observacoes) : IRequest<Result<int>>;

public class CreateConvenioPetCommandHandler : IRequestHandler<CreateConvenioPetCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentEmpresaService _empresaService;

    public CreateConvenioPetCommandHandler(IApplicationDbContext context, ICurrentEmpresaService empresaService)
    {
        _context = context;
        _empresaService = empresaService;
    }

    public async Task<Result<int>> Handle(CreateConvenioPetCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
            return Result<int>.Falha("Nome do convênio é obrigatório.");

        var convenio = new ConvenioPet
        {
            EmpresaId = _empresaService.EmpresaId ?? 1,
            Nome = request.Nome.Trim(),
            Cnpj = request.Cnpj,
            Telefone = request.Telefone,
            Email = request.Email,
            PercentualCoberturaPadrao = Math.Clamp(request.PercentualCoberturaPadrao, 0, 100),
            Observacoes = request.Observacoes,
            Ativo = true
        };

        _context.ConveniosPet.Add(convenio);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<int>.Ok(convenio.Id);
    }
}
