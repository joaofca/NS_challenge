using CollisionEvents.Application.UserRequest;
using CollisionEvents.Contracts.Api.Common;
using CollisionEvents.Contracts.Api.SatelliteOperators.Requests;
using CollisionEvents.Contracts.Api.SatelliteOperators.Responses;
using CollisionEvents.Contracts.Application.Services;
using CollisionEvents.Contracts.Infrastructure.Repositories;

namespace CollisionEvents.Application.SatelliteOperator
{
    public class SatelliteOperatorService(
        ISatelliteOperatorRepository satelliteOperatorRepository,
        IUserRequestService userRequestService) : ISatelliteOperatorService
    {
        public async Task<VoidOrErrorResponse<AddSatelliteOperatorErrorCode>> AddSatelliteOperatorAsync(AddSatelliteOperatorRequest request)
        {
            var requestValidationResult = await ValidateAddSatelliteOperatorRequest(request);

            if (requestValidationResult.HasErrors)
                return requestValidationResult;

            await satelliteOperatorRepository.AddSatelliteOperator(new Domain.Entities.SatelliteOperator
            {
                OperatorId = request.OperatorId,
                OperatorEmail = request.OperatorEmail
            });

            await satelliteOperatorRepository.SaveChangesAsync();

            return requestValidationResult;
        }

        private async Task<VoidOrErrorResponse<AddSatelliteOperatorErrorCode>> ValidateAddSatelliteOperatorRequest(AddSatelliteOperatorRequest request)
        {
            var requestValidationResult = new VoidOrErrorResponse<AddSatelliteOperatorErrorCode>();

            #region Validate operator id existance

            var satelliteOperator = await satelliteOperatorRepository.GetSatelliteOperatorAsync(request.OperatorId);

            if (satelliteOperator != null)
                requestValidationResult.AddErrorDetail(new ErrorDetail<AddSatelliteOperatorErrorCode>(AddSatelliteOperatorErrorCode.OperatorAlreadyExists, request.OperatorId));

            #endregion Validate operator id existance

            #region Validate operator email

            if (!userRequestService.IsEmailValid(request.OperatorEmail))
                requestValidationResult.AddErrorDetail(new ErrorDetail<AddSatelliteOperatorErrorCode>(AddSatelliteOperatorErrorCode.InvalidOperatorEmail, request.OperatorEmail));

            #endregion Validate operator email

            return requestValidationResult;
        }
    }
}
