using Jazz.Application;
using CanadaSoftware.ApiDotNet.Application.Data;
using Serilog;
using Serilog.Context;
using System.Net;

namespace CanadaSoftware.ApiDotNet.Application.RequestHandlers.Cliente;

/// <summary>
/// Handler para buscar cliente por ID
/// </summary>
public class GetClienteHandler : IQueryHandler<GetClienteQuery, GetClienteResult>
{
	private readonly IClienteRepository _repository;
	private static readonly ILogger _logger = Serilog.Log.ForContext<GetClienteHandler>();
	private static string _logRoute = "v1/clientes";

	public GetClienteHandler(IClienteRepository repository)
	{
		_repository = repository ?? throw new ArgumentNullException(nameof(repository));
	}

	public async Task<GetClienteResult> Handle(
		GetClienteQuery request,
		CancellationToken cancellationToken)
	{
		using var ctx = LogContext.PushProperty("ClienteId", request.Id);
		_logger.Debug("Received from - {Route} - {@Query}", _logRoute, request);

		try
		{
			_logger.Information("Buscando cliente por ID {Id}", request.Id);
			var cliente = await _repository.LoadAsync(request.Id, cancellationToken);

			if (cliente == null)
			{
				_logger.Warning("Cliente com ID {Id} não encontrado", request.Id);
				return GetClienteResult.NotFound();
			}

			_logger.Information("Cliente encontrado com sucesso");
			return GetClienteResult.Success(cliente);
		}
		catch (Exception ex)
		{
			_logger.Error(ex, "Erro ao buscar cliente");
			return GetClienteResult.Fail(ex);
		}
	}
}

/// <summary>
/// Query para buscar cliente por ID
/// </summary>
public record GetClienteQuery(Guid Id) : IQuery<GetClienteResult>;

/// <summary>
/// Resultado da busca de cliente
/// </summary>
public abstract record GetClienteResult
{
	public static GetClienteResult Success(Domain.Cliente cliente) =>
		new GetClienteSuccess(cliente);

	public static GetClienteResult NotFound() =>
		new GetClienteNotFound();

	public static GetClienteResult Fail(Exception exception) =>
		new GetClienteFail(exception);
}

public record GetClienteSuccess(Domain.Cliente Cliente) : GetClienteResult;

public record GetClienteNotFound : GetClienteResult;

public record GetClienteFail(Exception Exception) : GetClienteResult
{
	public AppProblemDetails AppProblemDetails => new AppProblemDetails(Exception);
}

