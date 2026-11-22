using ERP.Application.Common.Interfaces;
using ERP.Application.Common.Models;
using ERP.Domain.Core.Entities.Pet;
using ERP.Domain.Core.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Application.Pet.Pets;

public record TutorVinculoDto(
    int Id,
    int ClienteId,
    string NomeTutor,
    string? Celular,
    TipoVinculoTutor TipoVinculo,
    bool PodeAssinarTermos,
    bool RecebeNotificacoes);

public record PetDto(
    int Id,
    string Nome,
    EspeciePet Especie,
    string? Raca,
    string? CorPelagem,
    SexoPet Sexo,
    bool Castrado,
    DateTime? DataNascimento,
    int? IdadeEstimadaMeses,
    PortePet Porte,
    TipoPelagemPet TipoPelagem,
    decimal? PesoAtualKg,
    string? Microchip,
    TemperamentoPet Temperamento,
    string? Alergias,
    string? RestricoesAlimentares,
    string? Observacoes,
    int? ConvenioPetId,
    string? NomeConvenio,
    string? NumeroCarteirinhaConvenio,
    bool Obito,
    List<TutorVinculoDto> Tutores);

public record GetPetsQuery(string? Busca = null, EspeciePet? Especie = null, int? ClienteId = null) : IRequest<Result<List<PetDto>>>;

public class GetPetsQueryHandler : IRequestHandler<GetPetsQuery, Result<List<PetDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetPetsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<PetDto>>> Handle(GetPetsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Pets
            .AsNoTracking()
            .Include(p => p.ConvenioPet)
            .Include(p => p.Tutores)
                .ThenInclude(t => t.Cliente)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Busca))
        {
            var term = request.Busca.Trim().ToLower();
            query = query.Where(p =>
                p.Nome.ToLower().Contains(term) ||
                (p.Raca != null && p.Raca.ToLower().Contains(term)) ||
                (p.Microchip != null && p.Microchip.ToLower().Contains(term)) ||
                p.Tutores.Any(t => t.Cliente.Nome.ToLower().Contains(term)));
        }

        if (request.Especie.HasValue)
            query = query.Where(p => p.Especie == request.Especie.Value);

        if (request.ClienteId.HasValue)
            query = query.Where(p => p.Tutores.Any(t => t.ClienteId == request.ClienteId.Value));

        var pets = await query
            .OrderBy(p => p.Nome)
            .Take(200)
            .ToListAsync(cancellationToken);

        var dtos = pets.Select(p => new PetDto(
            p.Id,
            p.Nome,
            p.Especie,
            p.Raca,
            p.CorPelagem,
            p.Sexo,
            p.Castrado,
            p.DataNascimento,
            p.IdadeEstimadaMeses,
            p.Porte,
            p.TipoPelagem,
            p.PesoAtualKg,
            p.Microchip,
            p.Temperamento,
            p.Alergias,
            p.RestricoesAlimentares,
            p.Observacoes,
            p.ConvenioPetId,
            p.ConvenioPet?.Nome,
            p.NumeroCarteirinhaConvenio,
            p.Obito,
            p.Tutores.Select(t => new TutorVinculoDto(
                t.Id,
                t.ClienteId,
                t.Cliente.Nome,
                t.Cliente.Celular ?? t.Cliente.Telefone,
                t.TipoVinculo,
                t.PodeAssinarTermos,
                t.RecebeNotificacoes)).ToList()
        )).ToList();

        return Result<List<PetDto>>.Ok(dtos);
    }
}

public record CreatePetCommand(
    string Nome,
    EspeciePet Especie,
    string? Raca,
    string? CorPelagem,
    SexoPet Sexo,
    bool Castrado,
    DateTime? DataNascimento,
    int? IdadeEstimadaMeses,
    PortePet Porte,
    TipoPelagemPet TipoPelagem,
    decimal? PesoAtualKg,
    string? Microchip,
    TemperamentoPet Temperamento,
    string? Alergias,
    string? RestricoesAlimentares,
    string? Observacoes,
    int? ConvenioPetId,
    string? NumeroCarteirinhaConvenio,
    int TutorPrincipalClienteId) : IRequest<Result<int>>;

public class CreatePetCommandHandler : IRequestHandler<CreatePetCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentEmpresaService _empresaService;

    public CreatePetCommandHandler(IApplicationDbContext context, ICurrentEmpresaService empresaService)
    {
        _context = context;
        _empresaService = empresaService;
    }

    public async Task<Result<int>> Handle(CreatePetCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
            return Result<int>.Falha("O nome do Pet é obrigatório.");

        var tutorExiste = await _context.Clientes.AnyAsync(c => c.Id == request.TutorPrincipalClienteId, cancellationToken);
        if (!tutorExiste)
            return Result<int>.Falha("Tutor principal não encontrado.");

        var empresaId = _empresaService.EmpresaId ?? 1;

        var pet = new Domain.Core.Entities.Pet.Pet
        {
            EmpresaId = empresaId,
            Nome = request.Nome.Trim(),
            Especie = request.Especie,
            Raca = request.Raca,
            CorPelagem = request.CorPelagem,
            Sexo = request.Sexo,
            Castrado = request.Castrado,
            DataNascimento = request.DataNascimento?.ToUniversalTime(),
            IdadeEstimadaMeses = request.IdadeEstimadaMeses,
            Porte = request.Porte,
            TipoPelagem = request.TipoPelagem,
            PesoAtualKg = request.PesoAtualKg,
            Microchip = request.Microchip,
            Temperamento = request.Temperamento,
            Alergias = request.Alergias,
            RestricoesAlimentares = request.RestricoesAlimentares,
            Observacoes = request.Observacoes,
            ConvenioPetId = request.ConvenioPetId,
            NumeroCarteirinhaConvenio = request.NumeroCarteirinhaConvenio
        };

        pet.Tutores.Add(new TutorPet
        {
            EmpresaId = empresaId,
            ClienteId = request.TutorPrincipalClienteId,
            TipoVinculo = TipoVinculoTutor.Principal,
            PodeAssinarTermos = true,
            RecebeNotificacoes = true
        });

        _context.Pets.Add(pet);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Ok(pet.Id);
    }
}

public record UpdatePetCommand(
    int Id,
    string Nome,
    EspeciePet Especie,
    string? Raca,
    string? CorPelagem,
    SexoPet Sexo,
    bool Castrado,
    DateTime? DataNascimento,
    int? IdadeEstimadaMeses,
    PortePet Porte,
    TipoPelagemPet TipoPelagem,
    decimal? PesoAtualKg,
    string? Microchip,
    TemperamentoPet Temperamento,
    string? Alergias,
    string? RestricoesAlimentares,
    string? Observacoes,
    int? ConvenioPetId,
    string? NumeroCarteirinhaConvenio,
    bool Obito,
    DateTime? DataObito,
    string? CausaObito) : IRequest<Result<bool>>;

public class UpdatePetCommandHandler : IRequestHandler<UpdatePetCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public UpdatePetCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(UpdatePetCommand request, CancellationToken cancellationToken)
    {
        var pet = await _context.Pets.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (pet == null)
            return Result<bool>.Falha("Pet não encontrado.");

        pet.Nome = request.Nome.Trim();
        pet.Especie = request.Especie;
        pet.Raca = request.Raca;
        pet.CorPelagem = request.CorPelagem;
        pet.Sexo = request.Sexo;
        pet.Castrado = request.Castrado;
        pet.DataNascimento = request.DataNascimento?.ToUniversalTime();
        pet.IdadeEstimadaMeses = request.IdadeEstimadaMeses;
        pet.Porte = request.Porte;
        pet.TipoPelagem = request.TipoPelagem;
        pet.PesoAtualKg = request.PesoAtualKg;
        pet.Microchip = request.Microchip;
        pet.Temperamento = request.Temperamento;
        pet.Alergias = request.Alergias;
        pet.RestricoesAlimentares = request.RestricoesAlimentares;
        pet.Observacoes = request.Observacoes;
        pet.ConvenioPetId = request.ConvenioPetId;
        pet.NumeroCarteirinhaConvenio = request.NumeroCarteirinhaConvenio;
        pet.Obito = request.Obito;
        pet.DataObito = request.DataObito?.ToUniversalTime();
        pet.CausaObito = request.CausaObito;

        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Ok(true);
    }
}

public record VincularTutorPetCommand(
    int PetId,
    int ClienteId,
    TipoVinculoTutor TipoVinculo,
    bool PodeAssinarTermos,
    bool RecebeNotificacoes) : IRequest<Result<bool>>;

public class VincularTutorPetCommandHandler : IRequestHandler<VincularTutorPetCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentEmpresaService _empresaService;

    public VincularTutorPetCommandHandler(IApplicationDbContext context, ICurrentEmpresaService empresaService)
    {
        _context = context;
        _empresaService = empresaService;
    }

    public async Task<Result<bool>> Handle(VincularTutorPetCommand request, CancellationToken cancellationToken)
    {
        var jaExiste = await _context.TutorPets
            .AnyAsync(tp => tp.PetId == request.PetId && tp.ClienteId == request.ClienteId, cancellationToken);
        if (jaExiste)
            return Result<bool>.Falha("Este tutor já está vinculado ao Pet.");

        _context.TutorPets.Add(new TutorPet
        {
            EmpresaId = _empresaService.EmpresaId ?? 1,
            PetId = request.PetId,
            ClienteId = request.ClienteId,
            TipoVinculo = request.TipoVinculo,
            PodeAssinarTermos = request.PodeAssinarTermos,
            RecebeNotificacoes = request.RecebeNotificacoes
        });

        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Ok(true);
    }
}

public record RemoverVinculoTutorPetCommand(int TutorPetId) : IRequest<Result<bool>>;

public class RemoverVinculoTutorPetCommandHandler : IRequestHandler<RemoverVinculoTutorPetCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public RemoverVinculoTutorPetCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(RemoverVinculoTutorPetCommand request, CancellationToken cancellationToken)
    {
        var vinculo = await _context.TutorPets.FirstOrDefaultAsync(tp => tp.Id == request.TutorPetId, cancellationToken);
        if (vinculo == null)
            return Result<bool>.Falha("Vínculo não encontrado.");

        var totalTutores = await _context.TutorPets.CountAsync(tp => tp.PetId == vinculo.PetId, cancellationToken);
        if (totalTutores <= 1)
            return Result<bool>.Falha("O Pet precisa ter ao menos um tutor vinculado.");

        _context.TutorPets.Remove(vinculo);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Ok(true);
    }
}
