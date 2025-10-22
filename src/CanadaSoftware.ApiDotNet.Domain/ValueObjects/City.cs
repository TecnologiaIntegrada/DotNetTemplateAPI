namespace CanadaSoftware.ApiDotNet.Domain.ValueObjects;

/// <summary>
/// Value Object para Cidade
/// </summary>
public record City
{
    public string Value { get; set; } = string.Empty;

    // Construtor sem parâmetros para EF Core
    public City() { }

    public City(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Cidade não pode ser vazia", nameof(value));

        Value = value.Trim();
    }

    public override string ToString() => Value;

    public static implicit operator string(City city) => city.Value;
}
