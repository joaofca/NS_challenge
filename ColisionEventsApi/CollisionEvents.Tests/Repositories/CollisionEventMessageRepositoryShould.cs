using CollisionEvents.Domain.Entities;
using CollisionEvents.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CollisionEvents.UnitTests.Repositories
{
    public class CollisionEventMessageRepositoryShould
    {
        private AppDbContext CreateInMemoryDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task ValidateExistingMessageIdsAsync_ReturnsMatchingIds()
        {
            var dbContext = CreateInMemoryDbContext("ValidateExistingMessageIds");
            dbContext.CollisionEventMessages.AddRange(
                new CollisionEventMessage
                {
                    MessageId = "msg1",
                    CollisionEventId = "ev1",
                    CollisionDate = DateTimeOffset.UtcNow,
                    CollisionProbability = 0.9f,
                    OperatorId = "op1",
                    SatelliteId = "sat1",
                    ChaserObjectId = "ch1",
                    IsDeleted = false
                },
                new CollisionEventMessage
                {
                    MessageId = "msg2",
                    CollisionEventId = "ev2",
                    CollisionDate = DateTimeOffset.UtcNow,
                    CollisionProbability = 0.95f,
                    OperatorId = "op2",
                    SatelliteId = "sat2",
                    ChaserObjectId = "ch2",
                    IsDeleted = true
                }
            );
            await dbContext.SaveChangesAsync();

            var repository = new CollisionEventMessageRepository(dbContext);
            var result = await repository.ValidateExistingMessageIdsAsync(new[] { "msg1", "msg2" });

            Assert.Single(result);
            Assert.Equal("msg1", result[0].MessageId);
        }

        [Fact]
        public async Task AddCollisionEventMessagesAsync_AddsRecords()
        {
            var dbContext = CreateInMemoryDbContext("AddCollisionEventMessages");
            var repository = new CollisionEventMessageRepository(dbContext);

            var messages = new List<CollisionEventMessage>
            {
                new()
                {
                    MessageId = "msg1",
                    CollisionEventId = "ev1",
                    CollisionDate = DateTimeOffset.UtcNow,
                    CollisionProbability = 0.9f,
                    OperatorId = "op1",
                    SatelliteId = "sat1",
                    ChaserObjectId = "ch1",
                    IsDeleted = false
                },
                new()
                {
                    MessageId = "msg2",
                    CollisionEventId = "ev2",
                    CollisionDate = DateTimeOffset.UtcNow.AddDays(1),
                    CollisionProbability = 0.95f,
                    OperatorId = "op1",
                    SatelliteId = "sat2",
                    ChaserObjectId = "ch2",
                    IsDeleted = false
                }
            };

            await repository.AddCollisionEventMessagesAsync(messages);
            await dbContext.SaveChangesAsync();

            var stored = await dbContext.CollisionEventMessages.ToListAsync();
            Assert.Equal(2, stored.Count);
        }

        [Fact]
        public async Task DeleteCollisionEventMessagesAsync_MarksMessagesAsDeleted()
        {
            var dbContext = CreateInMemoryDbContext("DeleteCollisionEventMessages");
            dbContext.CollisionEventMessages.AddRange(
                new CollisionEventMessage
                {
                    MessageId = "msg1",
                    CollisionEventId = "ev1",
                    CollisionDate = DateTimeOffset.UtcNow,
                    CollisionProbability = 0.9f,
                    OperatorId = "op1",
                    SatelliteId = "sat1",
                    ChaserObjectId = "ch1",
                    IsDeleted = false
                },
                new CollisionEventMessage
                {
                    MessageId = "msg2",
                    CollisionEventId = "ev2",
                    CollisionDate = DateTimeOffset.UtcNow,
                    CollisionProbability = 0.8f,
                    OperatorId = "op1",
                    SatelliteId = "sat2",
                    ChaserObjectId = "ch2",
                    IsDeleted = false
                }
            );
            await dbContext.SaveChangesAsync();

            var repository = new CollisionEventMessageRepository(dbContext);
            await repository.DeleteCollisionEventMessagesAsync("op1", new[] { "msg1" });
            await dbContext.SaveChangesAsync();

            var result = await dbContext.CollisionEventMessages.FirstAsync(x => x.MessageId == "msg1");
            Assert.True(result.IsDeleted);
        }

        [Fact]
        public async Task GetCollisionEventMessages_ReturnsFilteredMessages()
        {
            var dbContext = CreateInMemoryDbContext("GetCollisionEventMessages");
            dbContext.CollisionEventMessages.AddRange(
                new CollisionEventMessage
                {
                    MessageId = "msg1",
                    CollisionEventId = "ev1",
                    CollisionDate = DateTimeOffset.UtcNow.AddDays(-1),
                    CollisionProbability = 0.9f,
                    OperatorId = "op1",
                    SatelliteId = "sat1",
                    ChaserObjectId = "ch1",
                    IsDeleted = false
                },
                new CollisionEventMessage
                {
                    MessageId = "msg2",
                    CollisionEventId = "ev2",
                    CollisionDate = DateTimeOffset.UtcNow.AddDays(-10),
                    CollisionProbability = 0.5f,
                    OperatorId = "op1",
                    SatelliteId = "sat2",
                    ChaserObjectId = "ch2",
                    IsDeleted = false
                }
            );
            await dbContext.SaveChangesAsync();

            var repository = new CollisionEventMessageRepository(dbContext);
            var result = await repository.GetCollisionEventMessages("op1", DateTimeOffset.UtcNow.AddDays(-2), 0.8f);

            Assert.Single(result);
            Assert.Equal("msg1", result[0].MessageId);
        }
    }
}
