using Jazz.EntityFramework;
using CanadaSoftware.ApiDotNet.Domain;
using CanadaSoftware.ApiDotNet.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace CanadaSoftware.ApiDotNet.Application.Data;

/// <summary>
/// Implementação do repositório de Cliente
/// </summary>
public class ClienteRepository : EntityFrameWorkRepository<Guid, Cliente, ProposalsDbContext>, IClienteRepository
{
	private readonly ProposalsDbContext _context;

	public ClienteRepository(ProposalsDbContext dbContext) : base(dbContext)
	{
		_context = dbContext;
	}

	public async Task<Cliente?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default)
	{
		return await _context.Clientes
			.FirstOrDefaultAsync(x => x.Cpf.Value == cpf, cancellationToken);
	}

	public async Task<Cliente?> GetByClienteIdAsync(string clienteId, CancellationToken cancellationToken = default)
	{
		return await _context.Clientes
			.FirstOrDefaultAsync(x => x.ClienteId == clienteId, cancellationToken);
	}

	public async Task<bool> ExistsByCpfAsync(string cpf, CancellationToken cancellationToken = default)
	{
		return await _context.Clientes
			.AnyAsync(x => x.Cpf.Value == cpf, cancellationToken);
	}

	public async Task<List<Cliente>> ListActivesAsync(CancellationToken cancellationToken = default)
	{
		return await _context.Clientes
			.Where(x => x.Ativo)
			.OrderBy(x => x.Nome)
			.ToListAsync(cancellationToken);
	}
}

