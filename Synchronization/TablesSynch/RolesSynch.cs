using Dapper;
using Npgsql;
using DataModel;
using CsvHelper;
using System.Globalization;
using LiteDB;

namespace Synchronization;

public class RolesSynch
{
    private readonly IConfiguration _config;
    public RolesSynch(IConfiguration config)
    {
        _config = config;
    }


    public void PgsqlToCsv()
    { 
        using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
        
        var roles =  connection.QueryAsync<Roles>("select * from roles").Result;
        using (var writer = new StreamWriter("./Synchronization/roles.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(roles);
        }
    }

    public void CsvToLitedb()
    {
        using var reader = new StreamReader("./Synchronization/roles.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        List<Roles> roles = csv.GetRecords<Roles>().ToList<Roles>();

        using(var litedb = new LiteDatabase("./Synchronization/data.db"))
        {
            var rolesCol = litedb.GetCollection<Roles>("roles");
            
            foreach (Roles role in rolesCol.FindAll().Except(roles).ToList())
            {
               
                rolesCol.Delete(role.id);
                    
            }
            
            foreach (Roles role in roles)
            {
                try
                {
                    rolesCol.Insert(role);
                }
                catch(LiteException e)
                { 
                    if (e.ErrorCode == 110)
                    {
                        rolesCol.Update(role);
                    }
                }
            }
                
        }
    }   


    public void LitedbToCsv()
    { 
        using var litedb = new LiteDatabase("./Synchronization/data.db");
        var rolesCol = litedb.GetCollection<Roles>("roles").FindAll();
        
        using (var writer = new StreamWriter("./Synchronization/roles.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(rolesCol);
        }
    }


    public async Task CsvToPgsql()
    {
        using var reader = new StreamReader("./Synchronization/roles.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        List<Roles> roles = csv.GetRecords<Roles>().ToList<Roles>();
        
        using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
        List<Roles> rolesPgsql =  connection.QueryAsync<Roles>("select * from roles").Result.ToList<Roles>();

        foreach (Roles role in rolesPgsql.Except(roles).ToList())
        {    
            await connection.ExecuteAsync(
            "delete from roles"+
            " where id = @id", 
            role);
        }           

        foreach (Roles role in roles)
        {
            try
            {
                await connection.ExecuteAsync(
                "insert into roles"+
                " values(@id, @name, @date_off, @date_on)", 
                role);
            }
            catch(NpgsqlException e)
            {
                if (e.ErrorCode == -2147467259)
                {
                    await connection.ExecuteAsync(
                    "update roles set"+
                    " id = @id, name = @name, date_off = @date_off, date_on = @date_on, where id = @id", 
                    role);
                }
            }
        }
    }
}