using Dapper;
using Npgsql;
using DataModel;
using CsvHelper;
using System.Globalization;
using LiteDB;

namespace Synchronization;

public class ExteranalObjectsSynch
{
private readonly IConfiguration _config;
    public ExteranalObjectsSynch(IConfiguration config)
    {
        _config = config;
    }
    

    public void PgsqlToCsv()
    { 
        using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
        
        var exteranalObjects =  connection.QueryAsync<ExteranalObjects>("select * from external_objects").Result;
        using (var writer = new StreamWriter("./Synchronization/external_objects.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(exteranalObjects);
        }
    }


    public void CsvToLitedb()
    {
        using var reader = new StreamReader("./Synchronization/external_objects.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        List<ExteranalObjects> exteranalObjects = csv.GetRecords<ExteranalObjects>().ToList<ExteranalObjects>();

        using(var litedb = new LiteDatabase("./Synchronization/data.db"))
        {
            var exteranalObjectsCol = litedb.GetCollection<ExteranalObjects>("external_objects");

            foreach (ExteranalObjects exteranalObject in exteranalObjectsCol.FindAll().Except(exteranalObjects).ToList())
            {
                exteranalObjectsCol.Delete(exteranalObject.id);
            }
            
            
            foreach (ExteranalObjects exteranalObject in exteranalObjects)
            {
                try
                {
                    exteranalObjectsCol.Insert(exteranalObject);
                }
                catch(LiteException e)
                { 
                    if (e.ErrorCode == 110)
                    {
                        exteranalObjectsCol.Update(exteranalObject);
                    }
                }
            }
                
        }
    }  


    public void LitedbToCsv()
    { 
        using var litedb = new LiteDatabase("./Synchronization/data.db");
        var exteranalObjectsCol = litedb.GetCollection<ExteranalObjects>("external_objects").FindAll();
        
        using (var writer = new StreamWriter("./Synchronization/external_objects.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(exteranalObjectsCol);
        }
    }


     public async Task CsvToPgsql()
    {
        using var reader = new StreamReader("./Synchronization/external_objects.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        List<ExteranalObjects> exteranalObjects = csv.GetRecords<ExteranalObjects>().ToList<ExteranalObjects>();
        
        using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
        List<ExteranalObjects> exteranalObjectsPgsql =  connection.QueryAsync<ExteranalObjects>("select * from external_objects").Result.ToList<ExteranalObjects>();

        
        foreach (ExteranalObjects exteranalObject in exteranalObjectsPgsql.Except(exteranalObjects).ToList())
        {    
            await connection.ExecuteAsync(
            "delete from external_objects"+
            " where id = @id", 
            exteranalObject);
        }
            

        foreach (ExteranalObjects exteranalObject in exteranalObjects)
        {
            try
            {
                await connection.ExecuteAsync(
                "insert into external_objects"+
                " values(@id, @date_on, @object_id, @system_id, @external_id, @hash, @date_off)", 
                exteranalObject);
            }
            catch(NpgsqlException e)
            {
                if (e.ErrorCode == -2147467259)
                {
                    await connection.ExecuteAsync(
                    "update external_objects set"+
                    " id = @id, date_on = @date_on, object_id = @object_id, system_id = @system_id, "+
                    "external_id = @external_id, hash = @hash, date_off = @date_off where id = @id", 
                    exteranalObject);
                }
            }
        }
    }
}