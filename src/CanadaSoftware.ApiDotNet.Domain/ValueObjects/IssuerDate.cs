namespace CanadaSoftware.ApiDotNet.Domain.ValueObjects;

public record IssuerDate
{
    public string Value { get; set; } = string.Empty;

    public IssuerDate(DateTime value)

    // Construtor sem parâmetros para EF Core
    public IssuerDate() { }
    {
        if (value > DateTime.Now)
            throw new ArgumentException("Data de emissão não pode ser futura", nameof(value));

        if (value < DateTime.Now.AddYears(-100))
            throw new ArgumentException("Data de emissão inválida", nameof(value));

        Value = value.Date;
    }

    public override string ToString() => Value.ToString("dd/MM/yyyy");
    public static implicit operator DateTime(IssuerDate issuerDate) => issuerDate.Value;
}

