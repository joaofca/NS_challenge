using CollisionEvents.Contracts.Api.CollisionEvents.Requests;
using CollisionEvents.Contracts.Api.CollisionEvents.Responses;
using CollisionEvents.Contracts.Api.Common;

namespace CollisionEvents.Contracts.Application.Services
{
    public interface ICollisionEventsService
    {
        Task<VoidOrErrorResponse<AddCollisionEventsErrorCode>> AddCollisionEventsAsync(string operatorId, IEnumerable<AddCollisionEventsRequest> colisionEventsToAdd);

        Task<VoidOrErrorResponse<DeleteCollisionEventsErrorCode>> DeleteCollisionEventsAsync(string operatorId, IEnumerable<DeleteCollisionEventsRequest> colisionEventsToDelete);
    }
}
