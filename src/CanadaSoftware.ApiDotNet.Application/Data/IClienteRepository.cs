using CanadaSoftware.ApiDotNet.Core;
using CanadaSoftware.ApiDotNet.Domain;

namespace CanadaSoftware.ApiDotNet.Application.Data;

/// <summary>
/// Interface do repositório de Cliente
/// </summary>
public interface IClienteRepository : IRepository<Guid, Cliente>
{
	/// <summary>
	/// Busca um cliente pelo CPF
	/// </summary>
	Task<Cliente?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default);
	
	/// <summary>
	/// Busca um cliente pelo ClienteId externo
	/// </summary>
	Task<Cliente?> GetByClienteIdAsync(string clienteId, CancellationToken cancellationToken = default);
	
	/// <summary>
	/// Verifica se existe um cliente com o CPF informado
	/// </summary>
	Task<bool> ExistsByCpfAsync(string cpf, CancellationToken cancellationToken = default);
	
	/// <summary>
	/// Lista clientes ativos
	/// </summary>
	Task<List<Cliente>> ListActivesAsync(CancellationToken cancellationToken = default);
}

