using System.Text.RegularExpressions;

namespace CanadaSoftware.ApiDotNet.Domain.ValueObjects;

/// <summary>
/// Value Object para CPF
/// </summary>
public record Cpf
{
    public string Value { get; set; } = string.Empty;

    // Construtor sem parâmetros para EF Core
    public Cpf() { }

    public Cpf(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("CPF não pode ser vazio", nameof(value));

        var cpfNumerico = Regex.Replace(value, @"[^\d]", "");

        if (cpfNumerico.Length != 11)
            throw new ArgumentException("CPF deve ter 11 dígitos", nameof(value));

        if (!IsValidCpf(cpfNumerico))
            throw new ArgumentException("CPF inválido", nameof(value));

        Value = cpfNumerico;
    }

    private static bool IsValidCpf(string cpf)
    {
        if (cpf.Length != 11 || cpf.All(c => c == cpf[0]))
            return false;

        var sum = 0;
        for (int i = 0; i < 9; i++)
            sum += int.Parse(cpf[i].ToString()) * (10 - i);

        var remainder = sum % 11;
        var digit1 = remainder < 2 ? 0 : 11 - remainder;

        if (int.Parse(cpf[9].ToString()) != digit1)
            return false;

        sum = 0;
        for (int i = 0; i < 10; i++)
            sum += int.Parse(cpf[i].ToString()) * (11 - i);

        remainder = sum % 11;
        var digit2 = remainder < 2 ? 0 : 11 - remainder;

        return int.Parse(cpf[10].ToString()) == digit2;
    }

    public string FormatoCpf() => $"{Value.Substring(0, 3)}.{Value.Substring(3, 3)}.{Value.Substring(6, 3)}-{Value.Substring(9, 2)}";

    public override string ToString() => FormatoCpf();

    public static implicit operator string(Cpf cpf) => cpf.Value;
}
