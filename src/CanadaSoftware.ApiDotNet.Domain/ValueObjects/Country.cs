namespace CanadaSoftware.ApiDotNet.Domain.ValueObjects;

/// <summary>
/// Value Object para País
/// </summary>
public record Country
{
    public string Value { get; set; } = string.Empty;

    // Construtor sem parâmetros para EF Core
    public Country() { }

    public Country(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("País não pode ser vazio", nameof(value));

        Value = value.Trim();
    }

    public override string ToString() => Value;

    public static implicit operator string(Country country) => country.Value;
}
