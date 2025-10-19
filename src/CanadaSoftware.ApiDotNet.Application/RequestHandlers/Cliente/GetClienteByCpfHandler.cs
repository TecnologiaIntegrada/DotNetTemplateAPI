using CanadaSoftware.ApiDotNet.Application;
using CanadaSoftware.ApiDotNet.Application.Data;
using Serilog;
using Serilog.Context;

namespace CanadaSoftware.ApiDotNet.Application.RequestHandlers.Cliente;

/// <summary>
/// Handler para buscar cliente por CPF
/// </summary>
public class GetClienteByCpfHandler : IQueryHandler<GetClienteByCpfQuery, GetClienteResult>
{
	private readonly IClienteRepository _repository;
	private static readonly ILogger _logger = Serilog.Log.ForContext<GetClienteByCpfHandler>();
	private static string _logRoute = "v1/clientes/cpf";

	public GetClienteByCpfHandler(IClienteRepository repository)
	{
		_repository = repository ?? throw new ArgumentNullException(nameof(repository));
	}

	public async Task<GetClienteResult> Handle(
		GetClienteByCpfQuery request,
		CancellationToken cancellationToken)
	{
		using var ctx = LogContext.PushProperty("Cpf", request.Cpf);
		_logger.Debug("Received from - {Route} - {@Query}", _logRoute, request);

		try
		{
			_logger.Information("Buscando cliente por CPF {Cpf}", request.Cpf);
			var cliente = await _repository.GetByCpfAsync(request.Cpf, cancellationToken);

			if (cliente == null)
			{
				_logger.Warning("Cliente com CPF {Cpf} não encontrado", request.Cpf);
				return GetClienteResult.NotFound();
			}

			_logger.Information("Cliente encontrado com sucesso");
			return GetClienteResult.Success(cliente);
		}
		catch (Exception ex)
		{
			_logger.Error(ex, "Erro ao buscar cliente por CPF");
			return GetClienteResult.Fail(ex);
		}
	}
}

/// <summary>
/// Query para buscar cliente por CPF
/// </summary>
public record GetClienteByCpfQuery(string Cpf) : IQuery<GetClienteResult>;

