using System.Text.RegularExpressions;

namespace CanadaSoftware.ApiDotNet.Domain.ValueObjects;

/// <summary>
/// Value Object para Email
/// </summary>
public record Email
{
    public string Value { get; init; } = string.Empty;

    // Construtor sem parâmetros para EF Core
    public Email() { }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email não pode ser vazio", nameof(value));

        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        if (!emailRegex.IsMatch(value))
            throw new ArgumentException("Email inválido", nameof(value));

        Value = value.Trim().ToLowerInvariant();
    }

    public override string ToString() => Value;

    public static implicit operator string(Email email) => email.Value;
}
