using CanadaSoftware.ApiDotNet.Domain;

namespace CanadaSoftware.ApiDotNet.Application.Data;

/// <summary>
/// Interface do repositório de clientes
/// </summary>
public interface IClienteRepository
{
	Task<Cliente?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
	Task<Cliente?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default);
	Task<IEnumerable<Cliente>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<Cliente> AddAsync(Cliente cliente, CancellationToken cancellationToken = default);
	Task UpdateAsync(Cliente cliente, CancellationToken cancellationToken = default);
	Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
