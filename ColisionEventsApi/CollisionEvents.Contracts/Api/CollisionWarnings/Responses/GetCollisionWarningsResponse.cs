using System.ComponentModel.DataAnnotations;

namespace CollisionEvents.Contracts.Api.CollisionWarnings.Responses
{
    /// <summary>
    /// Get collision warnings response
    /// </summary>
    public class GetCollisionWarningsResponse
    {
        /// <summary>
        /// Collision warnings
        /// </summary>
        [Required]
        public required List<GetCollisionWarningsResponseDetails> CollisionWarnings { get; set; }
    }

    /// <summary>
    /// Collision warning detail
    /// </summary>
    public class GetCollisionWarningsResponseDetails
    {
        [Required]
        [Length(1, 14)]
        public required string SatelliteId { get; set; }

        [Required]
        [Length(1, 9)]
        public required string ChaserObjectId { get; set; }

        [Required]
        public required DateTimeOffset EarliestColisionDate { get; set; }

        [Required]
        [Range(0, 1)]
        public required float HighestCollisionProbability { get; set; }
    }
}
