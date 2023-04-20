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
public class AuthController : ControllerBase
{


    private readonly ILogger<AuthController> _logger;
    private readonly IConfiguration _config;
    private UserRepository _userRepository;

    public AuthController(ILogger<AuthController> logger, IConfiguration config, UserRepository userRepository)
    {
        _config = config;
        _logger = logger;
        _userRepository = userRepository;

    }


    private string GenerateJwtToken(string username)
    {

        var hostName = System.Net.Dns.GetHostName();
        var ips = System.Net.Dns.GetHostAddresses(hostName);
        var _ipaddr = ips.First().MapToIPv4().ToString();
        _logger.LogInformation(1, $"Taxabooking responding from {_ipaddr}"); //Step C1 M10.01 - Tilføler præcist tidspunkt man har dotnet run.

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Secret"]));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, username)
        };
        var token = new JwtSecurityToken(
        _config["Issuer"],
        "http://localhost",
        claims,
        expires: DateTime.Now.AddMinutes(15),
        signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }



    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] User user) // her skal hentes bruger fra mongo
    {
        var loginUser = await _userRepository.FindUserByUsernameAndPassword(user.MongoId, user.Username, user.Password); // henter bruger

        if (user == null)
        {
            return Unauthorized();
        }

        /* dette skal slettes
        if (loginUser.Username != "username" || loginUser.Password != "password")
        {
            return Unauthorized();
        }
        hertil */

        var token = GenerateJwtToken(user.Username);
        return Ok(new { token });
    }


    [AllowAnonymous]
    [HttpPost("validate")]
    public async Task<IActionResult> ValidateJwtToken([FromBody] string? token)
    {
        if (token.IsNullOrEmpty())
            return BadRequest("Invalid token submited.");
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_config["Secret"]!);
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);
            var jwtToken = (JwtSecurityToken)validatedToken;
            var accountId = jwtToken.Claims.First(
            x => x.Type == ClaimTypes.NameIdentifier).Value;
            return Ok(accountId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(404);
        }
    }


}
