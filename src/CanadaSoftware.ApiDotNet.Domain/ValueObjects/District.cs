namespace CanadaSoftware.ApiDotNet.Domain.ValueObjects;

public record District
{
    public string Value { get; init; }

    public District(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Bairro não pode ser vazio", nameof(value));

        if (value.Length > 100)
            throw new ArgumentException("Bairro não pode ter mais de 100 caracteres", nameof(value));

        Value = value.Trim();
    }

    public override string ToString() => Value;
    public static implicit operator string(District district) => district.Value;
}

