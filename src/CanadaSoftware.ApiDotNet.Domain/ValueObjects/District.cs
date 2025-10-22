namespace CanadaSoftware.ApiDotNet.Domain.ValueObjects;

/// <summary>
/// Value Object para Bairro
/// </summary>
public record District
{
    public string Value { get; set; } = string.Empty;

    // Construtor sem parâmetros para EF Core
    public District() { }

    public District(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Bairro não pode ser vazio", nameof(value));

        Value = value.Trim();
    }

    public override string ToString() => Value;

    public static implicit operator string(District district) => district.Value;
}
