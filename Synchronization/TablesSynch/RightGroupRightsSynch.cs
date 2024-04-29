using Dapper;
using Npgsql;
using DataModel;
using CsvHelper;
using System.Globalization;
using LiteDB;
using DataForLiteDB;

namespace Synchronization;

public class RightGroupRightsSynch
{
    private readonly IConfiguration _config;
    public RightGroupRightsSynch(IConfiguration config)
    {
        _config = config;
    }

    public void PgsqlToCsv()
    { 
        using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
        
        var rightGroupRights =  connection.QueryAsync<RightGroupRights>("select * from right_group_rights").Result;
        using (var writer = new StreamWriter("./Synchronization/right_group_rights.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(rightGroupRights);
        }
    }


    public void CsvToLitedb()
    {
        using var reader = new StreamReader("./Synchronization/right_group_rights.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        List<RightGroupRights> rightGroupRights = csv.GetRecords<RightGroupRights>().ToList<RightGroupRights>();

        using(var litedb = new LiteDatabase("./Synchronization/data.db"))
        {
            var rightGroupRightsCol = litedb.GetCollection<RightGroupRightLite>("right_group_rights");
            
            
                foreach (RightGroupRightLite rightGroupRight in rightGroupRightsCol.FindAll())
                {
                    RightGroupRights RGR = new RightGroupRights{right_group_id = rightGroupRight.RightGroupId, right_id = rightGroupRight.RightId};
                    
                    if (!rightGroupRights.Contains(RGR))
                    {   
                        rightGroupRightsCol.Delete(rightGroupRight.Id);
                    }
                }
            
            
            foreach (RightGroupRights rightGroupRight in rightGroupRights)
            {RightGroupRightLite RGR = new RightGroupRightLite{Id = rightGroupRight.right_group_id.ToString()+rightGroupRight.right_id,RightGroupId = rightGroupRight.right_group_id, RightId = rightGroupRight.right_id};
                    
                try
                {
                    rightGroupRightsCol.Insert(RGR);
                }
                catch(LiteException e)
                { 
                    if (e.ErrorCode == 110)
                    {
                        rightGroupRightsCol.Update(RGR);
                    }
                }
            }
                
        }
    }    


    public void LitedbToCsv()
    { 
        using var litedb = new LiteDatabase("./Synchronization/data.db");
        var rightGroupRightsCol = litedb.GetCollection<RightGroupRightLite>("right_group_rights").FindAll();

        List<RightGroupRights> rightGroupRights = new List<RightGroupRights>();
        foreach (RightGroupRightLite RGRL in rightGroupRightsCol)
        {
            rightGroupRights.Add(new RightGroupRights{right_id = RGRL.RightId, right_group_id = RGRL.RightGroupId});
        }

        using (var writer = new StreamWriter("./Synchronization/right_group_rights.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(rightGroupRights);
        }
    }


    public async Task CsvToPgsql()
    {
        using var reader = new StreamReader("./Synchronization/right_group_rights.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        List<RightGroupRights> rightGroupRights = csv.GetRecords<RightGroupRights>().ToList<RightGroupRights>();
        
        using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
        List<RightGroupRights> rightGroupRightsPgsql =  connection.QueryAsync<RightGroupRights>("select * from right_group_rights").Result.ToList<RightGroupRights>();

        
                foreach (RightGroupRights rightGroupRight in rightGroupRightsPgsql.Except(rightGroupRights).ToList())
                {
                    await connection.ExecuteAsync(
                    "delete from right_group_rights"+
                    " where right_id = @right_id and right_group_id = @right_group_id", 
                    rightGroupRight);
                }
            

        foreach (RightGroupRights rightGroupRight in rightGroupRights)
        {
            try
            {
                await connection.ExecuteAsync(
                "insert into right_group_rights"+
                " values(@right_id, @right_group_id)", 
                rightGroupRight);
            }
            catch(NpgsqlException e)
            {
                if (e.ErrorCode == -2147467259)
                {
                    await connection.ExecuteAsync(
                    "update right_group_rights set"+
                    " right_id = @right_id, right_group_id = @right_group_id where right_id = @right_id and right_group_id = @right_group_id", 
                    rightGroupRight);
                }
            }
        }
    }
}