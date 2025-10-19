using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using Serilog;
using System.Net;

namespace Jazz.LoanProposal.Application.Configuration;

/// <summary>
/// Configurações de políticas de resiliência usando Polly
/// </summary>
public static class PollyPolicies
{
	private static readonly ILogger _logger = Log.ForContext(typeof(PollyPolicies));

	/// <summary>
	/// Política de retry com backoff exponencial para HTTP
	/// </summary>
	public static AsyncRetryPolicy<HttpResponseMessage> GetHttpRetryPolicy()
	{
		return HttpPolicyExtensions
			.HandleTransientHttpError()
			.Or<TimeoutException>()
			.WaitAndRetryAsync(
				retryCount: 3,
				sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
				onRetry: (outcome, timespan, retryCount, context) =>
				{
					_logger.Warning(
						"Tentativa {RetryCount} após {Delay}s devido a {Exception}",
						retryCount,
						timespan.TotalSeconds,
						outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString());
				});
	}

	/// <summary>
	/// Política de Circuit Breaker para HTTP
	/// </summary>
	public static IAsyncPolicy<HttpResponseMessage> GetHttpCircuitBreakerPolicy()
	{
		return HttpPolicyExtensions
			.HandleTransientHttpError()
			.Or<TimeoutException>()
			.CircuitBreakerAsync(
				handledEventsAllowedBeforeBreaking: 5,
				durationOfBreak: TimeSpan.FromSeconds(30),
				onBreak: (outcome, breakDelay) =>
				{
					_logger.Error(
						"Circuit Breaker aberto por {Delay}s devido a {Exception}",
						breakDelay.TotalSeconds,
						outcome.Exception?.Message ?? "Falhas consecutivas");
				},
				onReset: () =>
				{
					_logger.Information("Circuit Breaker resetado");
				},
				onHalfOpen: () =>
				{
					_logger.Information("Circuit Breaker em modo half-open");
				});
	}

	/// <summary>
	/// Política de timeout para operações HTTP
	/// </summary>
	public static IAsyncPolicy<HttpResponseMessage> GetHttpTimeoutPolicy(int timeoutSeconds = 30)
	{
		return Policy
			.TimeoutAsync<HttpResponseMessage>(
				TimeSpan.FromSeconds(timeoutSeconds),
				onTimeoutAsync: (context, timespan, abandonedTask) =>
				{
					_logger.Warning("Timeout de {Timeout}s atingido", timespan.TotalSeconds);
					return Task.CompletedTask;
				});
	}

	/// <summary>
	/// Combina todas as políticas HTTP (Retry + Circuit Breaker + Timeout)
	/// </summary>
	public static IAsyncPolicy<HttpResponseMessage> GetCombinedHttpPolicy()
	{
		return Policy.WrapAsync(
			GetHttpRetryPolicy(),
			GetHttpCircuitBreakerPolicy(),
			GetHttpTimeoutPolicy());
	}

	/// <summary>
	/// Política de retry para operações de banco de dados
	/// </summary>
	public static AsyncRetryPolicy GetDatabaseRetryPolicy()
	{
		return Policy
			.Handle<Exception>(ex =>
			{
				// Adicione aqui exceções específicas do banco que podem ser retentadas
				var message = ex.Message.ToLower();
				return message.Contains("timeout") ||
					   message.Contains("deadlock") ||
					   message.Contains("connection") ||
					   message.Contains("network");
			})
			.WaitAndRetryAsync(
				retryCount: 3,
				sleepDurationProvider: retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(2, retryAttempt) * 100),
				onRetry: (exception, timespan, retryCount, context) =>
				{
					_logger.Warning(
						exception,
						"Retry de banco de dados - Tentativa {RetryCount} após {Delay}ms",
						retryCount,
						timespan.TotalMilliseconds);
				});
	}

	/// <summary>
	/// Política de retry com fallback
	/// </summary>
	public static IAsyncPolicy<T> GetRetryWithFallbackPolicy<T>(T fallbackValue)
	{
		var retryPolicy = Policy<T>
			.Handle<Exception>()
			.WaitAndRetryAsync(
				retryCount: 2,
				sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(retryAttempt));

		var fallbackPolicy = Policy<T>
			.Handle<Exception>()
			.FallbackAsync(
				fallbackValue,
				onFallbackAsync: (result, context) =>
				{
					_logger.Warning(
						result.Exception,
						"Executando fallback devido a: {Exception}",
						result.Exception?.Message);
					return Task.CompletedTask;
				});

		return Policy.WrapAsync(fallbackPolicy, retryPolicy);
	}

	/// <summary>
	/// Política de Bulkhead para limitar concorrência
	/// </summary>
	public static IAsyncPolicy GetBulkheadPolicy(int maxParallelization = 10, int maxQueuingActions = 20)
	{
		return Policy
			.BulkheadAsync(
				maxParallelization,
				maxQueuingActions,
				onBulkheadRejectedAsync: context =>
				{
					_logger.Warning("Bulkhead rejeitou a execução - limite de concorrência atingido");
					return Task.CompletedTask;
				});
	}
}

/// <summary>
/// Extensões para configurar Polly nos HttpClients
/// </summary>
public static class PollyHttpClientExtensions
{
	/// <summary>
	/// Adiciona políticas de resiliência ao HttpClient
	/// </summary>
	public static IHttpClientBuilder AddPollyPolicies(this IHttpClientBuilder builder)
	{
		return builder
			.AddPolicyHandler(PollyPolicies.GetHttpRetryPolicy())
			.AddPolicyHandler(PollyPolicies.GetHttpCircuitBreakerPolicy())
			.AddPolicyHandler(PollyPolicies.GetHttpTimeoutPolicy());
	}

	/// <summary>
	/// Adiciona políticas combinadas de resiliência ao HttpClient
	/// </summary>
	public static IHttpClientBuilder AddCombinedPollyPolicies(this IHttpClientBuilder builder)
	{
		return builder.AddPolicyHandler(PollyPolicies.GetCombinedHttpPolicy());
	}
}

