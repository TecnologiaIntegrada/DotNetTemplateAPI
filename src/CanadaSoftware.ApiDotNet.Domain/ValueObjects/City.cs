namespace CanadaSoftware.ApiDotNet.Domain.ValueObjects;

public record City
{
    public string Value { get; init; }

    public City(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Cidade não pode ser vazia", nameof(value));

        if (value.Length > 100)
            throw new ArgumentException("Cidade não pode ter mais de 100 caracteres", nameof(value));

        Value = value.Trim();
    }

    public override string ToString() => Value;
    public static implicit operator string(City city) => city.Value;
}

