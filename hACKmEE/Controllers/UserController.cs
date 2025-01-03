using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using hACKmEE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace hACKmEE;

using System.Data.SqlClient;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly string _connectionString = "Server=localhost;Database=hACKmEE;User Id=hACKmEE;Password=hACKmEE;Trust Server Certificate=True;";
    private readonly string _jwtSecret = "ATUVULABELLECLE";

    [HttpGet("all")]
    public IActionResult GetAll()
    {
        string query = "SELECT * FROM Users";
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                var reader = cmd.ExecuteReader();
                var users = new List<User>();
                while (reader.Read())
                {
                    users.Add(new User
                    {
                        Id = (int) reader["Id"],
                        Username = (string) reader["Username"],
                        Email = (string) reader["Email"],
                        Password = (string) reader["Password"]
                    });
                }
                return Ok(users);
            }
        }
    }

    [HttpGet("byemail")]
    public IActionResult GetByEmail(string email)
    {
        string query = "SELECT * FROM Users WHERE Email = '" + email + "'";
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return Ok(new User
                    {
                        Id = (int) reader["Id"],
                        Username = (string) reader["Username"],
                        Email = (string) reader["Email"],
                        Password = (string) reader["Password"]
                    });
                }
                return NotFound();
            }
        }
    }

    [HttpDelete("delete")]
    public IActionResult Delete(int id)
    {
        string query = "DELETE FROM Users WHERE Id = " + id;
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.ExecuteNonQuery();
                return Ok("User deleted");
            }
        }
    }

    [HttpPut("update")]
    public IActionResult Update(int id, string username, string email, string password)
    {
        string query = "UPDATE Users SET Username = '" + username + "', Email = '" + email + "', Password = '" + password + "' WHERE Id = " + id;
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.ExecuteNonQuery();
                return Ok("User updated");
            }
        }
    }
    
    [HttpPost("login")]
    public IActionResult Login(string email, string password)
    {
        string query = $"SELECT * FROM Users WHERE Email = '{email}' AND Password = '{password}'"; 
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(_jwtSecret);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Email, email)
                        }),
                        Expires = DateTime.UtcNow.AddHours(1),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    return Ok(new { Token = tokenHandler.WriteToken(token) });
                }
                return Unauthorized();
            }
        }
    }

    [HttpPost("register")]
    public IActionResult Register(string username, string email, string password)
    {
        string query = $"INSERT INTO Users (Username, Email, Password) VALUES ('{username}', '{email}', '{password}')";
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.ExecuteNonQuery();
                return Ok("User registered");
            }
        }
    }
}
