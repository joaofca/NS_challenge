using CollisionEvents.Api.Configurations.Swagger;
using CollisionEvents.Application.CollisionWarnings;
using CollisionEvents.Application.UserRequest;
using CollisionEvents.Contracts.Api.CollisionWarnings.Responses;
using CollisionEvents.Contracts.Api.Common;
using Microsoft.AspNetCore.Mvc;

namespace CollisionEvents.Api.Controllers.CollisionWarnings
{
    /// <summary>
    /// Collision warnings controller
    /// </summary>
    /// <param name="collisionWarningsService"></param>
    /// <param name="userRequestService"></param>
    [ApiController]
    [Route("collisionWarning")]
    public class CollisionWarningsController(
        ICollisionWarningsService collisionWarningsService,
        IUserRequestService userRequestService) : ControllerBase
    {
        /// <summary>
        /// Get collision warnings
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AddRequiredOperatorIdParameter]
        [ProducesResponseType(typeof(GetCollisionWarningsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ErrorDetail<GetCollisionWarningsErrorCode>>), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> GetCollisionWarnings()
        {
            var result = await collisionWarningsService.GetCollisionWarnings(userRequestService.GetOperatorId());

            if (result.HasErrors)
                return UnprocessableEntity(result.ErrorDetails);

            return Ok(result.Model);
        }
    }
}
