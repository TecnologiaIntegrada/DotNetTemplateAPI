namespace CanadaSoftware.ApiDotNet.Domain.ValueObjects;

/// <summary>
/// Value Object para Estado
/// </summary>
public record State
{
    public string Value { get; set; } = string.Empty;

    // Construtor sem parâmetros para EF Core
    public State() { }

    public State(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Estado não pode ser vazio", nameof(value));

        Value = value.Trim();
    }

    public override string ToString() => Value;

    public static implicit operator string(State state) => state.Value;
}
