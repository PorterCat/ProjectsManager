using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProjectsManager.Core.Models;

namespace ProjectsManager.Business.Auth;

public class JwtService(IOptions<AuthSettings> options)
{
    public string GenerateJwtToken(Employee employee, EmployeeRole role)
    {
        var claims = new List<Claim>
        {
            new("email", employee.Email),
            new("id", employee.Id.ToString()),
            new(ClaimTypes.Role, role.ToString())
        };
    
        var jwtToken = new JwtSecurityToken(
            expires: DateTime.UtcNow.Add(options.Value.Expires),
            claims: claims,
            signingCredentials: 
            new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(options.Value.SecretKey)),
                SecurityAlgorithms.HmacSha256));
        
        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }
}