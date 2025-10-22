namespace CanadaSoftware.ApiDotNet.Domain.ValueObjects;

public record Bank
{
    public string Value { get; set; } = string.Empty;

    // Construtor sem parâmetros para EF Core
    public Bank() { }

    public Bank(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Banco não pode ser vazio", nameof(value));

        if (value.Length != 3)
            throw new ArgumentException("Código do banco deve ter 3 dígitos", nameof(value));

        Value = value.Trim();
    }

    public override string ToString() => Value;

    public static implicit operator string(Bank bank) => bank.Value;
}
