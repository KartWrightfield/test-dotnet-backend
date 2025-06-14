using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace UE.PostOffice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    /// <summary>
    /// Generates a JWT authentication token
    /// </summary>
    /// <returns>A JWT token that can be used to authenticate API requests</returns>
    /// <response code="200">Returns the generated JWT token</response>
    [HttpPost("token")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public IActionResult GetToken()
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = "super-secret-key-that-should-never-be-committed-to-a-repo"u8.ToArray();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Ok(new { token =  tokenHandler.WriteToken(token) });
    }
}