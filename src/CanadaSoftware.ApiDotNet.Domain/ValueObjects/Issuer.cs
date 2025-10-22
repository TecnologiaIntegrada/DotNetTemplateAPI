namespace CanadaSoftware.ApiDotNet.Domain.ValueObjects;

public record Issuer
{
    public string Value { get; set; } = string.Empty;

    public Issuer(string value)

    // Construtor sem parâmetros para EF Core
    public Issuer() { }
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Órgão emissor não pode ser vazio", nameof(value));

        if (value.Length > 50)
            throw new ArgumentException("Órgão emissor não pode ter mais de 50 caracteres", nameof(value));

        Value = value.Trim().ToUpperInvariant();
    }

    public override string ToString() => Value;
    public static implicit operator string(Issuer issuer) => issuer.Value;
}

