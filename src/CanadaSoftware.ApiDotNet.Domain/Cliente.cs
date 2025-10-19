using FluentValidation;
using CanadaSoftware.ApiDotNet.Common;
using CanadaSoftware.ApiDotNet.Common.AddressProperties;
using CanadaSoftware.ApiDotNet.Common.BankDataProperties;
using CanadaSoftware.ApiDotNet.Common.DocumentProperties;
using CanadaSoftware.ApiDotNet.Common.ClientProperties;
using System.Diagnostics.CodeAnalysis;

namespace CanadaSoftware.ApiDotNet.Domain;

/// <summary>
/// Entidade de domínio Cliente - Agregado Raiz
/// Representa um cliente do sistema com todas as suas informações pessoais, documentos e dados bancários
/// </summary>
public class Cliente
{
	// Identificação
	public Guid Id { get; private set; }
	public string ClienteId { get; private set; } // ID externo do cliente
	public Cpf Cpf { get; private set; }
	public Name Nome { get; private set; }
	public Name Sobrenome { get; private set; }
	public Email? Email { get; private set; }
	public Cellphone Celular { get; private set; }
	
	// Informações pessoais
	public Sex Sexo { get; private set; }
	public int Dependentes { get; private set; }
	public BirthDate DataNascimento { get; private set; }
	public Naturalness? Naturalidade { get; private set; }
	public Nationality? Nacionalidade { get; private set; }
	public Name? NomeMae { get; private set; }
	public Name? NomePai { get; private set; }
	public string? Profissao { get; private set; }
	public string? EstadoCivil { get; private set; }
	public decimal? RendaMensal { get; private set; }
	
	// Informações complementares
	public string? NomeConjuge { get; private set; }
	public string? CpfConjuge { get; private set; }
	
	// Endereço
	public Jazz.Commom.Address? Endereco { get; private set; }
	
	// Documento
	public Jazz.Commom.Document? Documento { get; private set; }
	
	// Dados Bancários
	public BankData? DadosBancarios { get; private set; }
	
	// Auditoria
	public DateTime DataCadastro { get; private set; }
	public DateTime? DataAtualizacao { get; private set; }
	public string? UsuarioCadastro { get; private set; }
	public string? UsuarioAtualizacao { get; private set; }
	public bool Ativo { get; private set; }

#pragma warning disable CS8618
	[ExcludeFromCodeCoverage]
	protected Cliente() { }
#pragma warning restore CS8618

	private Cliente(
		Guid id,
		string clienteId,
		Cpf cpf,
		Name nome,
		Name sobrenome,
		Email? email,
		Cellphone celular,
		Sex sexo,
		int dependentes,
		BirthDate dataNascimento,
		Naturalness? naturalidade,
		Nationality? nacionalidade,
		Name? nomeMae,
		Name? nomePai,
		string? profissao,
		string? estadoCivil,
		decimal? rendaMensal,
		string? nomeConjuge,
		string? cpfConjuge,
		string? usuarioCadastro)
	{
		Id = id;
		ClienteId = clienteId;
		Cpf = cpf;
		Nome = nome;
		Sobrenome = sobrenome;
		Email = email;
		Celular = celular;
		Sexo = sexo;
		Dependentes = dependentes;
		DataNascimento = dataNascimento;
		Naturalidade = naturalidade;
		Nacionalidade = nacionalidade;
		NomeMae = nomeMae;
		NomePai = nomePai;
		Profissao = profissao;
		EstadoCivil = estadoCivil;
		RendaMensal = rendaMensal;
		NomeConjuge = nomeConjuge;
		CpfConjuge = cpfConjuge;
		UsuarioCadastro = usuarioCadastro;
		DataCadastro = DateTime.UtcNow;
		Ativo = true;
	}

	/// <summary>
	/// Factory method para criar um novo cliente
	/// </summary>
	public static Cliente Criar(
		string clienteId,
		Cpf cpf,
		Name nome,
		Name sobrenome,
		Email? email,
		Cellphone celular,
		Sex sexo,
		int dependentes,
		BirthDate dataNascimento,
		Naturalness? naturalidade,
		Nationality? nacionalidade,
		Name? nomeMae,
		Name? nomePai,
		string? profissao = null,
		string? estadoCivil = null,
		decimal? rendaMensal = null,
		string? nomeConjuge = null,
		string? cpfConjuge = null,
		string? usuarioCadastro = null)
	{
		var cliente = new Cliente(
			Guid.NewGuid(),
			clienteId,
			cpf,
			nome,
			sobrenome,
			email,
			celular,
			sexo,
			dependentes,
			dataNascimento,
			naturalidade,
			nacionalidade,
			nomeMae,
			nomePai,
			profissao,
			estadoCivil,
			rendaMensal,
			nomeConjuge,
			cpfConjuge,
			usuarioCadastro);

		GetValidator().ValidateAndThrow(cliente);
		return cliente;
	}

	/// <summary>
	/// Adiciona ou atualiza o endereço do cliente
	/// </summary>
	public void AdicionarEndereco(
		PostalCode cep,
		Street logradouro,
		string numero,
		string? complemento,
		District bairro,
		State estado,
		City cidade,
		Country pais,
		int? tempoResidencia = null)
	{
		Endereco = new Jazz.Commom.Address
		{
			PostalCode = cep,
			Street = logradouro,
			Number = numero,
			Complement = complemento ?? string.Empty,
			District = bairro,
			State = estado,
			City = cidade,
			Country = pais,
			ResidenceTime = tempoResidencia
		};
		
		DataAtualizacao = DateTime.UtcNow;
	}

	/// <summary>
	/// Adiciona ou atualiza o documento do cliente
	/// </summary>
	public void AdicionarDocumento(
		Jazz.Commom.DocumentProperties.Type tipo,
		DocumentNumber numero,
		IssuerDate dataEmissao,
		Issuer orgaoEmissor,
		FederativeUnity uf)
	{
		Documento = new Jazz.Commom.Document
		{
			Type = tipo,
			Number = numero,
			IssuerDate = dataEmissao,
			Issuer = orgaoEmissor,
			FederativeUnity = uf
		};
		
		DataAtualizacao = DateTime.UtcNow;
	}

	/// <summary>
	/// Adiciona ou atualiza os dados bancários do cliente
	/// </summary>
	public void AdicionarDadosBancarios(
		Bank banco,
		Agency agencia,
		AgencyDac agenciaDac,
		Account conta,
		AccountDac contaDac,
		Jazz.Commom.BankDataProperties.AccountType tipoConta)
	{
		DadosBancarios = new BankData
		{
			Bank = banco,
			Agency = agencia,
			AgencyDac = agenciaDac,
			Account = conta,
			AccountDac = contaDac,
			AccountType = tipoConta
		};
		
		DataAtualizacao = DateTime.UtcNow;
	}

	/// <summary>
	/// Atualiza informações básicas do cliente
	/// </summary>
	public void AtualizarInformacoes(
		Name? nome = null,
		Name? sobrenome = null,
		Email? email = null,
		Cellphone? celular = null,
		string? profissao = null,
		decimal? rendaMensal = null,
		string? usuarioAtualizacao = null)
	{
		if (nome != null) Nome = nome;
		if (sobrenome != null) Sobrenome = sobrenome;
		if (email != null) Email = email;
		if (celular != null) Celular = celular;
		if (profissao != null) Profissao = profissao;
		if (rendaMensal != null) RendaMensal = rendaMensal;
		
		UsuarioAtualizacao = usuarioAtualizacao;
		DataAtualizacao = DateTime.UtcNow;
		
		GetValidator().ValidateAndThrow(this);
	}

	/// <summary>
	/// Atualiza dados do cônjuge
	/// </summary>
	public void AtualizarDadosConjuge(string? nomeConjuge, string? cpfConjuge)
	{
		NomeConjuge = nomeConjuge;
		CpfConjuge = cpfConjuge;
		DataAtualizacao = DateTime.UtcNow;
	}

	/// <summary>
	/// Desativa o cliente
	/// </summary>
	public void Desativar(string? usuarioAtualizacao = null)
	{
		Ativo = false;
		UsuarioAtualizacao = usuarioAtualizacao;
		DataAtualizacao = DateTime.UtcNow;
	}

	/// <summary>
	/// Reativa o cliente
	/// </summary>
	public void Reativar(string? usuarioAtualizacao = null)
	{
		Ativo = true;
		UsuarioAtualizacao = usuarioAtualizacao;
		DataAtualizacao = DateTime.UtcNow;
	}

	/// <summary>
	/// Retorna o nome completo do cliente
	/// </summary>
	public string NomeCompleto => $"{Nome} {Sobrenome}";

	public static ClienteValidator GetValidator() => new();

	public class ClienteValidator : AbstractValidator<Cliente>
	{
		public ClienteValidator()
		{
			RuleFor(x => x.Id)
				.NotEmpty()
				.WithMessage("Id do cliente é obrigatório");

			RuleFor(x => x.ClienteId)
				.NotEmpty()
				.WithMessage("ClienteId é obrigatório");

			RuleFor(x => x.Cpf)
				.NotNull()
				.WithMessage("CPF é obrigatório")
				.SetValidator(Cpf.GetValidator());

			RuleFor(x => x.Nome)
				.NotNull()
				.WithMessage("Nome é obrigatório")
				.SetValidator(Name.GetValidator());

			RuleFor(x => x.Sobrenome)
				.NotNull()
				.WithMessage("Sobrenome é obrigatório")
				.SetValidator(Name.GetValidator());

			RuleFor(x => x.Email)
				.SetValidator(Email.GetValidator()!)
				.When(x => x.Email != null);

			RuleFor(x => x.Celular)
				.NotNull()
				.WithMessage("Celular é obrigatório")
				.SetValidator(Cellphone.GetValidator());

			RuleFor(x => x.Sexo)
				.NotNull()
				.WithMessage("Sexo é obrigatório")
				.SetValidator(Sex.GetValidator());

			RuleFor(x => x.DataNascimento)
				.NotNull()
				.WithMessage("Data de nascimento é obrigatória")
				.SetValidator(BirthDate.GetValidator());

			RuleFor(x => x.Dependentes)
				.GreaterThanOrEqualTo(0)
				.WithMessage("Número de dependentes não pode ser negativo");

			RuleFor(x => x.RendaMensal)
				.GreaterThanOrEqualTo(0)
				.When(x => x.RendaMensal.HasValue)
				.WithMessage("Renda mensal não pode ser negativa");

			RuleFor(x => x.NomeMae)
				.SetValidator(Name.GetValidator()!)
				.When(x => x.NomeMae != null);

			RuleFor(x => x.NomePai)
				.SetValidator(Name.GetValidator()!)
				.When(x => x.NomePai != null);

			RuleFor(x => x.Naturalidade)
				.SetValidator(Naturalness.GetValidator()!)
				.When(x => x.Naturalidade != null);

			RuleFor(x => x.Nacionalidade)
				.SetValidator(Nationality.GetValidator()!)
				.When(x => x.Nacionalidade != null);
		}
	}
}

