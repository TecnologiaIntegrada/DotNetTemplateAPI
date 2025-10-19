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
/// Testes unitários para UpdateClienteHandler
/// </summary>
public class UpdateClienteHandlerTests
{
	private readonly Mock<IClienteRepository> _repositoryMock;
	private readonly Mock<IUnitOfWork> _uowMock;
	private readonly Mock<IClienteEventProducer> _eventProducerMock;
	private readonly UpdateClienteHandler _handler;

	public UpdateClienteHandlerTests()
	{
		_repositoryMock = new Mock<IClienteRepository>();
		_uowMock = new Mock<IUnitOfWork>();
		_eventProducerMock = new Mock<IClienteEventProducer>();
		_handler = new UpdateClienteHandler(_repositoryMock.Object, _uowMock.Object, _eventProducerMock.Object);
	}

	[Fact]
	public async Task Handle_ComClienteExistente_DeveAtualizarComSucesso()
	{
		// Arrange
		var clienteId = Guid.NewGuid();
		var clienteExistente = Cliente.Criar(
			"CLI123",
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

		var command = new UpdateClienteCommand
		{
			Id = clienteId,
			Nome = Name.From("Pedro"),
			Sobrenome = Name.From("Santos"),
			Email = Email.From("pedro@exemplo.com"),
			UsuarioAtualizacao = "admin"
		};

		_repositoryMock
			.Setup(x => x.LoadAsync(clienteId, It.IsAny<CancellationToken>()))
			.ReturnsAsync(clienteExistente);

		_repositoryMock
			.Setup(x => x.SaveAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		_uowMock
			.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		_eventProducerMock
			.Setup(x => x.PublishClienteAtualizadoAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.Should().BeOfType<UpdateClienteSuccess>();
		var successResult = result as UpdateClienteSuccess;
		successResult!.Cliente.Should().NotBeNull();
		successResult.Cliente.Nome.Should().Be(command.Nome);
		successResult.Cliente.Sobrenome.Should().Be(command.Sobrenome);
		successResult.Cliente.Email.Should().Be(command.Email);
		successResult.Cliente.UsuarioAtualizacao.Should().Be("admin");

		_repositoryMock.Verify(x => x.LoadAsync(clienteId, It.IsAny<CancellationToken>()), Times.Once);
		_repositoryMock.Verify(x => x.SaveAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()), Times.Once);
		_uowMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
		_eventProducerMock.Verify(x => x.PublishClienteAtualizadoAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task Handle_ComClienteInexistente_DeveRetornarNotFound()
	{
		// Arrange
		var clienteId = Guid.NewGuid();
		var command = new UpdateClienteCommand
		{
			Id = clienteId,
			Nome = Name.From("Pedro")
		};

		_repositoryMock
			.Setup(x => x.LoadAsync(clienteId, It.IsAny<CancellationToken>()))
			.ReturnsAsync((Cliente?)null);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.Should().BeOfType<UpdateClienteNotFound>();

		_repositoryMock.Verify(x => x.LoadAsync(clienteId, It.IsAny<CancellationToken>()), Times.Once);
		_repositoryMock.Verify(x => x.SaveAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()), Times.Never);
		_uowMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
		_eventProducerMock.Verify(x => x.PublishClienteAtualizadoAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	[Fact]
	public async Task Handle_AtualizandoEndereco_DeveAtualizarEnderecoEPublicarEvento()
	{
		// Arrange
		var clienteId = Guid.NewGuid();
		var clienteExistente = Cliente.Criar(
			"CLI123",
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

		var command = new UpdateClienteCommand
		{
			Id = clienteId,
			Endereco = new EnderecoDto
			{
				Cep = Jazz.Commom.AddressProperties.PostalCode.From("01310100"),
				Logradouro = Jazz.Commom.AddressProperties.Street.From("Av Paulista"),
				Numero = "2000",
				Bairro = Jazz.Commom.AddressProperties.District.From("Bela Vista"),
				Estado = Jazz.Commom.AddressProperties.State.From("SP"),
				Cidade = Jazz.Commom.AddressProperties.City.From("São Paulo"),
				Pais = Jazz.Commom.AddressProperties.Country.From("Brasil")
			}
		};

		_repositoryMock
			.Setup(x => x.LoadAsync(clienteId, It.IsAny<CancellationToken>()))
			.ReturnsAsync(clienteExistente);

		_repositoryMock
			.Setup(x => x.SaveAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		_uowMock
			.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		_eventProducerMock
			.Setup(x => x.PublishClienteAtualizadoAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.Should().BeOfType<UpdateClienteSuccess>();
		var successResult = result as UpdateClienteSuccess;
		successResult!.Cliente.Endereco.Should().NotBeNull();
		successResult.Cliente.Endereco!.PostalCode.Should().Be(command.Endereco.Cep);

		_eventProducerMock.Verify(x => x.PublishClienteAtualizadoAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()), Times.Once);
	}
}

