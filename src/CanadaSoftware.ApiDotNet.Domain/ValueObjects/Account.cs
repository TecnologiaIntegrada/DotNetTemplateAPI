namespace CanadaSoftware.ApiDotNet.Domain.ValueObjects;

public record Account
{
    public string Value { get; set; } = string.Empty;

    public Account(string value)

    // Construtor sem parâmetros para EF Core
    public Account() { }
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Conta não pode ser vazia", nameof(value));

        if (value.Length > 20)
            throw new ArgumentException("Conta não pode ter mais de 20 caracteres", nameof(value));

        Value = value.Trim();
    }

    public override string ToString() => Value;
    public static implicit operator string(Account account) => account.Value;
}

