using System.Net;

namespace tp_final_backend.Middlewares
{
    public class GlobalException(RequestDelegate next, ILogger<GlobalException> logger)
    {
        private readonly RequestDelegate _next = next;

        private readonly ILogger<GlobalException> _logger = logger;


        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción no manejada: {ExceptionMessage}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new ErrorResponse();

            switch (exception)
            {
                //Excepciones especificas 
                case ArgumentNullException:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = "Argumentos invalidos o nulos";
                    response.Detail = exception.Message;
                    break;
                case InvalidOperationException:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = "Operacion invalida";
                    response.Detail = exception.Message;
                    break;
                case UnauthorizedAccessException:
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    response.StatusCode = HttpStatusCode.Unauthorized;
                    response.Message = "No autorizado";
                    response.Detail = exception.Message;
                    break;
                case KeyNotFoundException:
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Message = "Recurso no encontrado";
                    response.Detail = exception.Message;
                    break;
                //Excepcion generica
                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    response.StatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "Error interno del servidor";
                    response.Detail = "Ocurrio un error inesperado. Contacta con soporte.";

                    response.Detail = exception.Message;
                    response.StackTrace = exception.StackTrace;
                    break;
            }

            response.TimeStamp = DateTime.UtcNow;
            return context.Response.WriteAsJsonAsync(response);
        }

    }

    //Estructura estandar para respuestas de error
    public class ErrorResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
        public string? StackTrace { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    }
}