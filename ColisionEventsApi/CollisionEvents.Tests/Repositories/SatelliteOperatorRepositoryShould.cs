using CollisionEvents.Domain.Entities;
using CollisionEvents.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CollisionEvents.UnitTests.Repositories
{
    public class SatelliteOperatorRepositoryShould
    {
        private AppDbContext CreateInMemoryDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task GetSatelliteOperatorAsync_ReturnsOperator_WhenExists()
        {
            var dbContext = CreateInMemoryDbContext("GetSatelliteOperator_Exists");

            dbContext.SatelliteOperators.Add(new SatelliteOperator
            {
                OperatorId = "op123",
                OperatorEmail = "test@example.com"
            });

            await dbContext.SaveChangesAsync();

            var repository = new SatelliteOperatorRepository(dbContext);
            var result = await repository.GetSatelliteOperatorAsync("op123");

            Assert.NotNull(result);
            Assert.Equal("op123", result!.OperatorId);
            Assert.Equal("test@example.com", result.OperatorEmail);
        }

        [Fact]
        public async Task GetSatelliteOperatorAsync_ReturnsNull_WhenNotFound()
        {
            var dbContext = CreateInMemoryDbContext("GetSatelliteOperator_NotFound");
            var repository = new SatelliteOperatorRepository(dbContext);

            var result = await repository.GetSatelliteOperatorAsync("nonexistent");

            Assert.Null(result);
        }

        [Fact]
        public async Task AddSatelliteOperator_AddsSuccessfully()
        {
            var dbContext = CreateInMemoryDbContext("AddSatelliteOperator");
            var repository = new SatelliteOperatorRepository(dbContext);

            var newOperator = new SatelliteOperator
            {
                OperatorId = "op999",
                OperatorEmail = "op999@example.com"
            };

            await repository.AddSatelliteOperator(newOperator);
            await dbContext.SaveChangesAsync();

            var result = await dbContext.SatelliteOperators.FindAsync("op999");

            Assert.NotNull(result);
            Assert.Equal("op999", result!.OperatorId);
            Assert.Equal("op999@example.com", result.OperatorEmail);
        }

    }
}
