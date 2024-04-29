using Dapper;
using Npgsql;
using DataModel;
using CsvHelper;
using System.Globalization;
using LiteDB;

namespace Synchronization;

public class EnterpriseDataAccessLevelSynch
{
    private readonly IConfiguration _config;
    public EnterpriseDataAccessLevelSynch(IConfiguration config)
    {
        _config = config;
    }

    public void PgsqlToCsv()
    { 
        using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
        
        var enterpriseDataAccessLevels =  connection.QueryAsync<EnterpriseDataAccessLevel>("select * from enterprise_data_access_level").Result;
        using (var writer = new StreamWriter("./Synchronization/enterprise_data_access_level.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(enterpriseDataAccessLevels);
        }
    }


    public void CsvToLitedb()
    {
        using var reader = new StreamReader("./Synchronization/enterprise_data_access_level.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        List<EnterpriseDataAccessLevel> enterpriseDataAccessLevels = csv.GetRecords<EnterpriseDataAccessLevel>().ToList<EnterpriseDataAccessLevel>();

        using(var litedb = new LiteDatabase("./Synchronization/data.db"))
        {
            var enterpriseDataAccessLevelsCol = litedb.GetCollection<EnterpriseDataAccessLevel>("enterprise_data_access_level");
            
            
                foreach (EnterpriseDataAccessLevel enterpriseDataAccessLevel in enterpriseDataAccessLevelsCol.FindAll().Except(enterpriseDataAccessLevels).ToList())
                {
 
                    enterpriseDataAccessLevelsCol.Delete(enterpriseDataAccessLevel.id);
                    
                }
            
            
            foreach (EnterpriseDataAccessLevel enterpriseDataAccessLevel in enterpriseDataAccessLevels)
            {
                try
                {
                    enterpriseDataAccessLevelsCol.Insert(enterpriseDataAccessLevel);
                }
                catch(LiteException e)
                { 
                    if (e.ErrorCode == 110)
                    {
                        enterpriseDataAccessLevelsCol.Update(enterpriseDataAccessLevel);
                    }
                }
            }
                
        }
    }


    public void LitedbToCsv()
    { 
        using var litedb = new LiteDatabase("./Synchronization/data.db");
        var enterpriseDataAccessLevelsCol = litedb.GetCollection<EnterpriseDataAccessLevel>("enterprise_data_access_level").FindAll();
        
        using (var writer = new StreamWriter("./Synchronization/enterprise_data_access_level.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(enterpriseDataAccessLevelsCol);
        }
    }


    public async Task CsvToPgsql()
    {
        using var reader = new StreamReader("./Synchronization/enterprise_data_access_level.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        List<EnterpriseDataAccessLevel> enterpriseDataAccessLevels = csv.GetRecords<EnterpriseDataAccessLevel>().ToList<EnterpriseDataAccessLevel>();
        
        using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
        List<EnterpriseDataAccessLevel> enterpriseDataAccessLevelsPgsql =  connection.QueryAsync<EnterpriseDataAccessLevel>("select * from enterprise_data_access_level").Result.ToList<EnterpriseDataAccessLevel>();

        
                foreach (EnterpriseDataAccessLevel enterpriseDataAccessLevel in enterpriseDataAccessLevelsPgsql.Except(enterpriseDataAccessLevels).ToList())
                { 
                        await connection.ExecuteAsync(
                        "delete from enterprise_data_access_level"+
                        " where id = @id", 
                        enterpriseDataAccessLevel);
                }
            

        foreach (EnterpriseDataAccessLevel enterpriseDataAccessLevel in enterpriseDataAccessLevels)
        {
            try
            {
                await connection.ExecuteAsync(
                "insert into enterprise_data_access_level"+
                " values(@id, @date_on, @enterprise_id, @data_access_level_id, @date_off)", 
                enterpriseDataAccessLevel);
            }
            catch(NpgsqlException e)
            {
                if (e.ErrorCode == -2147467259)
                {
                    await connection.ExecuteAsync(
                    "update enterprise_data_access_level set"+
                    " id = @id, date_on = @date_on, enterprise_id = @enterprise_id, data_access_level_id = @data_access_level_id, date_off = @date_off where id = @id", 
                    enterpriseDataAccessLevel);
                }
            }
        }
    }
}