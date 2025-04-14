using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace CollisionEvents.Application.UserRequest
{
    public interface IUserRequestService
    {
        string GetOperatorId();

        bool IsEmailValid(string email);
    }

    public class UserRequestService : IUserRequestService
    {
        private IHttpContextAccessor _httpContextAccessor;

        private string _emailRegex = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$";

        public UserRequestService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetOperatorId()
        {
            if (_httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("operatorId", out var value))
            {
                return value.ToString();
            }

            return string.Empty;
        }

        public bool IsEmailValid(string email)
            => Regex.IsMatch(email, _emailRegex);
    }
}
