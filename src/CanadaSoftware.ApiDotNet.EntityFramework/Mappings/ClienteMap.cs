using CanadaSoftware.ApiDotNet.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CanadaSoftware.ApiDotNet.EntityFramework.Mappings;

/// <summary>
/// Configuração do mapeamento EF Core para a entidade Cliente
/// </summary>
public class ClienteMap : IEntityTypeConfiguration<Cliente>
{
	public void Configure(EntityTypeBuilder<Cliente> builder)
	{
		// Tabela
		builder.ToTable("Clientes");

		// Chave primária
		builder.HasKey(x => x.Id);

		// Propriedades básicas
		builder.Property(x => x.Id)
			.IsRequired();

		builder.Property(x => x.ClienteId)
			.HasMaxLength(100)
			.IsRequired();

		builder.HasIndex(x => x.ClienteId)
			.IsUnique();

		builder.HasIndex(x => x.Cpf)
			.IsUnique();

		// Informações pessoais
		builder.Property(x => x.Profissao)
			.HasMaxLength(200);

		builder.Property(x => x.EstadoCivil)
			.HasMaxLength(50);

		builder.Property(x => x.RendaMensal)
			.HasColumnType("decimal(18,2)");

		builder.Property(x => x.NomeConjuge)
			.HasMaxLength(200);

		builder.Property(x => x.CpfConjuge)
			.HasMaxLength(11);

		// Auditoria
		builder.Property(x => x.DataCadastro)
			.IsRequired();

		builder.Property(x => x.DataAtualizacao);

		builder.Property(x => x.UsuarioCadastro)
			.HasMaxLength(200);

		builder.Property(x => x.UsuarioAtualizacao)
			.HasMaxLength(200);

		builder.Property(x => x.Ativo)
			.IsRequired()
			.HasDefaultValue(true);

		// Endereço como owned entity
		builder.OwnsOne(x => x.Endereco, endereco =>
		{
			endereco.Property(e => e.Number)
				.HasColumnName("EnderecoNumero")
				.HasMaxLength(20);

			endereco.Property(e => e.Complement)
				.HasColumnName("EnderecoComplemento")
				.HasMaxLength(200);

			endereco.Property(e => e.ResidenceTime)
				.HasColumnName("TempoResidencia");
		});

		// Documento como owned entity
		builder.OwnsOne(x => x.Documento, documento =>
		{
			// As propriedades do tipo complexo são mapeadas automaticamente
		});

		// Dados Bancários como owned entity
		builder.OwnsOne(x => x.DadosBancarios, dados =>
		{
			// As propriedades do tipo complexo são mapeadas automaticamente
		});
	}
}

