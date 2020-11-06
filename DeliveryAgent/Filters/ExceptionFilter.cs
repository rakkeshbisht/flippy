using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Net;

namespace DeliveryAgent.Filters
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string name, object key)
            : base($"Entity '{name}' ({key}) was not found.")
        {
        }
    }

    public class ParameterRequiredException : Exception
    {
        public ParameterRequiredException(string key)
            : base($"{key} is a required parameter.")
        {
        }
    }

    public class InvalidInputException : Exception
    {
        public InvalidInputException(string key)
            : base($"{key} object passed is a invalid")
        {
        }
    }

    public class UnexpectedErrorException : Exception
    {
        public UnexpectedErrorException()
            : base($"Unexpected error occured.")
        {
        }
    }


    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var statusCode = HttpStatusCode.InternalServerError;

            if (context.Exception is EntityNotFoundException)
            {
                statusCode = HttpStatusCode.NotFound;
            }
            else if (context.Exception is ParameterRequiredException)
            {
                statusCode = HttpStatusCode.BadRequest;
            }
            else if (context.Exception is ParameterRequiredException)
            {
                statusCode = HttpStatusCode.MethodNotAllowed;
            }
            else if (context.Exception is UnexpectedErrorException)
            {
                statusCode = HttpStatusCode.InternalServerError;
            }

            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.StatusCode = (int)statusCode;

            context.Result = new JsonResult(new
            {
                error = new[] { context.Exception.Message },
                stackTrace = context.Exception.StackTrace
            });
        }
    }
}
