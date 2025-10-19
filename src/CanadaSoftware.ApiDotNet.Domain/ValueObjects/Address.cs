namespace CanadaSoftware.ApiDotNet.Domain.ValueObjects;

/// <summary>
/// Value Object para Endereço
/// </summary>
public record Address
{
    public PostalCode PostalCode { get; init; }
    public Street Street { get; init; }
    public string Number { get; init; }
    public string? Complement { get; init; }
    public District District { get; init; }
    public State State { get; init; }
    public City City { get; init; }
    public Country Country { get; init; }

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

