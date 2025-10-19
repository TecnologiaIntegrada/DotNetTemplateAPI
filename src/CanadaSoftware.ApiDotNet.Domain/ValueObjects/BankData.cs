namespace CanadaSoftware.ApiDotNet.Domain.ValueObjects;

/// <summary>
/// Value Object para Dados Bancários
/// </summary>
public record BankData
{
    public Bank Bank { get; init; }
    public Agency Agency { get; init; }
    public AgencyDac AgencyDac { get; init; }
    public Account Account { get; init; }
    public AccountDac AccountDac { get; init; }
    public AccountType AccountType { get; init; }

    public BankData(
        Bank bank,
        Agency agency,
        AgencyDac agencyDac,
        Account account,
        AccountDac accountDac,
        AccountType accountType)
    {
        Bank = bank;
        Agency = agency;
        AgencyDac = agencyDac;
        Account = account;
        AccountDac = accountDac;
        AccountType = accountType;
    }

    public override string ToString() => $"Banco {Bank} - Agência {Agency}-{AgencyDac} - Conta {Account}-{AccountDac}";
}

