using FluentValidation;
using CanadaSoftware.ApiDotNet.Application;
using CanadaSoftware.ApiDotNet.Commom.ClientProperties;
using CanadaSoftware.ApiDotNet.Core;
using CanadaSoftware.ApiDotNet.Application.Data;
using CanadaSoftware.ApiDotNet.Application.MessageProducer;
using Serilog;
using Serilog.Context;
using System.Net;

namespace CanadaSoftware.ApiDotNet.Application.RequestHandlers.Cliente;

/// <summary>
/// Handler para atualização de cliente
/// </summary>
public class UpdateClienteHandler : ICommandHandler<UpdateClienteCommand, UpdateClienteResult>
{
	private readonly IClienteRepository _repository;
	private readonly IUnitOfWork _uow;
	private readonly IClienteEventProducer _eventProducer;
	private static readonly ILogger _logger = Serilog.Log.ForContext<UpdateClienteHandler>();
	private static string _logRoute = "v1/clientes";

	public UpdateClienteHandler(
		IClienteRepository repository,
		IUnitOfWork uow,
		IClienteEventProducer eventProducer)
	{
		_repository = repository ?? throw new ArgumentNullException(nameof(repository));
		_uow = uow ?? throw new ArgumentNullException(nameof(uow));
		_eventProducer = eventProducer ?? throw new ArgumentNullException(nameof(eventProducer));
	}

	public async Task<UpdateClienteResult> Handle(
		UpdateClienteCommand request,
		CancellationToken cancellationToken)
	{
		using var ctx = LogContext.PushProperty("ClienteId", request.Id);
		_logger.Debug("Received from - {Route} - {@Command}", _logRoute, request);

		try
		{
			// Buscar cliente
			_logger.Information("Buscando cliente com ID {Id}", request.Id);
			var cliente = await _repository.LoadAsync(request.Id, cancellationToken);

			if (cliente == null)
			{
				_logger.Warning("Cliente com ID {Id} não encontrado", request.Id);
				return UpdateClienteResult.NotFound();
			}

			// Atualizar informações básicas
			_logger.Information("Atualizando informações do cliente");
			cliente.AtualizarInformacoes(
				request.Nome,
				request.Sobrenome,
				request.Email,
				request.Celular,
				request.Profissao,
				request.RendaMensal,
				request.UsuarioAtualizacao);

			// Atualizar endereço se fornecido
			if (request.Endereco != null)
			{
				_logger.Information("Atualizando endereço do cliente");
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

			// Atualizar documento se fornecido
			if (request.Documento != null)
			{
				_logger.Information("Atualizando documento do cliente");
				cliente.AdicionarDocumento(
					request.Documento.Tipo,
					request.Documento.Numero,
					request.Documento.DataEmissao,
					request.Documento.OrgaoEmissor,
					request.Documento.Uf);
			}

			// Atualizar dados bancários se fornecidos
			if (request.DadosBancarios != null)
			{
				_logger.Information("Atualizando dados bancários do cliente");
				cliente.AdicionarDadosBancarios(
					request.DadosBancarios.Banco,
					request.DadosBancarios.Agencia,
					request.DadosBancarios.AgenciaDac,
					request.DadosBancarios.Conta,
					request.DadosBancarios.ContaDac,
					request.DadosBancarios.TipoConta);
			}

			// Atualizar dados do cônjuge se fornecidos
			if (request.NomeConjuge != null || request.CpfConjuge != null)
			{
				_logger.Information("Atualizando dados do cônjuge");
				cliente.AtualizarDadosConjuge(request.NomeConjuge, request.CpfConjuge);
			}

			// Salvar alterações
			_logger.Information("Salvando alterações do cliente");
			await _repository.SaveAsync(cliente, cancellationToken);
			await _uow.CommitAsync(cancellationToken);

			// Publicar evento no Kafka
			_logger.Information("Publicando evento ClienteAtualizado no Kafka");
			await _eventProducer.PublishClienteAtualizadoAsync(cliente, cancellationToken);

			_logger.Debug("Answered from - {Route} - Cliente atualizado com sucesso: {ClienteId}", _logRoute, cliente.Id);
			_logger.Information("Cliente atualizado com sucesso");

			return UpdateClienteResult.Success(cliente);
		}
		catch (ValidationException ex)
		{
			_logger.Error(ex, "Erro de validação ao atualizar cliente");

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
			_logger.Error(ex, "Erro ao atualizar cliente");
			return UpdateClienteResult.Fail(ex);
		}
	}

	private static UpdateClienteResult ResponseBadRequest(
		UpdateClienteCommand request,
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

		_logger.Error("UpdateClienteResult - BadRequest: {@Request}", request);
		return UpdateClienteResult.BadRequest(problemDetails, errorCode);
	}
}

/// <summary>
/// Command para atualização de cliente
/// </summary>
public record UpdateClienteCommand : ICommand<UpdateClienteResult>
{
	public Guid Id { get; init; }
	public Name? Nome { get; init; }
	public Name? Sobrenome { get; init; }
	public Email? Email { get; init; }
	public Cellphone? Celular { get; init; }
	public string? Profissao { get; init; }
	public decimal? RendaMensal { get; init; }
	public string? NomeConjuge { get; init; }
	public string? CpfConjuge { get; init; }
	public EnderecoDto? Endereco { get; init; }
	public DocumentoDto? Documento { get; init; }
	public DadosBancariosDto? DadosBancarios { get; init; }

	[System.Text.Json.Serialization.JsonIgnore]
	public string? UsuarioAtualizacao { get; set; }
}

/// <summary>
/// Resultado da atualização de cliente
/// </summary>
public abstract record UpdateClienteResult
{
	public static UpdateClienteResult Success(Domain.Cliente cliente) =>
		new UpdateClienteSuccess(cliente);

	public static UpdateClienteResult NotFound() =>
		new UpdateClienteNotFound();

	public static UpdateClienteResult Fail(Exception exception) =>
		new UpdateClienteFail(exception);

	public static UpdateClienteResult BadRequest(AppProblemDetails problemDetails, string errorCode) =>
		new UpdateClienteBadRequest(problemDetails, errorCode);
}

public record UpdateClienteSuccess(Domain.Cliente Cliente) : UpdateClienteResult;

public record UpdateClienteNotFound : UpdateClienteResult;

public record UpdateClienteFail(Exception Exception) : UpdateClienteResult
{
	public AppProblemDetails AppProblemDetails => new AppProblemDetails(Exception);
}

public record UpdateClienteBadRequest(AppProblemDetails ProblemDetails, string ErrorMessage) : UpdateClienteResult
{
	public IDictionary<string, string[]> Error => new Dictionary<string, string[]>
	{
		{ ProblemDetails.Detail!, new string[] { ErrorMessage } }
	};
}

/// <summary>
/// Validador para o command de atualização de cliente
/// </summary>
public class UpdateClienteCommandValidator : AbstractValidator<UpdateClienteCommand>
{
	public UpdateClienteCommandValidator()
	{
		RuleFor(cmd => cmd.Id)
			.NotEmpty()
			.WithMessage("Id é obrigatório");

		RuleFor(cmd => cmd.Nome)
			.SetValidator(Name.GetValidator()!)
			.When(cmd => cmd.Nome != null);

		RuleFor(cmd => cmd.Sobrenome)
			.SetValidator(Name.GetValidator()!)
			.When(cmd => cmd.Sobrenome != null);

		RuleFor(cmd => cmd.Email)
			.SetValidator(Email.GetValidator()!)
			.When(cmd => cmd.Email != null);

		RuleFor(cmd => cmd.Celular)
			.SetValidator(Cellphone.GetValidator()!)
			.When(cmd => cmd.Celular != null);
	}
}

