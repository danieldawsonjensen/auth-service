using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Model;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{


    private readonly ILogger<TestController> _logger;
    private readonly IConfiguration _config;

    public TestController(ILogger<TestController> logger, IConfiguration config)
    {
        _config = config;
        _logger = logger;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok("You're authorized!");
    }
}