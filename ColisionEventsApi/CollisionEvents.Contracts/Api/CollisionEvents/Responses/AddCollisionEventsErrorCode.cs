namespace CollisionEvents.Contracts.Api.CollisionEvents.Responses
{
    /// <summary>
    /// Add collision events error codes
    /// </summary>
    public enum AddCollisionEventsErrorCode
    {
        /// <summary>
        /// The collision date must be a future date
        /// </summary>
        InvalidCollisionDate,

        /// <summary>
        /// The received message identifies an invalid operator identifier
        /// </summary>
        InvalidOperatorId,

        /// <summary>
        /// The received message has already been processed
        /// </summary>
        MessageIdAlreadyReceived,

        /// <summary>
        /// The received payload has duplicated message ids
        /// </summary>
        RequestWithDuplicatedMessageIds,
    }
}
