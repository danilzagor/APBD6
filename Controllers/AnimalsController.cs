using APBD6.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Dapper;

namespace APBD6.Controllers;
[ApiController]
[Route("api/animals")]
public class AnimalsController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AnimalsController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
   
    [HttpGet]
    public IActionResult GetAllAnimals(string? orderBy="Name")
    {
        var response = new List<GetAllAnimalsResponse>();
        using (var sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            string safeOrderBy;
            switch (orderBy)
            {
                case "IdAnimal": safeOrderBy = "IdAnimal";
                    break;
                case "Description": safeOrderBy = "Description";
                    break;
                case "Category": safeOrderBy = "Category";
                    break;
                case "Area": safeOrderBy = "Area";
                    break;
                default:
                    safeOrderBy = "Name";
                    break;
            }
            var sqlCommand = new SqlCommand("SELECT * FROM Animal ORDER BY " + safeOrderBy, sqlConnection);
            sqlCommand.Connection.Open();
            var reader = sqlCommand.ExecuteReader();
            
            while (reader.Read())
            {
                response.Add(new GetAllAnimalsResponse(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetString(3),
                    reader.GetString(3)
                    )
                );
            }
        }
        return Ok(response);
    }

    [HttpGet("{id}")]
    public IActionResult GetAnimalById(int id)
    {
        var response = new List<GetAllAnimalsResponse>();
        var sql = "SELECT * FROM Animal WHERE IdAnimal = @Id";
        using (var sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            // var sqlCommand = new SqlCommand("SELECT * FROM Animal WHERE ID = @1", sqlConnection);
            // sqlCommand.Parameters.AddWithValue("@1", id);
            // sqlCommand.Connection.Open();
            //
            
            // var reader = sqlCommand.ExecuteReader();
            // if (!reader.Read()) return NotFound();
            //
            // while (reader.Read())
            // {
            //     response.Add(new GetAllAnimalsResponse(
            //             reader.GetInt32(0),
            //             reader.GetString(1),
            //             reader.GetString(2),
            //             reader.GetString(3),
            //             reader.GetString(3)
            //         )
            //     );
            // }
            var parameters = new { Id = id };
            sqlConnection.Open();
            response = sqlConnection.Query<GetAllAnimalsResponse>(sql, parameters).ToList();
        }
        return Ok(response);
    }
    
    [HttpPost]
    public IActionResult CreateAnimal(CreateAnimalRequest request){
        using (var sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            var sqlCommand = new SqlCommand(
                "INSERT INTO Animal (Name, Description, Category, Area) values (@1, @2, @3, @4); SELECT CAST(SCOPE_IDENTITY() as int)",
                sqlConnection
            );
            sqlCommand.Parameters.AddWithValue("@1", request.Name);
            sqlCommand.Parameters.AddWithValue("@2", request.Description);
            sqlCommand.Parameters.AddWithValue("@3", request.Category);
            sqlCommand.Parameters.AddWithValue("@4", request.Area);
            sqlCommand.Connection.Open();
            
            var id = sqlCommand.ExecuteScalar();
            
            return Created($"api/animals/{id}", new CreateAnimalResponse((int)id, request));
        }
    }
    [HttpPut("{id}")]
    public IActionResult ReplaceAnimal(int id, ReplaceAnimalRequest request){
        using (var sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            var sqlCommand = new SqlCommand(
                "UPDATE Animal SET Name = @1, Description = @2, Category = @3, Area=@4 WHERE IdAnimal = @5",
                sqlConnection
            );
            sqlCommand.Parameters.AddWithValue("@1", request.Name);
            sqlCommand.Parameters.AddWithValue("@2", request.Description);
            sqlCommand.Parameters.AddWithValue("@3", request.Category);
            sqlCommand.Parameters.AddWithValue("@4", request.Area);
            sqlCommand.Parameters.AddWithValue("@5", id);
            
            sqlCommand.Connection.Open();
            
            var affectedRows = sqlCommand.ExecuteNonQuery();
            return affectedRows == 0 ? NotFound() : NoContent();
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteAnimal(int id)
    {
        using (var sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            var sqlCommand = new SqlCommand(
                "DELETE FROM Animal WHERE IdAnimal = @1",
                sqlConnection
            );
            sqlCommand.Parameters.AddWithValue("@1", id);
            sqlCommand.Connection.Open();
            
            var affectedRows = sqlCommand.ExecuteNonQuery();
            return affectedRows == 0 ? NotFound() : NoContent();
        }
    }
}