using hACKmEE.Models;
using Microsoft.AspNetCore.Mvc;

namespace hACKmEE;

using System.Data.SqlClient;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly string _connectionString = "Server=localhost;Database=hACKmEE;User Id=hACKmEE;Password=hACKmEE;Trust Server Certificate=True;";

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
                    return Ok(new
                    {
                        Id = reader["Id"],
                        Username = reader["Username"],
                        Email = reader["Email"],
                        Password = reader["Password"]
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
}
