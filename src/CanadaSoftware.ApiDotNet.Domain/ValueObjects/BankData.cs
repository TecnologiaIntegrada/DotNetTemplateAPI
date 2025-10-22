namespace CanadaSoftware.ApiDotNet.Domain.ValueObjects;

/// <summary>
/// Value Object para Dados Bancários
/// </summary>
public record BankData
{
    public Bank Bank { get; set; } = null!;
    public Agency Agency { get; set; } = null!;
    public AgencyDac AgencyDac { get; set; } = null!;
    public Account Account { get; set; } = null!;
    public AccountDac AccountDac { get; set; } = null!;
    public AccountType AccountType { get; set; } = null!;

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
