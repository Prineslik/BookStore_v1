using BookStore.API.Helpers;
using BookStore.Application.Contracts.Common;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Extensions
{
    public static class ErrorExtensions
    {
        public static ActionResult ToErrorResult(this Error error, HttpContext context)
        {
            var problem = ProblemDetailsFactory.FromError(context, error);
            return new ObjectResult(problem) 
            { 
                StatusCode = problem.Status,
                ContentTypes = { "application/problem+json" } 
            };
        }
    }
}
