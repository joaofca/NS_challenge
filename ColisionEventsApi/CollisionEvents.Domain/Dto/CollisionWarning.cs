namespace CollisionEvents.Domain.Dto
{
    public class CollisionWarning
    {
        public required string OperatorId { get; set; }

        public required string SatelliteId { get; set; }

        public required string ChaserObjectId { get; set; }

        public required float HighestCollisionProbability { get; set; }

        public required bool Cancelled { get; set; }
    }
}
