using System.ComponentModel.DataAnnotations;

namespace CollisionEvents.Contracts.Api.CollisionEvents.Requests
{
    public class DeleteCollisionEventsRequest
    {
        [Required]
        [Length(1, 60)]
        public required string MessageId { get; set; }

        [Required]
        [Length(1, 60)]
        public required string OperatorId { get; set; }
    }
}
