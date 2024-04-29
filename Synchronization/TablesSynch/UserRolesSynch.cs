using Dapper;
using Npgsql;
using DataModel;
using CsvHelper;
using System.Globalization;
using LiteDB;
using DataForLiteDB;

namespace Synchronization;

public class UserRolesSynch
{
    private readonly IConfiguration _config;
    public UserRolesSynch(IConfiguration config)
    {
        _config = config;
    }

    public void PgsqlToCsv()
    { 
        using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
        
        var userRoles =  connection.QueryAsync<UserRoles>("select * from user_roles").Result;
        using (var writer = new StreamWriter("./Synchronization/user_roles.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(userRoles);
        }
    }


    public void CsvToLitedb()
    {
        using var reader = new StreamReader("./Synchronization/user_roles.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        
        List<UserRoles> userRoles = csv.GetRecords<UserRoles>().ToList<UserRoles>();
        
        using(var litedb = new LiteDatabase("./Synchronization/data.db"))
        {
            var userRolesCol = litedb.GetCollection<UserRolesLite>("user_roles");
            
            
                foreach (UserRolesLite userRole in userRolesCol.FindAll())
                {
                    UserRoles UR = new UserRoles{role_id = userRole.RoleId, user_id = userRole.UserId, date_off = userRole.DateOff, date_on = userRole.DateOn};
                    if (!userRoles.Contains(UR))
                    {   
                        userRolesCol.Delete(userRole.Id);
                    }
                }
            
            
            foreach (UserRoles userRole in userRoles)
            {UserRolesLite UR = new UserRolesLite{Id = userRole.role_id.ToString()+userRole.user_id, RoleId = userRole.role_id, UserId = userRole.user_id, DateOff = userRole.date_off, DateOn = userRole.date_on};
                    
                try
                {
                    userRolesCol.Insert(UR);
                }
                catch(LiteException e)
                { 
                    if (e.ErrorCode == 110)
                    {
                        userRolesCol.Update(UR);
                    }
                }
            }
                
        }
    }  



    public void LitedbToCsv()
    { 
        using var litedb = new LiteDatabase("./Synchronization/data.db");
        var userRolesCol = litedb.GetCollection<UserRolesLite>("user_roles").FindAll();

        List<UserRoles> userRoles = new List<UserRoles>();
        foreach (UserRolesLite URL in userRolesCol)
        { 
            userRoles.Add(new UserRoles{user_id = URL.UserId, role_id = URL.RoleId, date_off = URL.DateOff, date_on = URL.DateOn});
        }
        using (var writer = new StreamWriter("./Synchronization/user_roles.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(userRoles);
        }
    }

    public async Task CsvToPgsql()
    {
        using var reader = new StreamReader("./Synchronization/user_roles.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        List<UserRoles> userRoles = csv.GetRecords<UserRoles>().ToList<UserRoles>();
        
        using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
        List<UserRoles> userRolesPgsql =  connection.QueryAsync<UserRoles>("select * from user_roles").Result.ToList<UserRoles>();

        
                foreach (UserRoles userRole in userRolesPgsql.Except(userRoles).ToList())
                {
                    await connection.ExecuteAsync(
                    "delete from user_roles"+
                    " where role_id = @role_id and user_id = @user_id", 
                    userRole);

                }
            

        foreach (UserRoles userRole in userRoles)
        {
            try
            {
                await connection.ExecuteAsync(
                "insert into user_roles"+
                " values(@role_id, @user_id, @date_off, @date_on)", 
                userRole);
            }
            catch(NpgsqlException e)
            {
                if (e.ErrorCode == -2147467259)
                {
                    await connection.ExecuteAsync(
                    "update user_roles set"+
                    " role_id = @role_id, user_id = @user_id, date_off = @date_off, date_on = @date_on where role_id = @role_id and user_id = @user_id", 
                    userRole);
                }
            }
        }
    }
}