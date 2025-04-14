namespace CollisionEvents.Contracts.Api.Common
{
    public class SuccessOrErrorResponse<TSuccess, TError> : VoidOrErrorResponse<TError>
        where TError : Enum
    {
        public TSuccess? Model { get; set; }

        public static SuccessOrErrorResponse<TSuccess, TError> Success(TSuccess? model)
            => new SuccessOrErrorResponse<TSuccess, TError> { Model = model };

        public static SuccessOrErrorResponse<TSuccess, TError> Error(List<ErrorDetail<TError>> errorDetails)
            => new SuccessOrErrorResponse<TSuccess, TError> { ErrorDetails = errorDetails };
    }
}
