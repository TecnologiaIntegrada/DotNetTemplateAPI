namespace CanadaSoftware.ApiDotNet.Domain.ValueObjects;

/// <summary>
/// Value Object para Nome
/// </summary>
public record Name
{
    public string Value { get; init; }

    public Name(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Nome não pode ser vazio", nameof(value));

        if (value.Length < 2)
            throw new ArgumentException("Nome deve ter pelo menos 2 caracteres", nameof(value));

        if (value.Length > 100)
            throw new ArgumentException("Nome não pode ter mais de 100 caracteres", nameof(value));

        Value = value.Trim();
    }

    public override string ToString() => Value;

    public static implicit operator string(Name name) => name.Value;
}

