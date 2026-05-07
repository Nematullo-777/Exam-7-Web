using System;
using Microsoft.AspNetCore.Mvc;
using OnlineCourses.Application.Common;
using OnlineCourses.Domain.Enums;

namespace OnlineCourses.WebApi.Controllers;

public class BaseController : ControllerBase
{
    protected IActionResult HandleError<T>(Result<T> result)
    {
        return result.ErrorType switch
        {
            ErrorType.Validation  => BadRequest(result),
            ErrorType.NotFound    => NotFound(result),
            ErrorType.Conflict    => Conflict(result),
            ErrorType.Unauthorized => Unauthorized(result),
            ErrorType.Forbidden   => StatusCode(StatusCodes.Status403Forbidden, result),
            _                     => StatusCode(StatusCodes.Status500InternalServerError, result)
        };
    }
}
