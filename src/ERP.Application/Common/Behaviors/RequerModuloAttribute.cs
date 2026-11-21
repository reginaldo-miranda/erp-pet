namespace ERP.Application.Common.Behaviors;

/// <summary>
/// Marca um Request (Command/Query) como dependente de um módulo ativo para a empresa atual.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class RequerModuloAttribute : Attribute
{
    public string ModuloCodigo { get; }

    public RequerModuloAttribute(string moduloCodigo)
    {
        ModuloCodigo = moduloCodigo;
    }
}
