namespace CanadaSoftware.ApiDotNet.Domain.ValueObjects;

public record Agency
{
    public string Value { get; set; } = string.Empty;

    // Construtor sem parâmetros para EF Core
    public Agency() { }

    public Agency(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Agência não pode ser vazia", nameof(value));

        if (value.Length > 10)
            throw new ArgumentException("Agência não pode ter mais de 10 caracteres", nameof(value));

        Value = value.Trim();
    }

    public override string ToString() => Value;

    public static implicit operator string(Agency agency) => agency.Value;
}
