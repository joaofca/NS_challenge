namespace CollisionEvents.Domain.Dto
{
    public record OperatorMessageIds
    {
        public required string OperatorId { get; set; }
        public required string MessageId { get; set; }
    }
}
