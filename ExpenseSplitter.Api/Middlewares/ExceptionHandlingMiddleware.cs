using ExpenseSplitter.Domain.Exceptions.Authentication;

namespace ExpenseSplitter.Api.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(Exception exception)
            {
                var (statusCode, message) = HandleException(exception);
                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(
                    new { Message = message }
                );
            }

        }

        private static (int StatusCode, string Message) HandleException(Exception exception) =>
            exception switch
            {

                EmailAlreadyExistsException =>
                    (StatusCodes.Status409Conflict, exception.Message),

                InvalidCredentialsException =>
                    (StatusCodes.Status401Unauthorized, exception.Message),

                PasswordsDoNotMatchException =>
                    (StatusCodes.Status400BadRequest, exception.Message),

                _ =>
                    (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
            };

            
    }
}
