using System.ComponentModel.DataAnnotations;

namespace CollisionEvents.Contracts.Api.SatelliteOperators.Requests
{
    public class AddSatelliteOperatorRequest
    {
        [Required]
        [Length(1, 40)]
        public required string OperatorId { get; set; }

        [Required]
        [Length(6, 254)]
        public required string OperatorEmail { get; set; }
    }
}
