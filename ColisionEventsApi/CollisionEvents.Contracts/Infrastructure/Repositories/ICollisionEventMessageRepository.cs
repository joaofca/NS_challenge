using CollisionEvents.Domain.Dto;
using CollisionEvents.Domain.Entities;

namespace CollisionEvents.Contracts.Infrastructure.Repositories
{
    public interface ICollisionEventMessageRepository : IRepositoryBase
    {
        Task<OperatorMessageIds[]> ValidateExistingMessageIdsAsync(IEnumerable<string> messageIds);

        Task AddCollisionEventMessagesAsync(IEnumerable<CollisionEventMessage> collisionEventMessage);

        Task DeleteCollisionEventMessagesAsync(string operatorId, IEnumerable<string> collisionEventMessageIdsToDelete);

        Task<CollisionEventMessage[]> GetCollisionEventMessages(string operatorId, DateTimeOffset minimumCollisionDate, float minimumCollisionProbability);
    }
}
