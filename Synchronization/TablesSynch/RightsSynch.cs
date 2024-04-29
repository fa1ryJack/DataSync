using Dapper;
using Npgsql;
using DataModel;
using CsvHelper;
using System.Globalization;
using LiteDB;

namespace Synchronization;

public class RightsSynch
{
    private readonly IConfiguration _config;
    public RightsSynch(IConfiguration config)
    {
        _config = config;
    }

    public void PgsqlToCsv()
    { 
        using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
        
        var rights =  connection.QueryAsync<Rights>("select * from rights").Result;
        using (var writer = new StreamWriter("./Synchronization/rights.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(rights);
        }
    }

    public void CsvToLitedb()
    {
        using var reader = new StreamReader("./Synchronization/rights.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        List<Rights> rights = csv.GetRecords<Rights>().ToList<Rights>();

        using(var litedb = new LiteDatabase("./Synchronization/data.db"))
        {
            var rightsCol = litedb.GetCollection<Rights>("rights");            
            
            foreach (Rights right in rightsCol.FindAll().Except(rights).ToList())
            {
                rightsCol.Delete(right.id);   
            }
                       
            foreach (Rights right in rights)
            {
                try
                {
                    rightsCol.Insert(right);
                }
                catch(LiteException e)
                { 
                    if (e.ErrorCode == 110)
                    {
                        rightsCol.Update(right);
                    }
                }
            }
                
        }
    }  


    public void LitedbToCsv()
    { 
        using var litedb = new LiteDatabase("./Synchronization/data.db");
        var rightsCol = litedb.GetCollection<Rights>("rights").FindAll();
        
        using (var writer = new StreamWriter("./Synchronization/rights.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(rightsCol);
        }
    }


    public async Task CsvToPgsql()
    {
        using var reader = new StreamReader("./Synchronization/rights.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        List<Rights> rights = csv.GetRecords<Rights>().ToList<Rights>();
        
        using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
        List<Rights> rightsPgsql =  connection.QueryAsync<Rights>("select * from rights").Result.ToList<Rights>();
        
        foreach (Rights right in rightsPgsql.Except(rights).ToList())
        {                
            await connection.ExecuteAsync(
            "delete from rights"+
            " where id = @id", 
            right);            
        }            

        foreach (Rights right in rights)
        {
            try
            {
                await connection.ExecuteAsync(
                "insert into rights"+
                " values(@id, @name, @alias, @date_off, @date_on, @owner, @order)", 
                right);
            }
            catch(NpgsqlException e)
            {
                if (e.ErrorCode == -2147467259)
                {
                    await connection.ExecuteAsync(
                    "update rights set"+
                    " id = @id, name = @name, alias = @alias, date_off = @date_off, date_on = @date_on,"+
                    " owner = @owner, order = @order where id = @id", 
                    right);
                }
            }
        }
    }
}