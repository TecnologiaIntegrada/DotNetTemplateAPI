namespace CanadaSoftware.ApiDotNet.Domain.ValueObjects;

/// <summary>
/// Value Object para Data de Nascimento
/// </summary>
public record BirthDate
{
    public DateTime Value { get; set; }

    // Construtor sem parâmetros para EF Core
    public BirthDate() { }

    public BirthDate(DateTime value)
    {
        if (value > DateTime.Now)
            throw new ArgumentException("Data de nascimento não pode ser futura", nameof(value));

        if (value < DateTime.Now.AddYears(-150))
            throw new ArgumentException("Data de nascimento inválida", nameof(value));

        Value = value.Date;
    }

    public override string ToString() => Value.ToString("dd/MM/yyyy");

    public static implicit operator DateTime(BirthDate birthDate) => birthDate.Value;
}
