namespace CanadaSoftware.ApiDotNet.Domain.ValueObjects;

/// <summary>
/// Value Object para Documento
/// </summary>
public record Document
{
    public DocumentType DocumentType { get; set; } = null!;
    public DocumentNumber DocumentNumber { get; set; } = null!;
    public IssuerDate IssuerDate { get; set; } = null!;
    public Issuer Issuer { get; set; } = null!;
    public FederativeUnity FederativeUnity { get; set; } = null!;

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
