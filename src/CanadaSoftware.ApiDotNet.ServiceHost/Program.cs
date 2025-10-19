using System.Text.Json;
using System.Text.Json.Serialization;
using CanadaSoftware.ApiDotNet.Application.Configuration;
using CanadaSoftware.ApiDotNet.Application.RestApi;
using CanadaSoftware.ApiDotNet.EntityFramework;
using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Enrichers.Sensitive;
using Serilog.Enrichers.Span;
using Serilog.Formatting.Json;

try
{
	var builder = WebApplication.CreateBuilder(args);
	var config = builder.Configuration.AddEnvironmentVariables().Build();

	// Configurar Serilog
	builder.Host.UseSerilog((_, lc) => lc
		.ReadFrom.Configuration(builder.Configuration)
		.WriteTo.Console(new JsonFormatter(renderMessage: true))
		.WriteTo.Seq(config["Seq:ServerUrl"] ?? "http://seq-service.dev.svc.cluster.local:5341")
		.Enrich.FromLogContext()
		.Enrich.WithSpan()
		.Enrich.WithSensitiveDataMasking(options =>
		{
			options.MaskValue = "***MASKED***";
			options.MaskProperties.Add("Password");
			options.MaskProperties.Add("Cpf");
			options.MaskProperties.Add("Account");
			options.MaskProperties.Add("AccountNumber");
		}));

	// Configurar Serviços
	builder.Services
		.AddControllers();

	builder.Services
		.AddEndpointsApiExplorer()
		.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
			{
				Title = "Canada Software API",
				Version = "v1.0.0",
				Description = "API para gestão de clientes e operações bancárias"
			});
			c.EnableAnnotations();
		})
		.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true))
		.AddDbContext<AppDbContext>(options => options.UseNpgsql(
			config.GetConnectionString("Default") ?? 
			"Host=postgresql-service.dev.svc.cluster.local;Port=5432;Database=appdatabase;Username=postgres;Password=postgres",
			b => b.MigrationsAssembly(typeof(Program).Assembly.FullName)))
		.AddApplicationServices(config)
		.AddHealthChecks();

	// OpenTelemetry
	builder.Services.AddOpenTelemetry()
		.ConfigureResource(b => b.AddService("CanadaSoftware.ApiDotNet", "1.0.0"))
		.WithTracing(b =>
		{
			b.AddSource("CanadaSoftware.ApiDotNet")
				.AddHttpClientInstrumentation()
				.AddAspNetCoreInstrumentation()
#if (DEBUG)
				.AddOtlpExporter()
#endif
				.AddNpgsql();
		});

	// CAP (Kafka)
	builder.Services.AddCap(cap =>
	{
		cap.FailedRetryCount = 5;
		cap.DefaultGroupName = "api-service.grp";
		cap.UseEntityFramework<AppDbContext>();
		cap.UseKafka(o =>
		{
			o.Servers = config.GetConnectionString("Kafka") ?? "localhost:9092";
#if (!DEBUG)
			// Configuração de autenticação para produção
			if (!string.IsNullOrEmpty(config["Kafka:ClusterApiKey"]))
			{
				o.MainConfig.Add("security.protocol", "SASL_SSL");
				o.MainConfig.Add("sasl.mechanisms", "PLAIN");
				o.MainConfig.Add("sasl.username", config["Kafka:ClusterApiKey"]);
				o.MainConfig.Add("sasl.password", config["Kafka:ClusterApiSecret"]);
			}
#endif
		});
		cap.UseDashboard();
	});

	builder.Services.ConfigureHttpJsonOptions(opt =>
	{
		opt.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
	});

	var app = builder.Build();

	// Configurar Pipeline HTTP
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "Canada Software API v1");
		c.RoutePrefix = "swagger";
	});

	app.MapEndpoints();
	app.MapHealthChecks("/healthz");

	// Aplicar migrations automaticamente
	using (var scope = app.Services.CreateScope())
	{
		var services = scope.ServiceProvider;
		var context = services.GetRequiredService<AppDbContext>();
		context.Database.Migrate();
	}

	await app.RunAsync();
}
finally
{
	await Log.CloseAndFlushAsync();
}

