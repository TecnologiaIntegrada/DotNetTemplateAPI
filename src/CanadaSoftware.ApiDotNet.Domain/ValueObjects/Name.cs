namespace CanadaSoftware.ApiDotNet.Domain.ValueObjects;

/// <summary>
/// Value Object para Nome
/// </summary>
public record Name
{
    public string Value { get; set; } = string.Empty;

    // Construtor sem parâmetros para EF Core
    public Name() { }

    public Name(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Nome não pode ser vazio", nameof(value));

        Value = value.Trim();
    }

    public override string ToString() => Value;

    public static implicit operator string(Name name) => name.Value;
}
