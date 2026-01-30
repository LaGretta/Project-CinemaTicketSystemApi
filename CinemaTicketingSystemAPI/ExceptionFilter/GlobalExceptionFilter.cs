using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CinemaTicketingSystemAPI.ExceptionFilter;

public class GlobalExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var response = new
        {
            Error = "Error Server", 
            Message = context.Exception.Message,
            TimeStamp = DateTime.Now
        };
        context.Result = new JsonResult(response) { StatusCode = 500 };
    }
}