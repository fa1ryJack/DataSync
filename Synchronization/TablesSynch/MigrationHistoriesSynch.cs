using Dapper;
using Npgsql;
using DataModel;
using CsvHelper;
using System.Globalization;
using LiteDB;

namespace Synchronization;

public class MigrationHistoriesSynch
{
    private readonly IConfiguration _config;
    public MigrationHistoriesSynch(IConfiguration config)
    {
        _config = config;
    }


    public void PgsqlToCsv()
    { 
        using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
        
        var migrationHistorys =  connection.QueryAsync<MigrationHistories>("select * from _migration_history").Result;
        using (var writer = new StreamWriter("./Synchronization/_migration_history.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(migrationHistorys);
        }
    }


    public void CsvToLitedb()
    {
        using var reader = new StreamReader("./Synchronization/_migration_history.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        List<MigrationHistories> migrationHistorys = csv.GetRecords<MigrationHistories>().ToList<MigrationHistories>();

        using(var litedb = new LiteDatabase("./Synchronization/data.db"))
        {
            var migrationHistorysCol = litedb.GetCollection<MigrationHistories>("_migration_history");
                        
            foreach (MigrationHistories migrationHistory in migrationHistorysCol.FindAll().Except(migrationHistorys).ToList())
            {
                migrationHistorysCol.Delete(migrationHistory.id);       
            }            
            
            foreach (MigrationHistories migrationHistory in migrationHistorys)
            {
                try
                {
                    migrationHistorysCol.Insert(migrationHistory);
                }
                catch(LiteException e)
                { 
                    if (e.ErrorCode == 110)
                    {
                        migrationHistorysCol.Update(migrationHistory);
                    }
                }
            }
                
        }
    }        


    public void LitedbToCsv()
    { 
        using var litedb = new LiteDatabase("./Synchronization/data.db");
        var migrationHistorysCol = litedb.GetCollection<MigrationHistories>("_migration_history").FindAll();
        
        using (var writer = new StreamWriter("./Synchronization/_migration_history.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(migrationHistorysCol);
        }
    }


    public async Task CsvToPgsql()
    {
        using var reader = new StreamReader("./Synchronization/_migration_history.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        List<MigrationHistories> migrationHistories = csv.GetRecords<MigrationHistories>().ToList<MigrationHistories>();
        
        using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
        List<MigrationHistories> migrationHistoriesPgsql =  connection.QueryAsync<MigrationHistories>("select * from _migration_history").Result.ToList<MigrationHistories>();
        
        foreach (MigrationHistories migrationHistory in migrationHistoriesPgsql.Except(migrationHistories).ToList())
        {                   
            await connection.ExecuteAsync(
            "delete from _migration_history"+
            " where id = @id", 
            migrationHistory);    
        }            

        foreach (MigrationHistories migrationHistory in migrationHistories)
        {
            try
            {
                await connection.ExecuteAsync(
                "insert into _migration_history"+
                " values(@id, @script_name, @add_date)", 
                migrationHistory);
            }
            catch(NpgsqlException e)
            {
                if (e.ErrorCode == -2147467259)
                {
                    await connection.ExecuteAsync(
                    "update _migration_history set"+
                    " id = @id, script_name = @script_name, add_date = @add_date where id = @id", 
                    migrationHistory);
                }
            }
        }
    }
}