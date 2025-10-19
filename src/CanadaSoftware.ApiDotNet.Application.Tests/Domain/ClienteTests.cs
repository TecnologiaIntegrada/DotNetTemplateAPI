using CanadaSoftware.ApiDotNet.Domain;
using FluentAssertions;
using FluentValidation;
using CanadaSoftware.ApiDotNet.Commom;
using CanadaSoftware.ApiDotNet.Commom.ClientProperties;
using Xunit;

namespace CanadaSoftware.ApiDotNet.Tests.Domain;

/// <summary>
/// Testes unitários para a entidade Cliente
/// </summary>
public class ClienteTests
{
	[Fact]
	public void Criar_ComDadosValidos_DeveCriarCliente()
	{
		// Arrange
		var clienteId = "CLI123456";
		var cpf = Cpf.From("12345678900");
		var nome = Name.From("João");
		var sobrenome = Name.From("Silva");
		var celular = Cellphone.From("11999999999");
		var sexo = Sex.From("M");
		var dataNascimento = BirthDate.From(new DateTime(1990, 1, 1));

		// Act
		var cliente = Cliente.Criar(
			clienteId,
			cpf,
			nome,
			sobrenome,
			null, // email
			celular,
			sexo,
			0, // dependentes
			dataNascimento,
			null, // naturalidade
			null, // nacionalidade
			null, // nomeMae
			null, // nomePai
			null, // profissao
			null, // estadoCivil
			null, // rendaMensal
			null, // nomeConjuge
			null, // cpfConjuge
			"admin@teste.com" // usuarioCadastro
		);

		// Assert
		cliente.Should().NotBeNull();
		cliente.Id.Should().NotBeEmpty();
		cliente.ClienteId.Should().Be(clienteId);
		cliente.Cpf.Should().Be(cpf);
		cliente.Nome.Should().Be(nome);
		cliente.Sobrenome.Should().Be(sobrenome);
		cliente.NomeCompleto.Should().Be("João Silva");
		cliente.Ativo.Should().BeTrue();
		cliente.DataCadastro.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
	}

	[Fact]
	public void Criar_ComCpfInvalido_DeveLancarValidationException()
	{
		// Arrange
		var act = () => Cliente.Criar(
			"CLI123",
			Cpf.From("00000000000"), // CPF inválido
			Name.From("João"),
			Name.From("Silva"),
			null,
			Cellphone.From("11999999999"),
			Sex.From("M"),
			0,
			BirthDate.From(new DateTime(1990, 1, 1)),
			null, null, null, null, null, null, null, null, null, null
		);

		// Act & Assert
		act.Should().Throw<ValidationException>();
	}

	[Fact]
	public void AdicionarEndereco_DeveAtualizarEndereco()
	{
		// Arrange
		var cliente = CriarClienteValido();
		var cep = Jazz.Commom.AddressProperties.PostalCode.From("01310100");
		var logradouro = Jazz.Commom.AddressProperties.Street.From("Avenida Paulista");
		var bairro = Jazz.Commom.AddressProperties.District.From("Bela Vista");
		var estado = Jazz.Commom.AddressProperties.State.From("SP");
		var cidade = Jazz.Commom.AddressProperties.City.From("São Paulo");
		var pais = Jazz.Commom.AddressProperties.Country.From("Brasil");

		// Act
		cliente.AdicionarEndereco(
			cep, logradouro, "1000", "Apto 101",
			bairro, estado, cidade, pais, 12
		);

		// Assert
		cliente.Endereco.Should().NotBeNull();
		cliente.Endereco!.PostalCode.Should().Be(cep);
		cliente.Endereco.Street.Should().Be(logradouro);
		cliente.Endereco.Number.Should().Be("1000");
		cliente.DataAtualizacao.Should().NotBeNull();
	}

	[Fact]
	public void AdicionarDocumento_DeveAtualizarDocumento()
	{
		// Arrange
		var cliente = CriarClienteValido();
		var tipo = Jazz.Commom.DocumentProperties.Type.From("RG");
		var numero = Jazz.Common.DocumentProperties.DocumentNumber.From("123456789");
		var dataEmissao = Jazz.Common.DocumentProperties.IssuerDate.From(new DateTime(2015, 5, 10));
		var orgaoEmissor = Jazz.Common.DocumentProperties.Issuer.From("SSP");
		var uf = Jazz.Common.DocumentProperties.FederativeUnity.From("SP");

		// Act
		cliente.AdicionarDocumento(tipo, numero, dataEmissao, orgaoEmissor, uf);

		// Assert
		cliente.Documento.Should().NotBeNull();
		cliente.Documento!.Type.Should().Be(tipo);
		cliente.Documento.Number.Should().Be(numero);
		cliente.DataAtualizacao.Should().NotBeNull();
	}

	[Fact]
	public void AdicionarDadosBancarios_DeveAtualizarDadosBancarios()
	{
		// Arrange
		var cliente = CriarClienteValido();
		var banco = Jazz.Commom.BankDataProperties.Bank.From("001");
		var agencia = Jazz.Commom.BankDataProperties.Agency.From("1234");
		var agenciaDac = Jazz.Commom.BankDataProperties.AgencyDac.From("5");
		var conta = Jazz.Commom.BankDataProperties.Account.From("123456");
		var contaDac = Jazz.Commom.BankDataProperties.AccountDac.From("7");
		var tipoConta = Jazz.Commom.BankDataProperties.AccountType.From("C");

		// Act
		cliente.AdicionarDadosBancarios(banco, agencia, agenciaDac, conta, contaDac, tipoConta);

		// Assert
		cliente.DadosBancarios.Should().NotBeNull();
		cliente.DadosBancarios!.Bank.Should().Be(banco);
		cliente.DadosBancarios.Account.Should().Be(conta);
		cliente.DataAtualizacao.Should().NotBeNull();
	}

	[Fact]
	public void AtualizarInformacoes_DeveAtualizarDadosBasicos()
	{
		// Arrange
		var cliente = CriarClienteValido();
		var novoNome = Name.From("Pedro");
		var novoSobrenome = Name.From("Santos");
		var novoEmail = Email.From("pedro@exemplo.com");

		// Act
		cliente.AtualizarInformacoes(
			novoNome, novoSobrenome, novoEmail, null, "Engenheiro", 5000m, "admin_update"
		);

		// Assert
		cliente.Nome.Should().Be(novoNome);
		cliente.Sobrenome.Should().Be(novoSobrenome);
		cliente.Email.Should().Be(novoEmail);
		cliente.Profissao.Should().Be("Engenheiro");
		cliente.RendaMensal.Should().Be(5000m);
		cliente.UsuarioAtualizacao.Should().Be("admin_update");
		cliente.DataAtualizacao.Should().NotBeNull();
	}

	[Fact]
	public void Desativar_DeveMarcarClienteComoInativo()
	{
		// Arrange
		var cliente = CriarClienteValido();

		// Act
		cliente.Desativar("admin");

		// Assert
		cliente.Ativo.Should().BeFalse();
		cliente.UsuarioAtualizacao.Should().Be("admin");
		cliente.DataAtualizacao.Should().NotBeNull();
	}

	[Fact]
	public void Reativar_DeveMarcarClienteComoAtivo()
	{
		// Arrange
		var cliente = CriarClienteValido();
		cliente.Desativar("admin");

		// Act
		cliente.Reativar("admin");

		// Assert
		cliente.Ativo.Should().BeTrue();
		cliente.UsuarioAtualizacao.Should().Be("admin");
		cliente.DataAtualizacao.Should().NotBeNull();
	}

	[Fact]
	public void AtualizarDadosConjuge_DeveAtualizarInformacoesDoConjuge()
	{
		// Arrange
		var cliente = CriarClienteValido();

		// Act
		cliente.AtualizarDadosConjuge("Maria Silva", "98765432100");

		// Assert
		cliente.NomeConjuge.Should().Be("Maria Silva");
		cliente.CpfConjuge.Should().Be("98765432100");
		cliente.DataAtualizacao.Should().NotBeNull();
	}

	// Helper method
	private Cliente CriarClienteValido()
	{
		return Cliente.Criar(
			"CLI123456",
			Cpf.From("12345678900"),
			Name.From("João"),
			Name.From("Silva"),
			null,
			Cellphone.From("11999999999"),
			Sex.From("M"),
			0,
			BirthDate.From(new DateTime(1990, 1, 1)),
			null, null, null, null, null, null, null, null, null, null
		);
	}
}

