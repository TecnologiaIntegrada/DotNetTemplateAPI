namespace CanadaSoftware.ApiDotNet.Domain.ValueObjects;

/// <summary>
/// Value Object para Rua/Logradouro
/// </summary>
public record Street
{
    public string Value { get; set; } = string.Empty;

    // Construtor sem parâmetros para EF Core
    public Street() { }

    public Street(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Rua não pode ser vazia", nameof(value));

        Value = value.Trim();
    }

    public override string ToString() => Value;

    public static implicit operator string(Street street) => street.Value;
}
