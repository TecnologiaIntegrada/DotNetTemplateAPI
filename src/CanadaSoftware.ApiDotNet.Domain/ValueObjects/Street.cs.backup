namespace CanadaSoftware.ApiDotNet.Domain.ValueObjects;

public record Street
{
    public string Value { get; init; }

    public Street(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Logradouro não pode ser vazio", nameof(value));

        if (value.Length > 200)
            throw new ArgumentException("Logradouro não pode ter mais de 200 caracteres", nameof(value));

        Value = value.Trim();
    }

    public override string ToString() => Value;
    public static implicit operator string(Street street) => street.Value;
}

