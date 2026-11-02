using ExpenseSplitter.Domain.Exceptions.Authentication;

namespace ExpenseSplitter.Api.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "An error occurred while processing {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path
                );

                var (statusCode, message) = HandleException(exception);

                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(
                    new { Message = message }
                );
            }
        }

        private static (int StatusCode, string Message) HandleException(
            Exception exception) =>
            exception switch
            {
                EmailAlreadyExistsException =>
                    (StatusCodes.Status409Conflict, exception.Message),

                InvalidCredentialsException =>
                    (StatusCodes.Status401Unauthorized, exception.Message),

                PasswordsDoNotMatchException =>
                    (StatusCodes.Status400BadRequest, exception.Message),

                _ =>
                    (StatusCodes.Status500InternalServerError,
                     "An unexpected error occurred.")
            };
    }
}
