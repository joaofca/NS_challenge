using CollisionEvents.Contracts.Api.CollisionEvents.Requests;
using CollisionEvents.Contracts.Api.CollisionEvents.Responses;
using CollisionEvents.Contracts.Api.Common;
using CollisionEvents.Contracts.Application.Services;
using CollisionEvents.Contracts.Infrastructure.Repositories;
using CollisionEvents.Domain.Entities;

namespace CollisionEvents.Application.CollisionEvents
{
    public class CollisionEventsService(
        ICollisionEventMessageRepository collisionEventMessageRepository,
        ISatelliteOperatorRepository satelliteOperatorRepository) : ICollisionEventsService
    {
        public async Task<VoidOrErrorResponse<AddCollisionEventsErrorCode>> AddCollisionEventsAsync(string operatorId, IEnumerable<AddCollisionEventsRequest> collisionEventsToAdd)
        {
            var requestValidationResult = await ValidateAddCollisionEventsRequestAsync(operatorId, collisionEventsToAdd);

            if (requestValidationResult.HasErrors)
                return requestValidationResult;

            await collisionEventMessageRepository.AddCollisionEventMessagesAsync(collisionEventsToAdd.Select(x => new CollisionEventMessage
            {
                OperatorId = operatorId,
                ChaserObjectId = x.ChaserObjectId,
                CollisionDate = x.CollisionDate,
                CollisionProbability = x.CollisionProbability,
                CollisionEventId = x.CollisionEventId,
                SatelliteId = x.SatelliteId,
                MessageId = x.MessageId,
                IsDeleted = false
            }));

            await collisionEventMessageRepository.SaveChangesAsync();

            return requestValidationResult;
        }

        protected async Task<VoidOrErrorResponse<AddCollisionEventsErrorCode>> ValidateAddCollisionEventsRequestAsync(string operatorId, IEnumerable<AddCollisionEventsRequest> collisionEventsToAdd)
        {
            var requestValidationResult = new VoidOrErrorResponse<AddCollisionEventsErrorCode>();

            #region Validate operator id

            var distinctOperatorIds = collisionEventsToAdd
                .Select(x => x.OperatorId)
                .ToHashSet();

            // The operator can only send his own messages
            if (distinctOperatorIds.Count > 1 || operatorId != distinctOperatorIds.First())
                requestValidationResult.AddErrorDetail(new ErrorDetail<AddCollisionEventsErrorCode>(AddCollisionEventsErrorCode.InvalidOperatorId, operatorId));
            else
            {
                // Validate the existence of the received operator id
                if ((await satelliteOperatorRepository.GetSatelliteOperatorAsync(operatorId)) == null)
                    requestValidationResult.AddErrorDetail(new ErrorDetail<AddCollisionEventsErrorCode>(AddCollisionEventsErrorCode.InvalidOperatorId, operatorId));
            }

            #endregion Validate operator id

            #region Validate message id existance

            // If the operator identity is not valid, it does not makes sense to continue
            if (!requestValidationResult.ErrorDetails.Any())
            {
                // Validate if the request has duplicated messages
                var duplicatedMessageIds = collisionEventsToAdd
                .GroupBy(g => g.MessageId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key).ToList();

                if (duplicatedMessageIds.Count > 0)
                    requestValidationResult.AddErrorDetail(new ErrorDetail<AddCollisionEventsErrorCode>(AddCollisionEventsErrorCode.RequestWithDuplicatedMessageIds, string.Join(";", duplicatedMessageIds)));

                // Validate the existance of the received messages
                var existingMessageIds = await collisionEventMessageRepository.ValidateExistingMessageIdsAsync(duplicatedMessageIds);

                if (existingMessageIds.Length > 0)
                    requestValidationResult.AddErrorDetail(new ErrorDetail<AddCollisionEventsErrorCode>(AddCollisionEventsErrorCode.MessageIdAlreadyReceived, string.Join(";", existingMessageIds.Select(x => x.MessageId))));

                #endregion Validate message id existance

                #region Validate collision event date

                var currentDatetime = DateTimeOffset.Now;

                var expiredCollisionEventMessages = collisionEventsToAdd
                    .Where(x => x.CollisionDate <= currentDatetime)
                    .Select(x => x.MessageId);

                if (expiredCollisionEventMessages.Any())
                    requestValidationResult.AddErrorDetail(new ErrorDetail<AddCollisionEventsErrorCode>(AddCollisionEventsErrorCode.InvalidCollisionDate, string.Join(";", expiredCollisionEventMessages.OrderBy(x => x))));
            }

            #endregion Validate collision event date

            return requestValidationResult;
        }

        public async Task<VoidOrErrorResponse<DeleteCollisionEventsErrorCode>> DeleteCollisionEventsAsync(string operatorId, IEnumerable<DeleteCollisionEventsRequest> collisionEventsToDelete)
        {
            var requestValidationResult = await ValidateDeleteCollisionEventsAsync(operatorId, collisionEventsToDelete);

            if (requestValidationResult.HasErrors)
                return requestValidationResult;

            await collisionEventMessageRepository.DeleteCollisionEventMessagesAsync(operatorId, collisionEventsToDelete.Select(x => x.MessageId));

            await collisionEventMessageRepository.SaveChangesAsync();

            return requestValidationResult;
        }

        protected async Task<VoidOrErrorResponse<DeleteCollisionEventsErrorCode>> ValidateDeleteCollisionEventsAsync(string operatorId, IEnumerable<DeleteCollisionEventsRequest> collisionEventsToDelete)
        {
            var requestValidationResult = new VoidOrErrorResponse<DeleteCollisionEventsErrorCode>();

            #region Validate operator id

            var distinctOperatorIds = collisionEventsToDelete
                .Select(x => x.OperatorId)
                .ToHashSet();

            // The operator can only delete his own messages
            if (distinctOperatorIds.Count > 1 || operatorId != distinctOperatorIds.First())
                requestValidationResult.AddErrorDetail(new ErrorDetail<DeleteCollisionEventsErrorCode>(DeleteCollisionEventsErrorCode.InvalidOperatorId, operatorId));
            else
            {
                // Validate the validity of the received operator id
                var satelliteOperator = await satelliteOperatorRepository.GetSatelliteOperatorAsync(operatorId);

                if (satelliteOperator == null)
                    requestValidationResult.AddErrorDetail(new ErrorDetail<DeleteCollisionEventsErrorCode>(DeleteCollisionEventsErrorCode.InvalidOperatorId, operatorId));
            }

            #endregion Validate operator id

            #region Validate message id existance and ownership

            // If the operator identity is not valid, it does not makes sense to continue
            if (!requestValidationResult.ErrorDetails.Any())
            {
                // Validate if the request has duplicated messages
                var duplicatedMessageIds = collisionEventsToDelete
                    .GroupBy(g => g.MessageId)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key).ToList();

                if (duplicatedMessageIds.Count > 0)
                    requestValidationResult.AddErrorDetail(new ErrorDetail<DeleteCollisionEventsErrorCode>(DeleteCollisionEventsErrorCode.RequestWithDuplicatedMessageIds, string.Join(";", duplicatedMessageIds)));

                var existingMessageIds = (await collisionEventMessageRepository.ValidateExistingMessageIdsAsync(collisionEventsToDelete.Select(x => x.MessageId)))
                    .ToHashSet();

                // Validate the ownership of the messages to delete
                if (existingMessageIds.Any(x => x.OperatorId != operatorId))
                    requestValidationResult.AddErrorDetail(new ErrorDetail<DeleteCollisionEventsErrorCode>(DeleteCollisionEventsErrorCode.InvalidOperatorId, operatorId));

                var receivedMessageIds = collisionEventsToDelete
                    .Select(g => g.MessageId)
                    .ToHashSet();

                // Validate the existance of the received messages
                var invalidMessageIds = receivedMessageIds.Except(receivedMessageIds.Intersect(existingMessageIds.Select(x => x.MessageId)));

                if (invalidMessageIds.Any())
                    requestValidationResult.AddErrorDetail(new ErrorDetail<DeleteCollisionEventsErrorCode>(DeleteCollisionEventsErrorCode.InvalidMessageId, string.Join(";", invalidMessageIds.OrderBy(x => x))));
            }
            #endregion Validate message id existance and ownership

            return requestValidationResult;
        }
    }
}
