using CanadaSoftware.ApiDotNet.Application.Data;
using CanadaSoftware.ApiDotNet.Application.MessageProducer;
using CanadaSoftware.ApiDotNet.Application.RequestHandlers.Cliente;
using CanadaSoftware.ApiDotNet.EntityFramework;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CanadaSoftware.ApiDotNet.Application.Configuration;

public static class DependencyInjection
{
	public static IServiceCollection AddApplicationServices(
		this IServiceCollection services, IConfiguration configuration)
	{
		// MediatR e Validações
		services.TryAddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
		services.AddValidatorsFromAssemblyContaining<CreateClienteCommand>();
		services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateClienteHandler).Assembly));

		// Repositórios
		services.AddScoped<IClienteRepository, ClienteRepository>();

		// Event Producers
		services.AddScoped<IClienteEventProducer, ClienteEventProducer>();

		return services;
	}
}

// ValidationBehavior
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
{
	private readonly IEnumerable<IValidator<TRequest>> _validators;

	public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
	{
		_validators = validators;
	}

	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		if (_validators.Any())
		{
			var context = new ValidationContext<TRequest>(request);
			var validationResults = await Task.WhenAll(
				_validators.Select(v => v.ValidateAsync(context, cancellationToken)));

			var failures = validationResults
				.SelectMany(r => r.Errors)
				.Where(f => f != null)
				.ToList();

			if (failures.Any())
			{
				throw new ValidationException(failures);
			}
		}

		return await next();
	}
}

