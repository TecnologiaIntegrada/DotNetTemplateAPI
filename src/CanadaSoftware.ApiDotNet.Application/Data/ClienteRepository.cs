using CanadaSoftware.ApiDotNet.Domain;
using CanadaSoftware.ApiDotNet.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace CanadaSoftware.ApiDotNet.Application.Data;

/// <summary>
/// Repositório de clientes
/// </summary>
public class ClienteRepository : IClienteRepository
{
	private readonly AppDbContext _context;

	public ClienteRepository(AppDbContext context)
	{
		_context = context;
	}

	public async Task<Cliente?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
	{
		return await _context.Clientes
			.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
	}

	public async Task<Cliente?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default)
	{
		return await _context.Clientes
			.FirstOrDefaultAsync(x => x.Cpf.Value == cpf, cancellationToken);
	}

	public async Task<IEnumerable<Cliente>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return await _context.Clientes
			.Where(x => x.Ativo)
			.ToListAsync(cancellationToken);
	}

	public async Task<Cliente> AddAsync(Cliente cliente, CancellationToken cancellationToken = default)
	{
		await _context.Clientes.AddAsync(cliente, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);
		return cliente;
	}

	public async Task UpdateAsync(Cliente cliente, CancellationToken cancellationToken = default)
	{
		_context.Clientes.Update(cliente);
		await _context.SaveChangesAsync(cancellationToken);
	}

	public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
	{
		var cliente = await GetByIdAsync(id, cancellationToken);
		if (cliente != null)
		{
			cliente.Desativar();
			await UpdateAsync(cliente, cancellationToken);
		}
	}
}
