using System.ComponentModel.DataAnnotations;
using CollisionEvents.Api.Configurations.Swagger;
using CollisionEvents.Application.UserRequest;
using CollisionEvents.Contracts.Api.CollisionEvents.Requests;
using CollisionEvents.Contracts.Api.CollisionEvents.Responses;
using CollisionEvents.Contracts.Api.Common;
using CollisionEvents.Contracts.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CollisionEvents.Api.Controllers.CollisionEvents
{
    /// <summary>
    /// Collision events controller
    /// </summary>
    /// <param name="collisionEventsService"></param>
    /// <param name="userRequestService"></param>
    // por versão a funcionar
    //[ApiVersion("1.0.0")]
    [ApiController]
    [Route("collisionEvents")]
    public class CollisionEventsController(
        ICollisionEventsService collisionEventsService,
        IUserRequestService userRequestService) : ControllerBase
    {
        /// <summary>
        /// Add collision events
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [AddRequiredOperatorIdParameter]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ErrorDetail<AddCollisionEventsErrorCode>>), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> AddCollisionEvents([FromBody, Required, MinLength(1), MaxLength(2048)] AddCollisionEventsRequest[] request)
        {
            var result = await collisionEventsService.AddCollisionEventsAsync(userRequestService.GetOperatorId(), request);

            if (result.HasErrors)
                return UnprocessableEntity(result.ErrorDetails);

            return Ok();
        }

        /// <summary>
        /// Delete collision events
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete]
        [AddRequiredOperatorIdParameter]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ErrorDetail<DeleteCollisionEventsErrorCode>>), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> DeleteCollisionEvents([FromBody, Required, MinLength(1), MaxLength(2048)] DeleteCollisionEventsRequest[] request)
        {
            var result = await collisionEventsService.DeleteCollisionEventsAsync(userRequestService.GetOperatorId(), request);

            if (result.HasErrors)
                return UnprocessableEntity(result.ErrorDetails);

            return Ok();
        }
    }
}
