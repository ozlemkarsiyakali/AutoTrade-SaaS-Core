using Microsoft.AspNetCore.Mvc;

namespace AutoTrade.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomBaseController : ControllerBase
{
    [NonAction]
    public IActionResult CreateActionResult<T>(T data, int statusCode = 200)
    {
        return new ObjectResult(data) { StatusCode = statusCode };
    }

    [NonAction]
    public IActionResult CreateActionResult(int statusCode = 204)
    {
        return new ObjectResult(null) { StatusCode = statusCode };
    }
}