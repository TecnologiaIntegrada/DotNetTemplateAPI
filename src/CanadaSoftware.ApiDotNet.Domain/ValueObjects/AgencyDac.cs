namespace CanadaSoftware.ApiDotNet.Domain.ValueObjects;

public record AgencyDac
{
    public string Value { get; set; } = string.Empty;

    public AgencyDac(string value)

    // Construtor sem parâmetros para EF Core
    public AgencyDac() { }
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Dígito verificador da agência não pode ser vazio", nameof(value));

        if (value.Length > 2)
            throw new ArgumentException("Dígito verificador da agência não pode ter mais de 2 caracteres", nameof(value));

        Value = value.Trim();
    }

    public override string ToString() => Value;
    public static implicit operator string(AgencyDac agencyDac) => agencyDac.Value;
}

