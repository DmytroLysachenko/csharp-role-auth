using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Require authentication by default; individual policies below add detail.
public class UsersController : ControllerBase
{
    [HttpGet("me")]
    [AllowAnonymous]
    public IActionResult WhoAmI()
    {
        // Echo back the identity built by DemoAuthenticationHandler so you can see which claims are flowing in.
        var claims = User.Claims.Select(c => new { c.Type, c.Value });

        return Ok(
            new
            {
                authenticated = User.Identity?.IsAuthenticated ?? false,
                name = User.Identity?.Name,
                claims
            }
        );
    }

    [HttpGet("role-based")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetUserByRole() =>
        Ok($"Access granted to Admin role. Hello, {User.Identity?.Name ?? "unknown"}.");

    [HttpGet("claim-based")]
    [Authorize(Policy = "RequireITDepartment")]
    public IActionResult GetUserByClaim() =>
        Ok($"Access granted to IT department user. Hello, {User.Identity?.Name ?? "unknown"}.");
}
