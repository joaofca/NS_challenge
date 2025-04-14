using System.ComponentModel.DataAnnotations;

namespace CollisionEvents.Contracts.Api.CollisionEvents.Requests
{
    public class AddCollisionEventsRequest
    {
        [Required]
        [Length(1, 60)]
        public required string MessageId { get; set; }

        [Required]
        [Length(1, 60)]
        public required string CollisionEventId { get; set; }

        [Required]
        [Length(1, 60)]
        public required string SatelliteId { get; set; }

        [Required]
        [Length(1, 60)]
        public required string OperatorId { get; set; }

        [Required]
        [Length(1, 60)]
        public required string ChaserObjectId { get; set; }

        [Required]
        public required DateTimeOffset CollisionDate { get; set; }

        [Required]
        [Range(0, 1)]
        public required float CollisionProbability { get; set; }
    }
}
