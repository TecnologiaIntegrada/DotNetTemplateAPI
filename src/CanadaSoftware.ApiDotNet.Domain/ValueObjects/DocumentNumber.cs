namespace CanadaSoftware.ApiDotNet.Domain.ValueObjects;

public record DocumentNumber
{
    public string Value { get; set; } = string.Empty;

    // Construtor sem parâmetros para EF Core
    public DocumentNumber() { }

    public DocumentNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Número do documento não pode ser vazio", nameof(value));

        if (value.Length > 20)
            throw new ArgumentException("Número do documento não pode ter mais de 20 caracteres", nameof(value));

        Value = value.Trim();
    }

    public override string ToString() => Value;

    public static implicit operator string(DocumentNumber documentNumber) => documentNumber.Value;
}
