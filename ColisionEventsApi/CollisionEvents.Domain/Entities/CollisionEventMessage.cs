namespace CollisionEvents.Domain.Entities
{
    public class CollisionEventMessage
    {
        public required string MessageId { get; set; }

        public required string CollisionEventId { get; set; }

        public required DateTimeOffset CollisionDate { get; set; }

        public required float CollisionProbability { get; set; }

        public required string OperatorId { get; set; }

        public required string SatelliteId { get; set; }

        public required string ChaserObjectId { get; set; }

        public required bool IsDeleted { get; set; }
    }
}
