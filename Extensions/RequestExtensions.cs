using Microsoft.AspNetCore.Http;

namespace Weather_App.Extensions
{
    public static class RequestExtensions
    {
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }
    }
}