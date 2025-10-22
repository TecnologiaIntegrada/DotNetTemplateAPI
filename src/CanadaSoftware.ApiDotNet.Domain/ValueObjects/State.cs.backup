namespace CanadaSoftware.ApiDotNet.Domain.ValueObjects;

public record State
{
    public string Value { get; init; }

    public State(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Estado não pode ser vazio", nameof(value));

        var uf = value.Trim().ToUpperInvariant();

        if (uf.Length != 2)
            throw new ArgumentException("Estado deve ter 2 caracteres (UF)", nameof(value));

        Value = uf;
    }

    public override string ToString() => Value;
    public static implicit operator string(State state) => state.Value;
}

