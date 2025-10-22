namespace CanadaSoftware.ApiDotNet.Domain.ValueObjects;

public record Country
{
    public string Value { get; init; }

    public Country(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("País não pode ser vazio", nameof(value));

        if (value.Length > 50)
            throw new ArgumentException("País não pode ter mais de 50 caracteres", nameof(value));

        Value = value.Trim();
    }

    public override string ToString() => Value;
    public static implicit operator string(Country country) => country.Value;
}

