namespace CanadaSoftware.ApiDotNet.Domain.ValueObjects;

/// <summary>
/// Value Object para Naturalidade (Cidade de nascimento)
/// </summary>
public record Naturalness
{
    public string Value { get; set; } = string.Empty;

    // Construtor sem parâmetros para EF Core
    public Naturalness() { }

    public Naturalness(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Naturalidade não pode ser vazia", nameof(value));

        if (value.Length > 100)
            throw new ArgumentException("Naturalidade não pode ter mais de 100 caracteres", nameof(value));

        Value = value.Trim();
    }

    public override string ToString() => Value;

    public static implicit operator string(Naturalness naturalness) => naturalness.Value;
}
