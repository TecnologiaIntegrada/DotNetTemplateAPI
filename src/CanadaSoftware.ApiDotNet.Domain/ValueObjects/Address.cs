namespace CanadaSoftware.ApiDotNet.Domain.ValueObjects;

/// <summary>
/// Value Object para Endereço
/// </summary>
public record Address
{
    public PostalCode PostalCode { get; set; } = null!;
    public Street Street { get; set; } = null!;
    public string Number { get; set; } = string.Empty;
    public string? Complement { get; set; }
    public District District { get; set; } = null!;
    public State State { get; set; } = null!;
    public City City { get; set; } = null!;
    public Country Country { get; set; } = null!;

    // Construtor sem parâmetros para EF Core
    public Address() { }

    public Address(
        PostalCode postalCode,
        Street street,
        string number,
        string? complement,
        District district,
        State state,
        City city,
        Country country)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException("Número não pode ser vazio", nameof(number));

        PostalCode = postalCode;
        Street = street;
        Number = number.Trim();
        Complement = complement?.Trim();
        District = district;
        State = state;
        City = city;
        Country = country;
    }

    public override string ToString() => 
        $"{Street}, {Number}{(string.IsNullOrEmpty(Complement) ? "" : $", {Complement}")} - {District} - {City}/{State} - {PostalCode}";
}
