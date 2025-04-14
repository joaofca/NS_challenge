using System.ComponentModel.DataAnnotations;
using CollisionEvents.Contracts.Api.Common;
using CollisionEvents.Contracts.Api.SatelliteOperators.Requests;
using CollisionEvents.Contracts.Api.SatelliteOperators.Responses;
using CollisionEvents.Contracts.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CollisionEvents.Api.Controllers.Operators
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="satelliteOperatorService"></param>
    [ApiController]
    [Route("satelliteOperator")]
    public class SatelliteOperatorsController(
        ISatelliteOperatorService satelliteOperatorService) : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ErrorDetail<AddSatelliteOperatorErrorCode>>), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> AddSatelliteOperator([Required] AddSatelliteOperatorRequest request)
        {
            var result = await satelliteOperatorService.AddSatelliteOperatorAsync(request);

            if (result.HasErrors)
                return UnprocessableEntity(result.ErrorDetails);

            return Ok();
        }
    }
}
