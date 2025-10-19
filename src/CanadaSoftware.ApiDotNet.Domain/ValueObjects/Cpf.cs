using System.Text.RegularExpressions;

namespace CanadaSoftware.ApiDotNet.Domain.ValueObjects;

/// <summary>
/// Value Object para CPF (Cadastro de Pessoa Física)
/// </summary>
public record Cpf
{
    public string Value { get; init; }

    public Cpf(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("CPF não pode ser vazio", nameof(value));

        var cpfNumerico = Regex.Replace(value, @"[^\d]", "");

        if (cpfNumerico.Length != 11)
            throw new ArgumentException("CPF deve ter 11 dígitos", nameof(value));

        if (!ValidarCpf(cpfNumerico))
            throw new ArgumentException("CPF inválido", nameof(value));

        Value = cpfNumerico;
    }

    private static bool ValidarCpf(string cpf)
    {
        if (cpf.Distinct().Count() == 1)
            return false;

        int soma = 0;
        for (int i = 0; i < 9; i++)
            soma += int.Parse(cpf[i].ToString()) * (10 - i);

        int resto = soma % 11;
        int digitoVerificador1 = resto < 2 ? 0 : 11 - resto;

        if (int.Parse(cpf[9].ToString()) != digitoVerificador1)
            return false;

        soma = 0;
        for (int i = 0; i < 10; i++)
            soma += int.Parse(cpf[i].ToString()) * (11 - i);

        resto = soma % 11;
        int digitoVerificador2 = resto < 2 ? 0 : 11 - resto;

        return int.Parse(cpf[10].ToString()) == digitoVerificador2;
    }

    public string FormatoCpf() => $"{Value.Substring(0, 3)}.{Value.Substring(3, 3)}.{Value.Substring(6, 3)}-{Value.Substring(9, 2)}";

    public override string ToString() => FormatoCpf();

    public static implicit operator string(Cpf cpf) => cpf.Value;
}

