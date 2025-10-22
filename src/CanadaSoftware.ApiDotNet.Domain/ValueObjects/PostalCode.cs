using System.Text.RegularExpressions;

namespace CanadaSoftware.ApiDotNet.Domain.ValueObjects;

/// <summary>
/// Value Object para CEP (Código de Endereçamento Postal)
/// </summary>
public record PostalCode
{
    public string Value { get; init; } = string.Empty;

    // Construtor sem parâmetros para EF Core
    public PostalCode() { }

    public PostalCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("CEP não pode ser vazio", nameof(value));

        var cepNumerico = Regex.Replace(value, @"[^\d]", "");

        if (cepNumerico.Length != 8)
            throw new ArgumentException("CEP deve ter 8 dígitos", nameof(value));

        Value = cepNumerico;
    }

    public string FormatoCep() => $"{Value.Substring(0, 5)}-{Value.Substring(5, 3)}";

    public override string ToString() => FormatoCep();

    public static implicit operator string(PostalCode postalCode) => postalCode.Value;
}
