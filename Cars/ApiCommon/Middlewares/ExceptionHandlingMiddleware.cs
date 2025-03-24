using Cars.ApiCommon.Errors;
using ApplicationException = Cars.ApiCommon.Exceptions.ApplicationException;

namespace Cars.ApiCommon.Middlewares
{
    /// <summary>
    /// Middleware to handle exceptions in the ASP.NET Core pipeline.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        /// <summary>
        /// The next middleware in the pipeline.
        /// </summary>
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHandlingMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Invokes the middleware to handle exceptions in the request pipeline.
        /// </summary>
        /// <param name="context">The HttpContext.</param>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Handles all exceptions thrown in the request pipeline and
        /// returns a JSON response with the error details.
        /// </summary>
        /// <param name="context">The HttpContext.</param>
        /// <param name="exception">The exception that was thrown.</param>
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            ErrorDetail errorDetail;
            HttpResponse response = context.Response;
            
            // All exceptions either inherit from ApplicationException or are wrapped in it.
            if (exception is ApplicationException appEx)
            {
                response.StatusCode = appEx.HttpStatusCode;
                errorDetail = new ErrorDetail(
                    appEx.StatusCode,
                    appEx.Message
                );
            }
            else // For any other unhandled exception, we return 500 Internal Server Error.
            {
                response.StatusCode = 500;
                errorDetail = new ErrorDetail(
                    "InternalServerError",
                    "An internal server error has occurred."
                );
            }

            response.Headers.TryAdd("x-ms-error-code", errorDetail.Code);
            response.ContentType = "application/json";
            await response.WriteAsJsonAsync(errorDetail).ConfigureAwait(false);
        }
    }
}
