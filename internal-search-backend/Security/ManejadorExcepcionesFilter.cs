using internal_search.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace internal_search_backend.Security
{
    // 402 cuando el usuario se queda sin tokens, sin que cada controlador tenga que capturarlo
    public class ManejadorExcepcionesFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is TokensInsuficientesException ex)
            {
                context.Result = new ObjectResult(new
                {
                    message = ex.Message,
                    saldo = ex.Saldo,
                    costo = ex.Costo
                })
                { StatusCode = StatusCodes.Status402PaymentRequired };

                context.ExceptionHandled = true;
            }
            else if (context.Exception is ReniecNoDisponibleException)
            {
                // La consulta falló dentro de TokenService, que ya devolvió el token al usuario
                context.Result = new ObjectResult(new
                {
                    message = "El servicio de RENIEC no está disponible por ahora. No se descontó ningún token."
                })
                { StatusCode = StatusCodes.Status502BadGateway };

                context.ExceptionHandled = true;
            }
        }
    }
}
