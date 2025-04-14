using CollisionEvents.Contracts.Api.Common;
using CollisionEvents.Contracts.Api.SatelliteOperators.Requests;
using CollisionEvents.Contracts.Api.SatelliteOperators.Responses;

namespace CollisionEvents.Contracts.Application.Services
{
    public interface ISatelliteOperatorService
    {
        Task<VoidOrErrorResponse<AddSatelliteOperatorErrorCode>> AddSatelliteOperatorAsync(AddSatelliteOperatorRequest request);
    }
}
