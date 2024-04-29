using Dapper;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using DataModel;
using LiteDB;

namespace Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IConfiguration _config;
    public UsersController(IConfiguration config)
    {
        _config = config;
    }

    [HttpGet("all")]
    public async Task<ActionResult<List<Users>>> GetAllUsers()
    {
        using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
        
        return Ok(await SelectAllUsers(connection));
    }

    [HttpGet("allLiteDB")]
    public ActionResult<List<Users>> GetAllUsersLiteDB()
    {
        var litedb = new LiteDatabase("./Synchronization/data.db");
        var users = litedb.GetCollection<Users>("users").FindAll();
        return Ok(users);
    }


    [HttpPost("create")]
    public async Task<ActionResult<List<Users>>> CreateUser(Users user)
    {
        using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
        await connection.ExecuteAsync(
                "insert into users"+
                " values(@id, @login, @person_id, @password, @date_off, @date_on, @token, @queue, @is_blocked, @login_fail_count)", 
                user);
        return Ok(await SelectAllUsers(connection));
    }

    [HttpPost("createLiteDB")]
    public ActionResult<List<Users>> CreateUserLiteDB(Users user)
    {
        var litedb = new LiteDatabase("./Synchronization/data.db");
        ILiteCollection<Users> usersCol = litedb.GetCollection<Users>("users");
        usersCol.Insert(user);
        return Ok(usersCol.FindAll());
    }

    [HttpPut("update")]
    public async Task<ActionResult<Users>> UpdateUser(Users user)
    {
        using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
        await connection.ExecuteAsync(
                    "update users set"+
                    " id = @id, login = @login, person_id = @person_id, password = @password, date_off = @date_off,"+
                    " date_on = @date_on, token = @token, queue = @queue, is_blocked = @is_blocked,"+
                    " login_fail_count = @login_fail_count where id = @id", 
                    user);
        return Ok(await SelectAllUsers(connection));
    }

    [HttpPut("updateLiteDB")]
    public ActionResult<Users> UpdateUserLiteDB(Users user)
    {
        var litedb = new LiteDatabase("./Synchronization/data.db");
        ILiteCollection<Users> usersCol = litedb.GetCollection<Users>("users");
        usersCol.Update(user);
        return Ok(usersCol.FindAll());
    }

    [HttpDelete("delete")]
    public async Task<ActionResult<Users>> DeleteUser(Guid userId)
    {
        using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
        await connection.ExecuteAsync("delete from users where id = @Id", new {Id = userId});
        return Ok(await SelectAllUsers(connection));
    }

    [HttpDelete("deleteLiteDB")]
    public ActionResult<Users> DeleteUserLiteDB(Guid userId)
    {
        var litedb = new LiteDatabase("./Synchronization/data.db");
        ILiteCollection<Users> usersCol = litedb.GetCollection<Users>("users");
        usersCol.Delete(userId);
        return Ok(usersCol.FindAll());
    }

    private static async Task<IEnumerable<Users>> SelectAllUsers(NpgsqlConnection connection)
    {
        return await connection.QueryAsync<Users>("select * from users");
    }
}
