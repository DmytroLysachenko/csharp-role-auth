using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    [HttpGet("role-based")]
    public IActionResult GetUserByRole()
    {
        var user = new ClaimsPrincipal(
            new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.Name, "TestUser"),
                    new Claim(ClaimTypes.Role, "Admin"),
                },
                "mock"
            )
        );

        HttpContext.User = user;

        if (user.IsInRole("Admin"))
        {
            return Ok("Access granted to Admin role.");
        }
        else
        {
            return Forbid("No access");
        }
    }

    [HttpGet("claim-based")]
    public IActionResult GetUserByClaim()
    {
        var user = new ClaimsPrincipal(
            new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.Name, "TestUser"), new Claim("Department", "IT") },
                "mock"
            )
        );

        HttpContext.User = user;

        var hasClaim = user.HasClaim(c => c.Type == "Department" && c.Value == "IT");

        if (hasClaim)
        {
            return Ok("Access granted to IT department user.");
        }
        else
        {
            return Forbid("No access");
        }
    }
}
