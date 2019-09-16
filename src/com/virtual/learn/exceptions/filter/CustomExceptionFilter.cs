using System.Net;
using garda.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace garda.Exception.Filter
{
    ///<summary>Filter qui va renvoyer le bon message d'erreur</summary>
    internal class CustomExceptionFilter : IExceptionFilter
    {

        private readonly ILogger logger;
 
        public CustomExceptionFilter(ILogger<CustomExceptionFilter> logger)
        {
            this.logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            HttpStatusCode status = HttpStatusCode.InternalServerError;
            string message = string.Empty;
    
            var exceptionType = context.Exception.GetType();
            if (exceptionType == typeof(KnownUserException))
            {
                status = HttpStatusCode.BadRequest;
            }
            else if (exceptionType == typeof(UnauthorizedUserException))
            {
                status = HttpStatusCode.Unauthorized;
            }
            else if (exceptionType == typeof(UnknownClientAppException))
            {
                status = HttpStatusCode.Unauthorized;
            }
            else if (exceptionType == typeof(UnknownUserException))
            {
                status = HttpStatusCode.NotFound;
            }
            else if (exceptionType == typeof(RevokedTokenException))
            {
                status = HttpStatusCode.Unauthorized;
            }
            else
            {
                status = HttpStatusCode.ServiceUnavailable;
            }
            message = context.Exception.Message;
            context.ExceptionHandled=true;
    
            HttpResponse response = context.HttpContext.Response;
            response.StatusCode = (int)status;
            response.ContentType = "application/json";
            string err = message + " " + context.Exception.StackTrace;
            logger.LogInformation("Erreur lors de l'éxécution : " + status + " " + err);
            response.WriteAsync(err);
        }
    }
}