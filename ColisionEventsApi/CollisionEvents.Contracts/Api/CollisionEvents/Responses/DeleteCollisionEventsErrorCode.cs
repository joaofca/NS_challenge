namespace CollisionEvents.Contracts.Api.CollisionEvents.Responses
{
    /// <summary>
    /// Delete collision events error codes
    /// </summary>
    public enum DeleteCollisionEventsErrorCode
    {
        /// <summary>
        /// Invalid message identifier
        /// </summary>
        InvalidMessageId,

        /// <summary>
        /// Invalid operator identifier
        /// </summary>
        InvalidOperatorId,

        /// <summary>
        /// The received payload has duplicated message ids
        /// </summary>
        RequestWithDuplicatedMessageIds
    }
}
