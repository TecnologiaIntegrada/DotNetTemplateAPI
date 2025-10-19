namespace CanadaSoftware.ApiDotNet.Domain.ValueObjects;

/// <summary>
/// Value Object para Documento (RG, CNH, etc)
/// </summary>
public record Document
{
    public DocumentType DocumentType { get; init; }
    public DocumentNumber DocumentNumber { get; init; }
    public IssuerDate IssuerDate { get; init; }
    public Issuer Issuer { get; init; }
    public FederativeUnity FederativeUnity { get; init; }

    public Document(
        DocumentType documentType,
        DocumentNumber documentNumber,
        IssuerDate issuerDate,
        Issuer issuer,
        FederativeUnity federativeUnity)
    {
        DocumentType = documentType;
        DocumentNumber = documentNumber;
        IssuerDate = issuerDate;
        Issuer = issuer;
        FederativeUnity = federativeUnity;
    }

    public override string ToString() => $"{DocumentType}: {DocumentNumber} - {Issuer}/{FederativeUnity}";
}

