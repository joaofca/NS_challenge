using System.Text.Json.Serialization;

namespace CollisionEvents.Contracts.Api.Common
{
    public class VoidOrErrorResponse<T> where T : Enum
    {
        [JsonIgnore]
        public bool HasErrors
        {
            get => (ErrorDetails?.Count ?? 0) > 0;
        }

        public List<ErrorDetail<T>> ErrorDetails { get; protected set; } = [];

        public VoidOrErrorResponse() { }

        public VoidOrErrorResponse(List<ErrorDetail<T>> errorDetails)
            => ErrorDetails = errorDetails;

        public void AddErrorDetail(ErrorDetail<T> newErrorDetail)
            => ErrorDetails.Add(newErrorDetail);
    }

    public class ErrorDetail<T>
    {
        public T ErrorCode { get; }

        public string? ErrorMessage { get; }

        public ErrorDetail(T errorCode, string? errorMessage = null)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }
    }
}
