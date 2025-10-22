namespace CanadaSoftware.ApiDotNet.Domain.ValueObjects;

public record DocumentType
{
    public string Value { get; set; } = string.Empty;

    // Construtor sem parâmetros para EF Core
    public DocumentType() { }

    public DocumentType(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Tipo de documento não pode ser vazio", nameof(value));

        var tipo = value.Trim().ToUpperInvariant();

        if (tipo != "RG" && tipo != "CNH" && tipo != "RNE" && tipo != "PASSAPORTE")
            throw new ArgumentException("Tipo de documento deve ser RG, CNH, RNE ou PASSAPORTE", nameof(value));

        Value = tipo;
    }

    public override string ToString() => Value;

    public static implicit operator string(DocumentType documentType) => documentType.Value;
}
