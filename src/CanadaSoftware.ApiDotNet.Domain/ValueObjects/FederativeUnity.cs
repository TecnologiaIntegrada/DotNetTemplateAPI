namespace CanadaSoftware.ApiDotNet.Domain.ValueObjects;

public record FederativeUnity
{
    public string Value { get; set; } = string.Empty;

    public FederativeUnity(string value)

    // Construtor sem parâmetros para EF Core
    public FederativeUnity() { }
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("UF não pode ser vazia", nameof(value));

        var uf = value.Trim().ToUpperInvariant();

        if (uf.Length != 2)
            throw new ArgumentException("UF deve ter 2 caracteres", nameof(value));

        Value = uf;
    }

    public override string ToString() => Value;
    public static implicit operator string(FederativeUnity federativeUnity) => federativeUnity.Value;
}

