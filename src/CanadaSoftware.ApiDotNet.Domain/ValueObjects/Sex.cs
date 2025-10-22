namespace CanadaSoftware.ApiDotNet.Domain.ValueObjects;

/// <summary>
/// Value Object para Sexo
/// </summary>
public record Sex
{
    public string Value { get; set; } = string.Empty;

    // Construtor sem parâmetros para EF Core
    public Sex() { }

    public Sex(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Sexo não pode ser vazio", nameof(value));

        var sexo = value.Trim().ToUpperInvariant();

        if (sexo != "M" && sexo != "F" && sexo != "MASCULINO" && sexo != "FEMININO")
            throw new ArgumentException("Sexo deve ser M, F, MASCULINO ou FEMININO", nameof(value));

        Value = sexo == "MASCULINO" ? "M" : sexo == "FEMININO" ? "F" : sexo;
    }

    public override string ToString() => Value;

    public static implicit operator string(Sex sex) => sex.Value;
}
