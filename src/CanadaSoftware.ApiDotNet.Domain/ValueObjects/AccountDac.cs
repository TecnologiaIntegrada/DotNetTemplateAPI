namespace CanadaSoftware.ApiDotNet.Domain.ValueObjects;

public record AccountDac
{
    public string Value { get; set; } = string.Empty;

    // Construtor sem parâmetros para EF Core
    public AccountDac() { }

    public AccountDac(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Dígito verificador da conta não pode ser vazio", nameof(value));

        if (value.Length > 2)
            throw new ArgumentException("Dígito verificador da conta não pode ter mais de 2 caracteres", nameof(value));

        Value = value.Trim();
    }

    public override string ToString() => Value;

    public static implicit operator string(AccountDac accountDac) => accountDac.Value;
}
