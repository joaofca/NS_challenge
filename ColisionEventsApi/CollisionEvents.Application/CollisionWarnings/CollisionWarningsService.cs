using CollisionEvents.Contracts.Api.CollisionWarnings.Responses;
using CollisionEvents.Contracts.Api.Common;
using CollisionEvents.Contracts.Infrastructure.Repositories;
using CollisionEvents.Domain.Entities;

namespace CollisionEvents.Application.CollisionWarnings
{
    public interface ICollisionWarningsService
    {
        Task<SuccessOrErrorResponse<GetCollisionWarningsResponse, GetCollisionWarningsErrorCode>> GetCollisionWarnings(string operatorId);
    }

    public class CollisionWarningsService(
        ICollisionEventMessageRepository collisionEventMessageRepository,
        ISatelliteOperatorRepository satelliteOperatorRepository) : ICollisionWarningsService
    {
        public async Task<SuccessOrErrorResponse<GetCollisionWarningsResponse, GetCollisionWarningsErrorCode>> GetCollisionWarnings(string operatorId)
        {
            var validationResult = await ValidateRequestAsync(operatorId);

            if (validationResult.HasErrors)
                return SuccessOrErrorResponse<GetCollisionWarningsResponse, GetCollisionWarningsErrorCode>.Error(validationResult.ErrorDetails);

            DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;

            var collisionEventMessages = await collisionEventMessageRepository.GetCollisionEventMessages(operatorId, dateTimeOffset, CollisionsConstants.CollisionWarningThreshold);

            return SuccessOrErrorResponse<GetCollisionWarningsResponse, GetCollisionWarningsErrorCode>.Success(BuildGetCollisionWarningsResponse(collisionEventMessages));
        }

        protected async Task<VoidOrErrorResponse<GetCollisionWarningsErrorCode>> ValidateRequestAsync(string operatorId)
        {
            var requestValidationResult = new VoidOrErrorResponse<GetCollisionWarningsErrorCode>();

            // Validate the existence of the received operator id
            if ((await satelliteOperatorRepository.GetSatelliteOperatorAsync(operatorId)) == null)
                requestValidationResult.AddErrorDetail(new ErrorDetail<GetCollisionWarningsErrorCode>(GetCollisionWarningsErrorCode.InvalidOperatorId, operatorId));

            return requestValidationResult;
        }

        protected GetCollisionWarningsResponse BuildGetCollisionWarningsResponse(CollisionEventMessage[] collisionEventMessages)
        {
            return new GetCollisionWarningsResponse
            {
                CollisionWarnings = collisionEventMessages
                .GroupBy(g => g.SatelliteId)
                .Select(g => g.OrderBy(x => x.CollisionDate))
                .Select(x =>
                {
                    var earliestCollisionEventMessage = x.First();

                    return new GetCollisionWarningsResponseDetails
                    {
                        ChaserObjectId = earliestCollisionEventMessage.ChaserObjectId,
                        EarliestColisionDate = earliestCollisionEventMessage.CollisionDate,
                        HighestCollisionProbability = earliestCollisionEventMessage.CollisionProbability,
                        SatelliteId = earliestCollisionEventMessage.SatelliteId
                    };
                }).ToList()
            };
        }
    }
}