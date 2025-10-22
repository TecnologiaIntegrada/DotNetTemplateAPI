namespace CanadaSoftware.ApiDotNet.Domain.ValueObjects;

public record AccountType
{
    public string Value { get; set; } = string.Empty;

    public AccountType(string value)

    // Construtor sem parâmetros para EF Core
    public AccountType() { }
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Tipo de conta não pode ser vazio", nameof(value));

        var tipo = value.Trim().ToUpperInvariant();

        if (tipo != "CORRENTE" && tipo != "POUPANCA" && tipo != "SALARIO")
            throw new ArgumentException("Tipo de conta deve ser CORRENTE, POUPANCA ou SALARIO", nameof(value));

        Value = tipo;
    }

    public override string ToString() => Value;
    public static implicit operator string(AccountType accountType) => accountType.Value;
}

