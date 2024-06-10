using System.Net;

namespace NZWalks.MiddleWare
{
    public class ExceptionHandleMiddleware
    {
        private readonly ILogger logger1;
        private readonly RequestDelegate next;

        public ExceptionHandleMiddleware( ILogger<ExceptionHandleMiddleware> logger1,RequestDelegate next) 
        {
            this.logger1 = logger1;
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {

                await next(httpContext);
            }

            // Every excpetion which we get in controller it will catch here inside catch block
            catch (Exception ex)
            {
                var errorId = Guid.NewGuid();

                //Log this excpetion 
                logger1.LogError(ex,$"{errorId}: {ex.Message}");

                //return custom response back

                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                httpContext.Response.ContentType = "application/json";

                var error = new
                {
                    Id=errorId,
                    ErrorMessage="Something went wrong! We are looking into resolving this"
                };
                //written as Json As Async
                await httpContext.Response.WriteAsJsonAsync(error);
            }
        }
    }
}
