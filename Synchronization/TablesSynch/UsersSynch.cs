using Dapper;
using Npgsql;
using DataModel;
using CsvHelper;
using System.Globalization;
using LiteDB;

namespace Synchronization;

public class UsersSynch
{
    private readonly IConfiguration _config;
    public UsersSynch(IConfiguration config)
    {
        _config = config;
    }
    

    public void PgsqlToCsv()
    { 
        using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
        
        var users =  connection.QueryAsync<Users>("select * from users").Result;
        using (var writer = new StreamWriter("./Synchronization/users.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(users);
        }
    }


    public void CsvToLitedb()
    {
        using var reader = new StreamReader("./Synchronization/users.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        List<Users> users = csv.GetRecords<Users>().ToList<Users>();

        using(var litedb = new LiteDatabase("./Synchronization/data.db"))
        {
            var usersCol = litedb.GetCollection<Users>("users");
            
            foreach (Users user in usersCol.FindAll().Except(users).ToList())
            {
                usersCol.Delete(user.id);
            }
                
            
            
            foreach (Users user in users)
            {
                try
                {
                    usersCol.Insert(user);
                }
                catch(LiteException e)
                { 
                    if (e.ErrorCode == 110)
                    {
                        usersCol.Update(user);
                    }
                }
            }
                
        }
    }        
    

    public void LitedbToCsv()
    { 
        using var litedb = new LiteDatabase("./Synchronization/data.db");
        var usersCol = litedb.GetCollection<Users>("users").FindAll();
        
        using (var writer = new StreamWriter("./Synchronization/users.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(usersCol);
        }
    }
 
 
    public async Task CsvToPgsql()
    {
        using var reader = new StreamReader("./Synchronization/users.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        List<Users> users = csv.GetRecords<Users>().ToList();
        
        using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
        List<Users> usersPgsql =  connection.QueryAsync<Users>("select * from users").Result.ToList();

        
                foreach (Users user in usersPgsql.Except(users).ToList())
                {
                    await connection.ExecuteAsync(
                    "delete from users"+
                    " where id = @id", 
                    user);
                }
            

        foreach (Users user in users)
        { 
            try
            {
                await connection.ExecuteAsync(
                "insert into users"+
                " values(@id, @login, @person_id, @password, @date_off, @date_on, @token, @queue, @is_blocked, @login_fail_count)", 
                user);
            }
            catch(NpgsqlException e)
            {
                if (e.ErrorCode == -2147467259)
                {
                    await connection.ExecuteAsync(
                    "update users set"+
                    " id = @id, login = @login, person_id = @person_id, password = @password, date_off = @date_off,"+
                    " date_on = @date_on, token = @token, queue = @queue, is_blocked = @is_blocked,"+
                    " login_fail_count = @login_fail_count where id = @id", 
                    user);
                }
            }
        }
    }
}