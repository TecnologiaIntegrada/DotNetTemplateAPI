using CanadaSoftware.ApiDotNet.Domain;
using CanadaSoftware.ApiDotNet.EntityFramework.Mappings;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CanadaSoftware.ApiDotNet.EntityFramework;

public class AppDbContext : DbContext
{
	public DbSet<Cliente> Clientes => Set<Cliente>();

	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.ApplyConfiguration(new ClienteMap());
	}

	protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
	{
		var assembly = GetType().Assembly;
	}
}

