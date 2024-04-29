using Dapper;
using Npgsql;
using DataModel;
using CsvHelper;
using System.Globalization;
using LiteDB;
using DataForLiteDB;

namespace Synchronization;

public class RoleRightsSynch
{
    private readonly IConfiguration _config;
    public RoleRightsSynch(IConfiguration config)
    {
        _config = config;
    }

    public void PgsqlToCsv()
    { 
        using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
        
        var roleRights =  connection.QueryAsync<RoleRights>("select * from role_rights").Result;
        using (var writer = new StreamWriter("./Synchronization/role_rights.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(roleRights);
        }
    }


    public void CsvToLitedb()
    {
        using var reader = new StreamReader("./Synchronization/role_rights.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        List<RoleRights> roleRights = csv.GetRecords<RoleRights>().ToList<RoleRights>();

        using(var litedb = new LiteDatabase("./Synchronization/data.db"))
        {
            var roleRightsCol = litedb.GetCollection<RoleRightsLite>("role_rights");
            
            
                foreach (RoleRightsLite roleRight in roleRightsCol.FindAll())
                {
                    RoleRights RR = new RoleRights{role_id = roleRight.RoleId, right_id = roleRight.RightId, date_off = roleRight.DateOff, date_on = roleRight.DateOn};
                    if(!roleRights.Contains(RR))
                    {
                        roleRightsCol.Delete(roleRight.Id);
                    }
                }
            
            
            foreach (RoleRights roleRight in roleRights)
            {RoleRightsLite RR = new RoleRightsLite{Id = roleRight.role_id.ToString()+roleRight.right_id, RoleId = roleRight.role_id, RightId = roleRight.right_id, DateOff = roleRight.date_off, DateOn = roleRight.date_on};
                    
                try
                {
                    roleRightsCol.Insert(RR);
                }
                catch(LiteException e)
                { 
                    if (e.ErrorCode == 110)
                    {
                        roleRightsCol.Update(RR);
                    }
                }
            }
                
        }
    }    



    public void LitedbToCsv()
    { 
        using var litedb = new LiteDatabase("./Synchronization/data.db");
        var roleRightsCol = litedb.GetCollection<RoleRightsLite>("role_rights").FindAll();

        List<RoleRights> roleRights = new List<RoleRights>();
        foreach (RoleRightsLite RRL in roleRightsCol)
        {
            roleRights.Add(new RoleRights{right_id = RRL.RightId, role_id = RRL.RoleId, date_off = RRL.DateOff, date_on = RRL.DateOn});
        }

        using (var writer = new StreamWriter("./Synchronization/role_rights.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(roleRights);
        }
    }

    public async Task CsvToPgsql()
    {
        using var reader = new StreamReader("./Synchronization/role_rights.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        List<RoleRights> roleRights = csv.GetRecords<RoleRights>().ToList<RoleRights>();
        
        using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
        List<RoleRights> roleRightsPgsql =  connection.QueryAsync<RoleRights>("select * from role_rights").Result.ToList<RoleRights>();

        
                foreach (RoleRights roleRight in roleRightsPgsql.Except(roleRights).ToList())
                {
                    await connection.ExecuteAsync(
                    "delete from role_rights"+
                    " where role_id = @role_id and right_id = @right_id", 
                    roleRight);
                }
            

        foreach (RoleRights roleRight in roleRights)
        {
            try
            {
                await connection.ExecuteAsync(
                "insert into role_rights"+
                " values(@role_id, @right_id, @date_off, @date_on)", 
                roleRight);
            }
            catch(NpgsqlException e)
            {
                if (e.ErrorCode == -2147467259)
                {
                    await connection.ExecuteAsync(
                    "update role_rights set"+
                    " role_id = @role_id, right_id = @right_id, date_off = @date_off, date_on = @date_on"+
                    " where role_id = @role_id and right_id = @right_id", 
                    roleRight);
                }
            }
        }
    }
}