using CollisionEvents.Contracts.Infrastructure.Repositories;
using CollisionEvents.Domain.Dto;
using CollisionEvents.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CollisionEvents.Infrastructure.Repositories
{
    public class CollisionEventMessageRepository : RepositoryBase, ICollisionEventMessageRepository
    {
        public CollisionEventMessageRepository(AppDbContext dbContext) : base(dbContext) { }

        public async Task<OperatorMessageIds[]> ValidateExistingMessageIdsAsync(IEnumerable<string> messageIds)
        {
            return await _dbContext.CollisionEventMessages
                .Where(x => messageIds.Contains(x.MessageId) && !x.IsDeleted)
                .Select(x => new OperatorMessageIds
                {
                    OperatorId = x.OperatorId,
                    MessageId = x.MessageId
                })
                .ToArrayAsync();
        }

        public async Task AddCollisionEventMessagesAsync(IEnumerable<CollisionEventMessage> collisionEventMessage)
        {
            await _dbContext.CollisionEventMessages.AddRangeAsync(collisionEventMessage);
        }

        public async Task DeleteCollisionEventMessagesAsync(string operatorId, IEnumerable<string> collisionEventMessageIdsToDelete)
        {
            var collisionEventMessagesToDelete = await _dbContext.CollisionEventMessages
                .Where(x => collisionEventMessageIdsToDelete.Contains(x.MessageId) && !x.IsDeleted && x.OperatorId == operatorId)
                .Select(x => x)
                .ToArrayAsync();

            foreach (var collisionEventMessage in collisionEventMessagesToDelete)
                collisionEventMessage.IsDeleted = true;
        }

        public async Task<CollisionEventMessage[]> GetCollisionEventMessages(string operatorId, DateTimeOffset minimumCollisionDate, float minimumCollisionProbability)
        {
            return await _dbContext.CollisionEventMessages
                .Where(x => x.OperatorId == operatorId && x.CollisionDate >= minimumCollisionDate && x.CollisionProbability >= minimumCollisionProbability && !x.IsDeleted)
                .Select(x => x).ToArrayAsync();
        }
    }
}