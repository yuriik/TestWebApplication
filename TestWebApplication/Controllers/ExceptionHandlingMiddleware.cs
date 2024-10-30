using System.Text.Json;
using TestWebApplication.models;

namespace TestWebApplication.Controllers
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly ApplicationDbContext _dbContext;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            ApplicationDbContext dbContext)
        {
            _next = next;
            _logger = logger;
            _dbContext = dbContext;
        }

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

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var eventId = Guid.NewGuid().ToString();
            var exceptionLog = new ExceptionLog
            {
                EventId = eventId,
                Timestamp = DateTime.UtcNow,
                QueryParams = JsonSerializer.Serialize(context.Request.Query),
                RequestBody = await new StreamReader(context.Request.Body).ReadToEndAsync(),
                StackTrace = exception.StackTrace
            };

            await _dbContext.ExceptionLogs.AddAsync(exceptionLog);
            await _dbContext.SaveChangesAsync();

            context.Response.ContentType = "application/json";

            if (exception is SecureException)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    type = exception.GetType().Name,
                    id = eventId,
                    data = new { message = exception.Message }
                }));
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    type = "Exception",
                    id = eventId,
                    data = new { message = $"Internal server error ID = {eventId}" }
                }));
            }

            _logger.LogError(exception, $"Exception caught: {exception.Message}");
        }
    }
}
