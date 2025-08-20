using Microsoft.EntityFrameworkCore;
using Moq;
using ProvaPub.Interfaces;
using ProvaPub.Models;
using ProvaPub.Repository;
using ProvaPub.Services;
using Xunit;

namespace ProvaPub.Tests
{
    public class CustomerServiceTests
    {
        // Método auxiliar para criar um DbContext em memória isolado para cada teste
        private TestDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
             .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // banco exclusivo
             .Options;

            var context = new TestDbContext(options);
            // Garante que o banco está limpo antes de cada teste.
            context.Database.EnsureDeleted();
            // Garante que o esquema do banco de dados é criado para o teste.
            context.Database.EnsureCreated();
            return context;
        }

        // Teste de cenário de sucesso: Cliente válido, compra válida, sem restrições.
        [Fact]
        public async Task CanPurchase_ShouldReturnTrue_ForValidCustomerAndPurchase()
        {
            // Arrange
            using var context = CreateInMemoryDbContext(); // DbContext em memória
            var mockDateTimeProvider = new Mock<IDateTimeProvider>();
            // Configura a data e hora para um dia útil e horário comercial (Ex: Terça-feira, 10h UTC)
            mockDateTimeProvider.Setup(x => x.UtcNow).Returns(new DateTime(2024, 8, 20, 10, 0, 0, DateTimeKind.Utc));

            await context.SaveChangesAsync(); // Persiste o cliente no DB em memória

            var customerService = new CustomerService(context, mockDateTimeProvider.Object);

            // Act
            var result = await customerService.CanPurchaseAsync(1, 50m);

            // Assert
            Assert.True(result);
        }

        // --- Testes para validação de Argumentos (customerId e purchaseValue) ---
        [Fact]
        public async Task CanPurchase_ShouldThrowArgumentOutOfRangeException_WhenCustomerIdIsZero()
        {
            using var context = CreateInMemoryDbContext();
            var mockDateTimeProvider = new Mock<IDateTimeProvider>();
            var customerService = new CustomerService(context, mockDateTimeProvider.Object);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => customerService.CanPurchaseAsync(0, 100m));
        }

        [Fact]
        public async Task CanPurchase_ShouldThrowArgumentOutOfRangeException_WhenCustomerIdIsNegative()
        {
            using var context = CreateInMemoryDbContext();
            var mockDateTimeProvider = new Mock<IDateTimeProvider>();
            var customerService = new CustomerService(context, mockDateTimeProvider.Object);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => customerService.CanPurchaseAsync(-1, 100m));
        }

        [Fact]
        public async Task CanPurchase_ShouldThrowArgumentOutOfRangeException_WhenPurchaseValueIsZero()
        {
            using var context = CreateInMemoryDbContext();
            var mockDateTimeProvider = new Mock<IDateTimeProvider>();
            var customerService = new CustomerService(context, mockDateTimeProvider.Object);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => customerService.CanPurchaseAsync(1, 0m));
        }

        [Fact]
        public async Task CanPurchase_ShouldThrowArgumentOutOfRangeException_WhenPurchaseValueIsNegative()
        {
            using var context = CreateInMemoryDbContext();
            var mockDateTimeProvider = new Mock<IDateTimeProvider>();
            var customerService = new CustomerService(context, mockDateTimeProvider.Object);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => customerService.CanPurchaseAsync(1, -10m));
        }

        // --- Teste para Regra de Negócio: Cliente não existe ---
        [Fact]
        public async Task CanPurchase_ShouldThrowInvalidOperationException_WhenCustomerDoesNotExist()
        {
            using var context = CreateInMemoryDbContext();
            var mockDateTimeProvider = new Mock<IDateTimeProvider>();
            var customerService = new CustomerService(context, mockDateTimeProvider.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => customerService.CanPurchaseAsync(999, 100m)); // ID inexistente
        }

        // --- Testes para Regra de Negócio: Uma compra por mês ---
        [Fact]
        public async Task CanPurchase_ShouldReturnFalse_WhenCustomerHasOrderInLastMonth()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var mockDateTimeProvider = new Mock<IDateTimeProvider>();
            var now = new DateTime(2024, 8, 20, 10, 0, 0, DateTimeKind.Utc); // Data de referência
            mockDateTimeProvider.Setup(x => x.UtcNow).Returns(now);

            context.Orders.Add(new Order { CustomerId = 1, OrderDate = now.AddDays(-10), Value = 50m }); // Ordem dentro do último mês
            await context.SaveChangesAsync();

            var customerService = new CustomerService(context, mockDateTimeProvider.Object);

            // Act
            var result = await customerService.CanPurchaseAsync(1, 50m);

            // Assert
            Assert.False(result); // Deve retornar falso
        }

        [Fact]
        public async Task CanPurchase_ShouldReturnTrue_WhenCustomerHasOrderOutsideLastMonth()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var mockDateTimeProvider = new Mock<IDateTimeProvider>();
            var now = new DateTime(2024, 8, 20, 10, 0, 0, DateTimeKind.Utc);
            mockDateTimeProvider.Setup(x => x.UtcNow).Returns(now);

            context.Orders.Add(new Order { CustomerId = 1, OrderDate = now.AddMonths(-2), Value = 50m }); // Ordem fora do último mês
            await context.SaveChangesAsync();

            var customerService = new CustomerService(context, mockDateTimeProvider.Object);

            // Act
            var result = await customerService.CanPurchaseAsync(1, 50m);

            // Assert
            Assert.True(result); // Deve retornar verdadeiro
        }

        // --- Testes para Regra de Negócio: Primeira compra de até R$100,00 ---
        [Fact]
        public async Task CanPurchase_ShouldReturnFalse_WhenFirstPurchaseAndValueIsOverOneHundred()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var mockDateTimeProvider = new Mock<IDateTimeProvider>();
            mockDateTimeProvider.Setup(x => x.UtcNow).Returns(new DateTime(2024, 8, 20, 10, 0, 0, DateTimeKind.Utc));

            // Nenhuma ordem para este cliente, simulando a primeira compra.
            await context.SaveChangesAsync();

            var customerService = new CustomerService(context, mockDateTimeProvider.Object);

            // Act
            var result = await customerService.CanPurchaseAsync(1, 101m); // Valor acima de 100

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CanPurchase_ShouldReturnTrue_WhenFirstPurchaseAndValueIsOneHundredOrLess()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var mockDateTimeProvider = new Mock<IDateTimeProvider>();
            mockDateTimeProvider.Setup(x => x.UtcNow).Returns(new DateTime(2024, 8, 20, 10, 0, 0, DateTimeKind.Utc));

            await context.SaveChangesAsync();

            var customerService = new CustomerService(context, mockDateTimeProvider.Object);

            // Act
            var result = await customerService.CanPurchaseAsync(1, 100m); // Valor de 100

            // Assert
            Assert.True(result);
        }

        // --- Testes para Regra de Negócio: Horário comercial e dias úteis ---
        [Theory] 
        [InlineData(7)] // Antes das 8h UTC
        [InlineData(19)] // Depois das 18h UTC
        public async Task CanPurchase_ShouldReturnFalse_WhenOutsideBusinessHours(int hour)
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var mockDateTimeProvider = new Mock<IDateTimeProvider>();
            // Configura a data e hora para um dia de semana (Terça-feira), mas fora do horário comercial
            mockDateTimeProvider.Setup(x => x.UtcNow).Returns(new DateTime(2024, 8, 20, hour, 0, 0, DateTimeKind.Utc));

            await context.SaveChangesAsync();

            var customerService = new CustomerService(context, mockDateTimeProvider.Object);

            // Act
            var result = await customerService.CanPurchaseAsync(1, 50m);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData(DayOfWeek.Saturday)]
        [InlineData(DayOfWeek.Sunday)]
        public async Task CanPurchase_ShouldReturnFalse_WhenOnWeekend(DayOfWeek dayOfWeek)
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var mockDateTimeProvider = new Mock<IDateTimeProvider>();
            // Configura a data para um dia específico do fim de semana (ex: o próximo sábado ou domingo a partir de uma base)
            var date = new DateTime(2024, 8, 19, 10, 0, 0, DateTimeKind.Utc); // Base: Segunda-feira, 10h UTC
            while (date.DayOfWeek != dayOfWeek)
            {
                date = date.AddDays(1); // Avança até o dia da semana desejado
            }
            mockDateTimeProvider.Setup(x => x.UtcNow).Returns(date);

            await context.SaveChangesAsync();

            var customerService = new CustomerService(context, mockDateTimeProvider.Object);

            // Act
            var result = await customerService.CanPurchaseAsync(1, 50m);

            // Assert
            Assert.False(result);
        }
    }
}
