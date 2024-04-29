using Dapper;
using Npgsql;
using DataModel;
using CsvHelper;
using System.Globalization;
using LiteDB;

namespace Synchronization;

public class RightGroupsSynch
{
    private readonly IConfiguration _config;
    public RightGroupsSynch(IConfiguration config)
    {
        _config = config;
    }
    
    public void PgsqlToCsv()
    { 
        using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
        
        var rightGroups =  connection.QueryAsync<RightGroups>("select * from right_groups").Result;
        using (var writer = new StreamWriter("./Synchronization/right_groups.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(rightGroups);
        }
    }


    public void CsvToLitedb()
    {
        using var reader = new StreamReader("./Synchronization/right_groups.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        List<RightGroups> rightGroups = csv.GetRecords<RightGroups>().ToList<RightGroups>();

        using(var litedb = new LiteDatabase("./Synchronization/data.db"))
        {
            var rightGroupsCol = litedb.GetCollection<RightGroups>("right_groups");

            foreach (RightGroups rightGroup in rightGroupsCol.FindAll().Except(rightGroups).ToList())
            {  
                rightGroupsCol.Delete(rightGroup.id);
            }
                        
            foreach (RightGroups rightGroup in rightGroups)
            {
                try
                {
                    rightGroupsCol.Insert(rightGroups);
                }
                catch(LiteException e)
                { 
                    if (e.ErrorCode == 110)
                    {
                        rightGroupsCol.Update(rightGroups);
                    }
                }
            }
                
        }
    }


     public void LitedbToCsv()
    { 
        using var litedb = new LiteDatabase("./Synchronization/data.db");
        var rightGroupsCol = litedb.GetCollection<RightGroups>("right_groups").FindAll();
        
        using (var writer = new StreamWriter("./Synchronization/right_groups.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(rightGroupsCol);
        }
    }        

    public async Task CsvToPgsql()
    {
        using var reader = new StreamReader("./Synchronization/right_groups.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        List<RightGroups> rightGroups = csv.GetRecords<RightGroups>().ToList<RightGroups>();
        
        using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
        List<RightGroups> rightGroupsPgsql =  connection.QueryAsync<RightGroups>("select * from right_groups").Result.ToList<RightGroups>();
        
        foreach (RightGroups rightGroup in rightGroupsPgsql.Except(rightGroups).ToList())
        {
            await connection.ExecuteAsync(
             "delete from right_groups"+
            " where id = @id", 
            rightGroup);
        }
            
        foreach (RightGroups rightGroup in rightGroups)
        {
            try
            {
                await connection.ExecuteAsync(
                "insert into right_groups"+
                " values(@id, @order, @name, @date_off, @date_on)", 
                rightGroup);
            }
            catch(NpgsqlException e)
            {
                if (e.ErrorCode == -2147467259)
                {
                    await connection.ExecuteAsync(
                    "update right_groups set"+
                    " id = @id, order = @order, name = @name, date_off = @date_off, date_on = @date_on where id = @id", 
                    rightGroup);
                }
            }
        }
    }
}