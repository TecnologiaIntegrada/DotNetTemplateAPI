using CanadaSoftware.ApiDotNet.Application.Data;
using CanadaSoftware.ApiDotNet.Application.MessageProducer;
using CanadaSoftware.ApiDotNet.Application.RequestHandlers.Cliente;
using CanadaSoftware.ApiDotNet.Domain;
using FluentAssertions;
using CanadaSoftware.ApiDotNet.Commom;
using CanadaSoftware.ApiDotNet.Commom.ClientProperties;
using CanadaSoftware.ApiDotNet.Core;
using Moq;
using Xunit;

namespace CanadaSoftware.ApiDotNet.Tests.Application;

/// <summary>
/// Testes unitários para CreateClienteHandler
/// </summary>
public class CreateClienteHandlerTests
{
	private readonly Mock<IClienteRepository> _repositoryMock;
	private readonly Mock<IUnitOfWork> _uowMock;
	private readonly Mock<IClienteEventProducer> _eventProducerMock;
	private readonly CreateClienteHandler _handler;

	public CreateClienteHandlerTests()
	{
		_repositoryMock = new Mock<IClienteRepository>();
		_uowMock = new Mock<IUnitOfWork>();
		_eventProducerMock = new Mock<IClienteEventProducer>();
		_handler = new CreateClienteHandler(_repositoryMock.Object, _uowMock.Object, _eventProducerMock.Object);
	}

	[Fact]
	public async Task Handle_ComDadosValidos_DeveCriarClienteComSucesso()
	{
		// Arrange
		var command = new CreateClienteCommand
		{
			ClienteId = "CLI123",
			Cpf = Cpf.From("12345678900"),
			Nome = Name.From("João"),
			Sobrenome = Name.From("Silva"),
			Celular = Cellphone.From("11999999999"),
			Sexo = Sex.From("M"),
			DataNascimento = BirthDate.From(new DateTime(1990, 1, 1)),
			Dependentes = 0,
			UsuarioCadastro = "admin@teste.com"
		};

		_repositoryMock
			.Setup(x => x.ExistsByCpfAsync(command.Cpf.Value, It.IsAny<CancellationToken>()))
			.ReturnsAsync(false);

		_repositoryMock
			.Setup(x => x.SaveAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		_uowMock
			.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		_eventProducerMock
			.Setup(x => x.PublishClienteCriadoAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.Should().BeOfType<CreateClienteSuccess>();
		var successResult = result as CreateClienteSuccess;
		successResult!.Cliente.Should().NotBeNull();
		successResult.Cliente.ClienteId.Should().Be(command.ClienteId);
		successResult.Cliente.Cpf.Should().Be(command.Cpf);
		successResult.Cliente.Nome.Should().Be(command.Nome);

		_repositoryMock.Verify(x => x.ExistsByCpfAsync(command.Cpf.Value, It.IsAny<CancellationToken>()), Times.Once);
		_repositoryMock.Verify(x => x.SaveAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()), Times.Once);
		_uowMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
		_eventProducerMock.Verify(x => x.PublishClienteCriadoAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task Handle_ComCpfJaExistente_DeveRetornarBadRequest()
	{
		// Arrange
		var command = new CreateClienteCommand
		{
			ClienteId = "CLI123",
			Cpf = Cpf.From("12345678900"),
			Nome = Name.From("João"),
			Sobrenome = Name.From("Silva"),
			Celular = Cellphone.From("11999999999"),
			Sexo = Sex.From("M"),
			DataNascimento = BirthDate.From(new DateTime(1990, 1, 1)),
			Dependentes = 0
		};

		_repositoryMock
			.Setup(x => x.ExistsByCpfAsync(command.Cpf.Value, It.IsAny<CancellationToken>()))
			.ReturnsAsync(true);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.Should().BeOfType<CreateClienteBadRequest>();
		var badRequestResult = result as CreateClienteBadRequest;
		badRequestResult!.ProblemDetails.Title.Should().Be("Cliente já cadastrado");

		_repositoryMock.Verify(x => x.ExistsByCpfAsync(command.Cpf.Value, It.IsAny<CancellationToken>()), Times.Once);
		_repositoryMock.Verify(x => x.SaveAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()), Times.Never);
		_uowMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
		_eventProducerMock.Verify(x => x.PublishClienteCriadoAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	[Fact]
	public async Task Handle_ComEnderecoEDocumento_DeveCriarClienteCompleto()
	{
		// Arrange
		var command = new CreateClienteCommand
		{
			ClienteId = "CLI123",
			Cpf = Cpf.From("12345678900"),
			Nome = Name.From("João"),
			Sobrenome = Name.From("Silva"),
			Celular = Cellphone.From("11999999999"),
			Sexo = Sex.From("M"),
			DataNascimento = BirthDate.From(new DateTime(1990, 1, 1)),
			Dependentes = 0,
			Endereco = new EnderecoDto
			{
				Cep = Jazz.Commom.AddressProperties.PostalCode.From("01310100"),
				Logradouro = Jazz.Commom.AddressProperties.Street.From("Av Paulista"),
				Numero = "1000",
				Bairro = Jazz.Commom.AddressProperties.District.From("Bela Vista"),
				Estado = Jazz.Commom.AddressProperties.State.From("SP"),
				Cidade = Jazz.Commom.AddressProperties.City.From("São Paulo"),
				Pais = Jazz.Commom.AddressProperties.Country.From("Brasil")
			},
			Documento = new DocumentoDto
			{
				Tipo = Jazz.Commom.DocumentProperties.Type.From("RG"),
				Numero = Jazz.Common.DocumentProperties.DocumentNumber.From("123456789"),
				DataEmissao = Jazz.Common.DocumentProperties.IssuerDate.From(new DateTime(2015, 1, 1)),
				OrgaoEmissor = Jazz.Common.DocumentProperties.Issuer.From("SSP"),
				Uf = Jazz.Common.DocumentProperties.FederativeUnity.From("SP")
			},
			DadosBancarios = new DadosBancariosDto
			{
				Banco = Jazz.Commom.BankDataProperties.Bank.From("001"),
				Agencia = Jazz.Commom.BankDataProperties.Agency.From("1234"),
				AgenciaDac = Jazz.Commom.BankDataProperties.AgencyDac.From("5"),
				Conta = Jazz.Commom.BankDataProperties.Account.From("123456"),
				ContaDac = Jazz.Commom.BankDataProperties.AccountDac.From("7"),
				TipoConta = Jazz.Commom.BankDataProperties.AccountType.From("C")
			}
		};

		_repositoryMock
			.Setup(x => x.ExistsByCpfAsync(command.Cpf.Value, It.IsAny<CancellationToken>()))
			.ReturnsAsync(false);

		_repositoryMock
			.Setup(x => x.SaveAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		_uowMock
			.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		_eventProducerMock
			.Setup(x => x.PublishClienteCriadoAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.Should().BeOfType<CreateClienteSuccess>();
		var successResult = result as CreateClienteSuccess;
		successResult!.Cliente.Endereco.Should().NotBeNull();
		successResult.Cliente.Documento.Should().NotBeNull();
		successResult.Cliente.DadosBancarios.Should().NotBeNull();

		_repositoryMock.Verify(x => x.SaveAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()), Times.Once);
		_eventProducerMock.Verify(x => x.PublishClienteCriadoAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()), Times.Once);
	}
}

