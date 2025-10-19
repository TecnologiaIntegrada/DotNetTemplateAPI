using FluentValidation;
using Jazz.Application;
using Jazz.Commom;
using Jazz.Commom.AddressProperties;
using Jazz.Commom.BankDataProperties;
using Jazz.Commom.ClientProperties;
using Jazz.Commom.DocumentProperties;
using Jazz.Common.DocumentProperties;
using Jazz.Core;
using CanadaSoftware.ApiDotNet.Application.Data;
using CanadaSoftware.ApiDotNet.Application.MessageProducer;
using CanadaSoftware.ApiDotNet.Domain;
using Serilog;
using Serilog.Context;
using System.Net;

namespace CanadaSoftware.ApiDotNet.Application.RequestHandlers.Cliente;

/// <summary>
/// Handler para criação de clientes
/// </summary>
public class CreateClienteHandler : ICommandHandler<CreateClienteCommand, CreateClienteResult>
{
	private readonly IClienteRepository _repository;
	private readonly IUnitOfWork _uow;
	private readonly IClienteEventProducer _eventProducer;
	private static readonly ILogger _logger = Serilog.Log.ForContext<CreateClienteHandler>();
	private static string _logRoute = "v1/clientes";

	public CreateClienteHandler(
		IClienteRepository repository,
		IUnitOfWork uow,
		IClienteEventProducer eventProducer)
	{
		_repository = repository ?? throw new ArgumentNullException(nameof(repository));
		_uow = uow ?? throw new ArgumentNullException(nameof(uow));
		_eventProducer = eventProducer ?? throw new ArgumentNullException(nameof(eventProducer));
	}

	public async Task<CreateClienteResult> Handle(
		CreateClienteCommand request, 
		CancellationToken cancellationToken)
	{
		using var ctx = LogContext.PushProperty("Cpf", request.Cpf.Value);
		_logger.Debug("Received from - {Route} - {@Command}", _logRoute, request);

		try
		{
			// Validar se já existe cliente com o CPF
			_logger.Information("Verificando se já existe cliente com o CPF {Cpf}", request.Cpf.Value);
			var existeCliente = await _repository.ExistsByCpfAsync(request.Cpf.Value, cancellationToken);
			
			if (existeCliente)
			{
				_logger.Warning("Cliente com CPF {Cpf} já existe", request.Cpf.Value);
				return CreateClienteResult.BadRequest(
					new AppProblemDetails(
						type: "Cliente/JaExiste",
						title: "Cliente já cadastrado",
						errorCode: "400",
						detail: $"Já existe um cliente cadastrado com o CPF {request.Cpf.Value}",
						status: (int)HttpStatusCode.BadRequest),
					"400");
			}

			// Criar o cliente
			_logger.Information("Criando novo cliente");
			var cliente = Domain.Cliente.Criar(
				request.ClienteId,
				request.Cpf,
				request.Nome,
				request.Sobrenome,
				request.Email,
				request.Celular,
				request.Sexo,
				request.Dependentes,
				request.DataNascimento,
				request.Naturalidade,
				request.Nacionalidade,
				request.NomeMae,
				request.NomePai,
				request.Profissao,
				request.EstadoCivil,
				request.RendaMensal,
				request.NomeConjuge,
				request.CpfConjuge,
				request.UsuarioCadastro);

			// Adicionar endereço se fornecido
			if (request.Endereco != null)
			{
				_logger.Information("Adicionando endereço ao cliente");
				cliente.AdicionarEndereco(
					request.Endereco.Cep,
					request.Endereco.Logradouro,
					request.Endereco.Numero,
					request.Endereco.Complemento,
					request.Endereco.Bairro,
					request.Endereco.Estado,
					request.Endereco.Cidade,
					request.Endereco.Pais,
					request.Endereco.TempoResidencia);
			}

			// Adicionar documento se fornecido
			if (request.Documento != null)
			{
				_logger.Information("Adicionando documento ao cliente");
				cliente.AdicionarDocumento(
					request.Documento.Tipo,
					request.Documento.Numero,
					request.Documento.DataEmissao,
					request.Documento.OrgaoEmissor,
					request.Documento.Uf);
			}

			// Adicionar dados bancários se fornecidos
			if (request.DadosBancarios != null)
			{
				_logger.Information("Adicionando dados bancários ao cliente");
				cliente.AdicionarDadosBancarios(
					request.DadosBancarios.Banco,
					request.DadosBancarios.Agencia,
					request.DadosBancarios.AgenciaDac,
					request.DadosBancarios.Conta,
					request.DadosBancarios.ContaDac,
					request.DadosBancarios.TipoConta);
			}

			// Salvar o cliente
			_logger.Information("Salvando cliente no banco de dados");
			await _repository.SaveAsync(cliente, cancellationToken);
			await _uow.CommitAsync(cancellationToken);

			// Publicar evento no Kafka
			_logger.Information("Publicando evento ClienteCriado no Kafka");
			await _eventProducer.PublishClienteCriadoAsync(cliente, cancellationToken);

			_logger.Debug("Answered from - {Route} - Cliente criado com sucesso: {ClienteId}", _logRoute, cliente.Id);
			_logger.Information("Cliente criado com sucesso");

			return CreateClienteResult.Success(cliente);
		}
		catch (ValidationException ex)
		{
			_logger.Error(ex, "Erro de validação ao criar cliente");
			
			var errorCode = "400";
			var errorMessage = "Erro de validação";
			
			if (ex.Errors.Any())
			{
				errorCode = ex.Errors.First().ErrorCode ?? errorCode;
				errorMessage = ex.Errors.First().ErrorMessage ?? errorMessage;
			}
			
			return ResponseBadRequest(request, ex.Message, errorCode, errorMessage);
		}
		catch (Exception ex)
		{
			_logger.Error(ex, "Erro ao criar cliente");
			return CreateClienteResult.Fail(ex);
		}
	}

	private static CreateClienteResult ResponseBadRequest(
		CreateClienteCommand request, 
		string detail, 
		string errorCode, 
		string error)
	{
		var problemDetails = new AppProblemDetails(
			type: "about:blank",
			title: error,
			detail: detail,
			status: (int)HttpStatusCode.BadRequest,
			errorCode: errorCode);

		_logger.Error("CreateClienteResult - BadRequest: {@Request}", request);
		return CreateClienteResult.BadRequest(problemDetails, errorCode);
	}
}

/// <summary>
/// Command para criação de cliente
/// </summary>
public record CreateClienteCommand : ICommand<CreateClienteResult>
{
	public string ClienteId { get; init; }
	public Cpf Cpf { get; init; }
	public Name Nome { get; init; }
	public Name Sobrenome { get; init; }
	public Email? Email { get; init; }
	public Cellphone Celular { get; init; }
	public Sex Sexo { get; init; }
	public int Dependentes { get; init; }
	public BirthDate DataNascimento { get; init; }
	public Naturalness? Naturalidade { get; init; }
	public Nationality? Nacionalidade { get; init; }
	public Name? NomeMae { get; init; }
	public Name? NomePai { get; init; }
	public string? Profissao { get; init; }
	public string? EstadoCivil { get; init; }
	public decimal? RendaMensal { get; init; }
	public string? NomeConjuge { get; init; }
	public string? CpfConjuge { get; init; }
	public EnderecoDto? Endereco { get; init; }
	public DocumentoDto? Documento { get; init; }
	public DadosBancariosDto? DadosBancarios { get; init; }
	
	[System.Text.Json.Serialization.JsonIgnore]
	public string? UsuarioCadastro { get; set; }

#pragma warning disable CS8618
	public CreateClienteCommand() { }
#pragma warning restore CS8618
}

/// <summary>
/// DTO para endereço
/// </summary>
public record EnderecoDto
{
	public PostalCode Cep { get; init; }
	public Street Logradouro { get; init; }
	public string Numero { get; init; }
	public string? Complemento { get; init; }
	public District Bairro { get; init; }
	public State Estado { get; init; }
	public City Cidade { get; init; }
	public Country Pais { get; init; }
	public int? TempoResidencia { get; init; }

#pragma warning disable CS8618
	public EnderecoDto() { }
#pragma warning restore CS8618
}

/// <summary>
/// DTO para documento
/// </summary>
public record DocumentoDto
{
	public Jazz.Commom.DocumentProperties.Type Tipo { get; init; }
	public DocumentNumber Numero { get; init; }
	public IssuerDate DataEmissao { get; init; }
	public Issuer OrgaoEmissor { get; init; }
	public FederativeUnity Uf { get; init; }

#pragma warning disable CS8618
	public DocumentoDto() { }
#pragma warning restore CS8618
}

/// <summary>
/// DTO para dados bancários
/// </summary>
public record DadosBancariosDto
{
	public Bank Banco { get; init; }
	public Agency Agencia { get; init; }
	public AgencyDac AgenciaDac { get; init; }
	public Account Conta { get; init; }
	public AccountDac ContaDac { get; init; }
	public Jazz.Commom.BankDataProperties.AccountType TipoConta { get; init; }

#pragma warning disable CS8618
	public DadosBancariosDto() { }
#pragma warning restore CS8618
}

/// <summary>
/// Resultado da criação de cliente
/// </summary>
public abstract record CreateClienteResult
{
	public static CreateClienteResult Success(Domain.Cliente cliente) =>
		new CreateClienteSuccess(cliente);

	public static CreateClienteResult Fail(Exception exception) =>
		new CreateClienteFail(exception);

	public static CreateClienteResult BadRequest(AppProblemDetails problemDetails, string errorCode) =>
		new CreateClienteBadRequest(problemDetails, errorCode);
}

public record CreateClienteSuccess(Domain.Cliente Cliente) : CreateClienteResult;

public record CreateClienteFail(Exception Exception) : CreateClienteResult
{
	public AppProblemDetails AppProblemDetails => new AppProblemDetails(Exception);
}

public record CreateClienteBadRequest(AppProblemDetails ProblemDetails, string ErrorMessage) : CreateClienteResult
{
	public IDictionary<string, string[]> Error => new Dictionary<string, string[]>
	{
		{ ProblemDetails.Detail!, new string[] { ErrorMessage } }
	};
}

/// <summary>
/// Validador para o command de criação de cliente
/// </summary>
public class CreateClienteCommandValidator : AbstractValidator<CreateClienteCommand>
{
	public CreateClienteCommandValidator()
	{
		RuleFor(cmd => cmd.ClienteId)
			.NotEmpty()
			.NotNull()
			.WithMessage("ClienteId é obrigatório");

		RuleFor(cmd => cmd.Cpf)
			.NotEmpty()
			.NotNull()
			.WithMessage("CPF é obrigatório")
			.SetValidator(Cpf.GetValidator());

		RuleFor(cmd => cmd.Nome)
			.NotEmpty()
			.NotNull()
			.WithMessage("Nome é obrigatório")
			.SetValidator(Name.GetValidator());

		RuleFor(cmd => cmd.Sobrenome)
			.NotEmpty()
			.NotNull()
			.WithMessage("Sobrenome é obrigatório")
			.SetValidator(Name.GetValidator());

		RuleFor(cmd => cmd.Celular)
			.NotEmpty()
			.NotNull()
			.WithMessage("Celular é obrigatório")
			.SetValidator(Cellphone.GetValidator());

		RuleFor(cmd => cmd.Sexo)
			.NotEmpty()
			.NotNull()
			.WithMessage("Sexo é obrigatório")
			.SetValidator(Sex.GetValidator());

		RuleFor(cmd => cmd.DataNascimento)
			.NotEmpty()
			.NotNull()
			.WithMessage("Data de nascimento é obrigatória")
			.SetValidator(BirthDate.GetValidator());

		RuleFor(cmd => cmd.Dependentes)
			.GreaterThanOrEqualTo(0)
			.WithMessage("Número de dependentes não pode ser negativo");

		RuleFor(cmd => cmd.Email)
			.SetValidator(Email.GetValidator()!)
			.When(cmd => cmd.Email != null);

		RuleFor(cmd => cmd.NomeMae)
			.SetValidator(Name.GetValidator()!)
			.When(cmd => cmd.NomeMae != null);

		RuleFor(cmd => cmd.NomePai)
			.SetValidator(Name.GetValidator()!)
			.When(cmd => cmd.NomePai != null);

		RuleFor(cmd => cmd.Naturalidade)
			.SetValidator(Naturalness.GetValidator()!)
			.When(cmd => cmd.Naturalidade != null);

		RuleFor(cmd => cmd.Nacionalidade)
			.SetValidator(Nationality.GetValidator()!)
			.When(cmd => cmd.Nacionalidade != null);
	}
}

