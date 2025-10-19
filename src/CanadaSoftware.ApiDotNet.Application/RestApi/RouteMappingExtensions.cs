using System.Net;
using Jazz.Application;
using CanadaSoftware.ApiDotNet.Application.RequestHandlers.Cliente;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CanadaSoftware.ApiDotNet.Application.RestApi;

/// <summary>
/// Extensões de mapeamento de rotas da API
/// </summary>
public static class RouteMappingExtensions
{
	private const string CLIENTES_ROUTE = "/v1/clientes";

	/// <summary>
	/// Mapeia todos os endpoints da API
	/// </summary>
	public static WebApplication MapEndpoints(this WebApplication app)
	{
		return app
			.MapClienteEndpoints();
	}

	/// <summary>
	/// Mapeia os endpoints de clientes
	/// </summary>
	public static WebApplication MapClienteEndpoints(this WebApplication app)
	{
		return app
			.MapCreateCliente()
			.MapGetClienteById()
			.MapGetClienteByCpf()
			.MapUpdateCliente();
	}

	/// <summary>
	/// Endpoint para criação de cliente
	/// </summary>
	private static WebApplication MapCreateCliente(this WebApplication app)
	{
		app.MapPost(CLIENTES_ROUTE,
			async (
				[FromHeader(Name = "User-Id")] string? userId,
				[FromBody] CreateClienteCommand cmd,
				[FromServices] IMediator mediator) =>
			{
				cmd.UsuarioCadastro = userId;

				var response = await mediator.Send(cmd);

				return response switch
				{
					CreateClienteSuccess success => Results.Created(
						$"{CLIENTES_ROUTE}/{success.Cliente.Id}",
						new
						{
							success.Cliente.Id,
							success.Cliente.ClienteId,
							success.Cliente.Cpf,
							success.Cliente.NomeCompleto,
							success.Cliente.Email,
							success.Cliente.Celular,
							success.Cliente.DataCadastro
						}),

					CreateClienteBadRequest badRequest => Results.Problem(
						detail: badRequest.ProblemDetails.Detail,
						instance: badRequest.ProblemDetails.Instance,
						statusCode: badRequest.ProblemDetails.Status ?? (int)HttpStatusCode.BadRequest,
						title: badRequest.ProblemDetails.Title,
						extensions: new Dictionary<string, object?>
						{
							{ "errorCode", badRequest.ProblemDetails.ErrorCode },
							{ "timestamp", DateTime.UtcNow }
						}),

					CreateClienteFail failed => Results.Problem(
						detail: failed.AppProblemDetails.Detail,
						statusCode: (int)HttpStatusCode.InternalServerError,
						title: "Erro ao criar cliente"),

					_ => throw new ArgumentOutOfRangeException(nameof(response))
				};
			})
			.WithMetadata(new SwaggerOperationAttribute
			{
				Summary = "Criar novo cliente",
				Description = @"
					<p>Cria um novo cliente no sistema com todas as informações necessárias.</p>
					<h4>Dados obrigatórios:</h4>
					<ul>
						<li><strong>ClienteId</strong>: Identificador externo do cliente</li>
						<li><strong>CPF</strong>: CPF do cliente (formato: 00000000000)</li>
						<li><strong>Nome</strong>: Nome do cliente</li>
						<li><strong>Sobrenome</strong>: Sobrenome do cliente</li>
						<li><strong>Celular</strong>: Telefone celular (formato: 00000000000)</li>
						<li><strong>Sexo</strong>: M (Masculino) ou F (Feminino)</li>
						<li><strong>DataNascimento</strong>: Data de nascimento (formato: YYYY-MM-DD)</li>
						<li><strong>Dependentes</strong>: Número de dependentes (>= 0)</li>
					</ul>
					<h4>Dados opcionais:</h4>
					<ul>
						<li><strong>Email</strong>: Email do cliente</li>
						<li><strong>Naturalidade</strong>: Cidade de nascimento</li>
						<li><strong>Nacionalidade</strong>: N (Nacional) ou E (Estrangeiro)</li>
						<li><strong>NomeMae</strong>: Nome da mãe</li>
						<li><strong>NomePai</strong>: Nome do pai</li>
						<li><strong>Profissao</strong>: Profissão do cliente</li>
						<li><strong>EstadoCivil</strong>: Estado civil</li>
						<li><strong>RendaMensal</strong>: Renda mensal em reais</li>
						<li><strong>Endereco</strong>: Dados do endereço</li>
						<li><strong>Documento</strong>: Dados do documento de identidade</li>
						<li><strong>DadosBancarios</strong>: Dados bancários</li>
					</ul>"
			})
			.Produces(StatusCodes.Status201Created)
			.Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails))
			.Produces(StatusCodes.Status500InternalServerError, typeof(ProblemDetails))
			.WithTags("Clientes");

		return app;
	}

	/// <summary>
	/// Endpoint para buscar cliente por ID
	/// </summary>
	private static WebApplication MapGetClienteById(this WebApplication app)
	{
		app.MapGet($"{CLIENTES_ROUTE}/{{id}}",
			async ([FromRoute] Guid id, [FromServices] IMediator mediator) =>
			{
				var query = new GetClienteQuery(id);
				var response = await mediator.Send(query);

				return response switch
				{
					GetClienteSuccess success => Results.Ok(success.Cliente),

					GetClienteNotFound => Results.NotFound(new ProblemDetails
					{
						Status = (int)HttpStatusCode.NotFound,
						Title = "Cliente não encontrado",
						Detail = $"Não foi encontrado cliente com o ID {id}",
						Instance = $"{CLIENTES_ROUTE}/{id}"
					}),

					GetClienteFail failed => Results.Problem(
						detail: failed.AppProblemDetails.Detail,
						statusCode: (int)HttpStatusCode.InternalServerError,
						title: "Erro ao buscar cliente"),

					_ => throw new ArgumentOutOfRangeException(nameof(response))
				};
			})
			.WithMetadata(new SwaggerOperationAttribute
			{
				Summary = "Buscar cliente por ID",
				Description = "Retorna os dados completos de um cliente pelo seu identificador único."
			})
			.Produces(StatusCodes.Status200OK, typeof(Domain.Cliente))
			.Produces(StatusCodes.Status404NotFound, typeof(ProblemDetails))
			.Produces(StatusCodes.Status500InternalServerError, typeof(ProblemDetails))
			.WithTags("Clientes");

		return app;
	}

	/// <summary>
	/// Endpoint para buscar cliente por CPF
	/// </summary>
	private static WebApplication MapGetClienteByCpf(this WebApplication app)
	{
		app.MapGet($"{CLIENTES_ROUTE}/cpf/{{cpf}}",
			async ([FromRoute] string cpf, [FromServices] IMediator mediator) =>
			{
				var query = new GetClienteByCpfQuery(cpf);
				var response = await mediator.Send(query);

				return response switch
				{
					GetClienteSuccess success => Results.Ok(success.Cliente),

					GetClienteNotFound => Results.NotFound(new ProblemDetails
					{
						Status = (int)HttpStatusCode.NotFound,
						Title = "Cliente não encontrado",
						Detail = $"Não foi encontrado cliente com o CPF {cpf}",
						Instance = $"{CLIENTES_ROUTE}/cpf/{cpf}"
					}),

					GetClienteFail failed => Results.Problem(
						detail: failed.AppProblemDetails.Detail,
						statusCode: (int)HttpStatusCode.InternalServerError,
						title: "Erro ao buscar cliente"),

					_ => throw new ArgumentOutOfRangeException(nameof(response))
				};
			})
			.WithMetadata(new SwaggerOperationAttribute
			{
				Summary = "Buscar cliente por CPF",
				Description = "Retorna os dados completos de um cliente pelo seu CPF."
			})
			.Produces(StatusCodes.Status200OK, typeof(Domain.Cliente))
			.Produces(StatusCodes.Status404NotFound, typeof(ProblemDetails))
			.Produces(StatusCodes.Status500InternalServerError, typeof(ProblemDetails))
			.WithTags("Clientes");

		return app;
	}

	/// <summary>
	/// Endpoint para atualização de cliente
	/// </summary>
	private static WebApplication MapUpdateCliente(this WebApplication app)
	{
		app.MapPut($"{CLIENTES_ROUTE}/{{id}}",
			async (
				[FromRoute] Guid id,
				[FromHeader(Name = "User-Id")] string? userId,
				[FromBody] UpdateClienteCommand cmd,
				[FromServices] IMediator mediator) =>
			{
				// Sobrescrever o ID da rota
				cmd = cmd with { Id = id, UsuarioAtualizacao = userId };

				var response = await mediator.Send(cmd);

				return response switch
				{
					UpdateClienteSuccess success => Results.Ok(new
					{
						success.Cliente.Id,
						success.Cliente.ClienteId,
						success.Cliente.Cpf,
						success.Cliente.NomeCompleto,
						success.Cliente.Email,
						success.Cliente.Celular,
						success.Cliente.DataAtualizacao
					}),

					UpdateClienteNotFound => Results.NotFound(new ProblemDetails
					{
						Status = (int)HttpStatusCode.NotFound,
						Title = "Cliente não encontrado",
						Detail = $"Não foi encontrado cliente com o ID {id}",
						Instance = $"{CLIENTES_ROUTE}/{id}"
					}),

					UpdateClienteBadRequest badRequest => Results.Problem(
						detail: badRequest.ProblemDetails.Detail,
						instance: badRequest.ProblemDetails.Instance,
						statusCode: badRequest.ProblemDetails.Status ?? (int)HttpStatusCode.BadRequest,
						title: badRequest.ProblemDetails.Title,
						extensions: new Dictionary<string, object?>
						{
							{ "errorCode", badRequest.ProblemDetails.ErrorCode },
							{ "timestamp", DateTime.UtcNow }
						}),

					UpdateClienteFail failed => Results.Problem(
						detail: failed.AppProblemDetails.Detail,
						statusCode: (int)HttpStatusCode.InternalServerError,
						title: "Erro ao atualizar cliente"),

					_ => throw new ArgumentOutOfRangeException(nameof(response))
				};
			})
			.WithMetadata(new SwaggerOperationAttribute
			{
				Summary = "Atualizar cliente",
				Description = @"
					<p>Atualiza os dados de um cliente existente.</p>
					<p>Todos os campos são opcionais. Apenas os campos informados serão atualizados.</p>"
			})
			.Produces(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails))
			.Produces(StatusCodes.Status404NotFound, typeof(ProblemDetails))
			.Produces(StatusCodes.Status500InternalServerError, typeof(ProblemDetails))
			.WithTags("Clientes");

		return app;
	}
}
