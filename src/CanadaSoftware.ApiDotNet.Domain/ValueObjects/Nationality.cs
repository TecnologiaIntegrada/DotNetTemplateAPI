namespace CanadaSoftware.ApiDotNet.Domain.ValueObjects;

/// <summary>
/// Value Object para Nacionalidade
/// </summary>
public record Nationality
{
    public string Value { get; set; } = string.Empty;

    // Construtor sem parâmetros para EF Core
    public Nationality() { }

    public Nationality(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Nacionalidade não pode ser vazia", nameof(value));

        if (value.Length > 50)
            throw new ArgumentException("Nacionalidade não pode ter mais de 50 caracteres", nameof(value));

        Value = value.Trim();
    }

    public override string ToString() => Value;

    public static implicit operator string(Nationality nationality) => nationality.Value;
}
