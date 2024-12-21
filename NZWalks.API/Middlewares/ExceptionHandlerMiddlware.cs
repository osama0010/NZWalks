using System.Net;

namespace NZWalks.API.Middlewares
{
    public class ExceptionHandlerMiddlware
    {
        private readonly ILogger<ExceptionHandlerMiddlware> logger;
        private readonly RequestDelegate next;

        public ExceptionHandlerMiddlware(ILogger<ExceptionHandlerMiddlware> logger, RequestDelegate next)
        {
            this.logger = logger;
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext httpRequest)
        {
            try
            {
                await next(httpRequest);
            }
            catch (Exception exc)
            {
                var errorID = Guid.NewGuid();

                // Logging Exception 
                logger.LogError($"{errorID} : {exc.Message}");

                // Return Custom Error Message
                httpRequest.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                httpRequest.Response.ContentType = "application/json";

                var err = new
                {
                    ErrID = errorID,
                    ErrorMessage = "Something Went Wrong! We're working on it"
                };

                await httpRequest.Response.WriteAsJsonAsync(err);

            }
        }

    }
}
