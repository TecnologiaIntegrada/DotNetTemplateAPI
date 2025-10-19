using System.Text.RegularExpressions;

namespace CanadaSoftware.ApiDotNet.Domain.ValueObjects;

/// <summary>
/// Value Object para Celular
/// </summary>
public record Cellphone
{
    public string Value { get; init; }

    public Cellphone(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Celular não pode ser vazio", nameof(value));

        var celularNumerico = Regex.Replace(value, @"[^\d]", "");

        if (celularNumerico.Length != 11)
            throw new ArgumentException("Celular deve ter 11 dígitos (DDD + 9 dígitos)", nameof(value));

        Value = celularNumerico;
    }

    public string FormatoCelular() => $"({Value.Substring(0, 2)}) {Value.Substring(2, 5)}-{Value.Substring(7, 4)}";

    public override string ToString() => FormatoCelular();

    public static implicit operator string(Cellphone cellphone) => cellphone.Value;
}

