namespace CanadaSoftware.ApiDotNet.Domain.ValueObjects;

/// <summary>
/// Value Object para Dados Bancários
/// </summary>
public record BankData
{
    public Bank Bank { get; init; } = null!;
    public Agency Agency { get; init; } = null!;
    public AgencyDac AgencyDac { get; init; } = null!;
    public Account Account { get; init; } = null!;
    public AccountDac AccountDac { get; init; } = null!;
    public AccountType AccountType { get; init; } = null!;

    // Construtor sem parâmetros para EF Core
    public BankData() { }

    public BankData(Bank bank, Agency agency, AgencyDac agencyDac, Account account, AccountDac accountDac, AccountType accountType)
    {
        Bank = bank;
        Agency = agency;
        AgencyDac = agencyDac;
        Account = account;
        AccountDac = accountDac;
        AccountType = accountType;
    }

    public override string ToString() => $"{Bank} - Ag: {Agency}-{AgencyDac} - Conta: {Account}-{AccountDac} ({AccountType})";
}
