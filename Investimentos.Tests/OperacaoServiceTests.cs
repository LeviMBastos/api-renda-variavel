using Investimentos.Domain.Entities;
using Investimentos.Domain.Interfaces;
using Investimentos.Domain.Services;
using Investimentos.Infra.Services;
using Moq;

namespace Investimentos.Tests
{
    public class OperacaoServiceTests
    {
        private readonly Mock<IOperacaoRepository> _operacaoRepositoryMock;
        private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
        private readonly Mock<IPosicaoService> _posicaoServiceMock;
        private readonly OperacaoService _operacaoService;

        public OperacaoServiceTests()
        {
            _operacaoRepositoryMock = new Mock<IOperacaoRepository>();
            _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
            _posicaoServiceMock = new Mock<IPosicaoService>();

            _operacaoService = new OperacaoService(
                _operacaoRepositoryMock.Object,
                _usuarioRepositoryMock.Object,
                _posicaoServiceMock.Object
            );
        }

        [Fact]
        public async Task CalcularPrecoMedioAsync_DeveCalcularCorretamente()
        {
            // Arrange
            int usuarioId = 1;
            int ativoId = 100;

            var operacoes = new List<Operacao>
            {
                new Operacao { Quantidade = 10, PrecoUnitario = 20.0m },
                new Operacao { Quantidade = 5, PrecoUnitario = 30.0m }
            };

            _operacaoRepositoryMock
                .Setup(repo => repo.GetOperacoesDeCompraAsync(usuarioId, ativoId))
                .ReturnsAsync(operacoes);

            // Act
            decimal precoMedio = await _operacaoService.CalcularPrecoMedioAsync(ativoId, usuarioId);

            // Assert
            // (10*20 + 5*30) / (10+5) = (200 + 150) / 15 = 350 / 15 = 23.33...
            Assert.Equal(23.33m, Math.Round(precoMedio, 2));
        }

        [Fact]
        public async Task CalcularPrecoMedioAsync_SemOperacoes_DeveLancarExcecao()
        {
            // Arrange
            int usuarioId = 1;
            int ativoId = 200;

            _operacaoRepositoryMock
                .Setup(repo => repo.GetOperacoesDeCompraAsync(usuarioId, ativoId))
                .ReturnsAsync(new List<Operacao>());

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _operacaoService.CalcularPrecoMedioAsync(ativoId, usuarioId));
        }

        [Fact]
        public async Task CalcularPrecoMedioAsync_QuantidadeTotalZero_DeveRetornarZero()
        {
            // Arrange
            int usuarioId = 1;
            int ativoId = 300;

            var operacoes = new List<Operacao>
            {
                new Operacao { Quantidade = 0, PrecoUnitario = 50.0m }
            };

            _operacaoRepositoryMock
                .Setup(repo => repo.GetOperacoesDeCompraAsync(usuarioId, ativoId))
                .ReturnsAsync(operacoes);

            // Act
            decimal precoMedio = await _operacaoService.CalcularPrecoMedioAsync(ativoId, usuarioId);

            // Assert
            Assert.Equal(0, precoMedio);
        }
    }
}