namespace CanadaSoftware.ApiDotNet.Domain.ValueObjects;

/// <summary>
/// Value Object para Documento
/// </summary>
public record Document
{
    public DocumentType DocumentType { get; init; } = null!;
    public DocumentNumber DocumentNumber { get; init; } = null!;
    public IssuerDate IssuerDate { get; init; } = null!;
    public Issuer Issuer { get; init; } = null!;
    public FederativeUnity FederativeUnity { get; init; } = null!;

    // Construtor sem parâmetros para EF Core
    public Document() { }

    public Document(DocumentType documentType, DocumentNumber documentNumber, IssuerDate issuerDate, Issuer issuer, FederativeUnity federativeUnity)
    {
        DocumentType = documentType;
        DocumentNumber = documentNumber;
        IssuerDate = issuerDate;
        Issuer = issuer;
        FederativeUnity = federativeUnity;
    }

    public override string ToString() => $"{DocumentType} {DocumentNumber} - {Issuer}/{FederativeUnity} - {IssuerDate}";
}
