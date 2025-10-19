using DotNetCore.CAP;
using CanadaSoftware.ApiDotNet.Domain;
using Serilog;

namespace CanadaSoftware.ApiDotNet.Application.MessageProducer;

/// <summary>
/// Producer de eventos de Cliente para Kafka
/// </summary>
public interface IClienteEventProducer
{
	Task PublishClienteCriadoAsync(Cliente cliente, CancellationToken cancellationToken = default);
	Task PublishClienteAtualizadoAsync(Cliente cliente, CancellationToken cancellationToken = default);
	Task PublishClienteDesativadoAsync(Guid clienteId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Implementação do producer de eventos de Cliente
/// </summary>
public class ClienteEventProducer : IClienteEventProducer
{
	private readonly ICapPublisher _capPublisher;
	private static readonly ILogger _logger = Log.ForContext<ClienteEventProducer>();

	private const string TOPIC_CLIENTE_CRIADO = "cliente.criado";
	private const string TOPIC_CLIENTE_ATUALIZADO = "cliente.atualizado";
	private const string TOPIC_CLIENTE_DESATIVADO = "cliente.desativado";

	public ClienteEventProducer(ICapPublisher capPublisher)
	{
		_capPublisher = capPublisher ?? throw new ArgumentNullException(nameof(capPublisher));
	}

	public async Task PublishClienteCriadoAsync(Cliente cliente, CancellationToken cancellationToken = default)
	{
		try
		{
			var evento = new ClienteCriadoEvent
			{
				Id = cliente.Id,
				ClienteId = cliente.ClienteId,
				Cpf = cliente.Cpf.Value,
				NomeCompleto = cliente.NomeCompleto,
				Email = cliente.Email?.Value,
				Celular = cliente.Celular.Value,
				DataCadastro = cliente.DataCadastro,
				Ativo = cliente.Ativo,
				UsuarioCadastro = cliente.UsuarioCadastro
			};

			await _capPublisher.PublishAsync(TOPIC_CLIENTE_CRIADO, evento, cancellationToken: cancellationToken);

			_logger.Information(
				"Evento {EventType} publicado para Cliente {ClienteId} no tópico {Topic}",
				nameof(ClienteCriadoEvent),
				cliente.Id,
				TOPIC_CLIENTE_CRIADO);
		}
		catch (Exception ex)
		{
			_logger.Error(ex, "Erro ao publicar evento ClienteCriado para Cliente {ClienteId}", cliente.Id);
			throw;
		}
	}

	public async Task PublishClienteAtualizadoAsync(Cliente cliente, CancellationToken cancellationToken = default)
	{
		try
		{
			var evento = new ClienteAtualizadoEvent
			{
				Id = cliente.Id,
				ClienteId = cliente.ClienteId,
				Cpf = cliente.Cpf.Value,
				NomeCompleto = cliente.NomeCompleto,
				Email = cliente.Email?.Value,
				Celular = cliente.Celular?.Value,
				DataAtualizacao = cliente.DataAtualizacao,
				UsuarioAtualizacao = cliente.UsuarioAtualizacao
			};

			await _capPublisher.PublishAsync(TOPIC_CLIENTE_ATUALIZADO, evento, cancellationToken: cancellationToken);

			_logger.Information(
				"Evento {EventType} publicado para Cliente {ClienteId} no tópico {Topic}",
				nameof(ClienteAtualizadoEvent),
				cliente.Id,
				TOPIC_CLIENTE_ATUALIZADO);
		}
		catch (Exception ex)
		{
			_logger.Error(ex, "Erro ao publicar evento ClienteAtualizado para Cliente {ClienteId}", cliente.Id);
			throw;
		}
	}

	public async Task PublishClienteDesativadoAsync(Guid clienteId, CancellationToken cancellationToken = default)
	{
		try
		{
			var evento = new ClienteDesativadoEvent
			{
				Id = clienteId,
				DataDesativacao = DateTime.UtcNow
			};

			await _capPublisher.PublishAsync(TOPIC_CLIENTE_DESATIVADO, evento, cancellationToken: cancellationToken);

			_logger.Information(
				"Evento {EventType} publicado para Cliente {ClienteId} no tópico {Topic}",
				nameof(ClienteDesativadoEvent),
				clienteId,
				TOPIC_CLIENTE_DESATIVADO);
		}
		catch (Exception ex)
		{
			_logger.Error(ex, "Erro ao publicar evento ClienteDesativado para Cliente {ClienteId}", clienteId);
			throw;
		}
	}
}

/// <summary>
/// Evento de cliente criado
/// </summary>
public record ClienteCriadoEvent
{
	public Guid Id { get; init; }
	public string ClienteId { get; init; } = string.Empty;
	public string Cpf { get; init; } = string.Empty;
	public string NomeCompleto { get; init; } = string.Empty;
	public string? Email { get; init; }
	public string Celular { get; init; } = string.Empty;
	public DateTime DataCadastro { get; init; }
	public bool Ativo { get; init; }
	public string? UsuarioCadastro { get; init; }
}

/// <summary>
/// Evento de cliente atualizado
/// </summary>
public record ClienteAtualizadoEvent
{
	public Guid Id { get; init; }
	public string ClienteId { get; init; } = string.Empty;
	public string Cpf { get; init; } = string.Empty;
	public string NomeCompleto { get; init; } = string.Empty;
	public string? Email { get; init; }
	public string? Celular { get; init; }
	public DateTime? DataAtualizacao { get; init; }
	public string? UsuarioAtualizacao { get; init; }
}

/// <summary>
/// Evento de cliente desativado
/// </summary>
public record ClienteDesativadoEvent
{
	public Guid Id { get; init; }
	public DateTime DataDesativacao { get; init; }
}

