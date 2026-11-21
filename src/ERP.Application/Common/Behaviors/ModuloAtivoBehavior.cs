using System.Reflection;
using ERP.Application.Common.Exceptions;
using ERP.Application.Common.Interfaces;
using MediatR;

namespace ERP.Application.Common.Behaviors;

/// <summary>
/// Pipeline Behavior do MediatR que valida se os módulos exigidos pelo Request estão ativos na empresa atual.
/// </summary>
public class ModuloAtivoBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IModuloService _moduloService;

    public ModuloAtivoBehavior(IModuloService moduloService)
    {
        _moduloService = moduloService;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var attrs = typeof(TRequest).GetCustomAttributes<RequerModuloAttribute>().ToList();
        if (attrs.Count > 0)
        {
            foreach (var attr in attrs)
            {
                var ativo = await _moduloService.ModuloAtivoAsync(attr.ModuloCodigo, cancellationToken);
                if (!ativo)
                {
                    throw new ForbiddenException($"O módulo '{attr.ModuloCodigo}' não está habilitado para esta empresa.");
                }
            }
        }

        return await next();
    }
}
