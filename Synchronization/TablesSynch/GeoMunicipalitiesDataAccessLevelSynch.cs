using Dapper;
using Npgsql;
using DataModel;
using CsvHelper;
using System.Globalization;
using LiteDB;

namespace Synchronization;

public class GeoMunicipalitiesDataAccessLevelSynch
{
    private readonly IConfiguration _config;
    public GeoMunicipalitiesDataAccessLevelSynch(IConfiguration config)
    {
        _config = config;
    }


    public void PgsqlToCsv()
    { 
        using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
        
        var geoMunicipalitiesDataAccessLevels =  connection.QueryAsync<GeoMunicipalitiesDataAccessLevel>("select * from geo_municipalities_data_access_level").Result;
        using (var writer = new StreamWriter("./Synchronization/geo_municipalities_data_access_level.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(geoMunicipalitiesDataAccessLevels);
        }
    }


    public void CsvToLitedb()
    {
        using var reader = new StreamReader("./Synchronization/geo_municipalities_data_access_level.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        List<GeoMunicipalitiesDataAccessLevel> geoMunicipalitiesDataAccessLevels = csv.GetRecords<GeoMunicipalitiesDataAccessLevel>().ToList<GeoMunicipalitiesDataAccessLevel>();

        using(var litedb = new LiteDatabase("./Synchronization/data.db"))
        {
            var geoMunicipalitiesDataAccessLevelsCol = litedb.GetCollection<GeoMunicipalitiesDataAccessLevel>("geo_municipalities_data_access_level");
                        
            foreach (GeoMunicipalitiesDataAccessLevel geoMunicipalitiesDataAccessLevel in geoMunicipalitiesDataAccessLevelsCol.FindAll().Except(geoMunicipalitiesDataAccessLevels).ToList())
            {
                    
                geoMunicipalitiesDataAccessLevelsCol.Delete(geoMunicipalitiesDataAccessLevel.id);
                    
            }           
            
            foreach (GeoMunicipalitiesDataAccessLevel geoMunicipalitiesDataAccessLevel in geoMunicipalitiesDataAccessLevels)
            {
                try
                {
                    geoMunicipalitiesDataAccessLevelsCol.Insert(geoMunicipalitiesDataAccessLevel);
                }
                catch(LiteException e)
                { 
                    if (e.ErrorCode == 110)
                    {
                        geoMunicipalitiesDataAccessLevelsCol.Update(geoMunicipalitiesDataAccessLevel);
                    }
                }
            }
                
        }
    } 


    public void LitedbToCsv()
    { 
        using var litedb = new LiteDatabase("./Synchronization/data.db");
        var geoMunicipalitiesDataAccessLevelsCol = litedb.GetCollection<GeoMunicipalitiesDataAccessLevel>("geo_municipalities_data_access_level").FindAll();
        
        using (var writer = new StreamWriter("./Synchronization/geo_municipalities_data_access_level.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(geoMunicipalitiesDataAccessLevelsCol);
        }
    }


     public async Task CsvToPgsql()
    {
        using var reader = new StreamReader("./Synchronization/geo_municipalities_data_access_level.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        List<GeoMunicipalitiesDataAccessLevel> geoMunicipalitiesDataAccessLevels = csv.GetRecords<GeoMunicipalitiesDataAccessLevel>().ToList<GeoMunicipalitiesDataAccessLevel>();
        
        using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
        List<GeoMunicipalitiesDataAccessLevel> geoMunicipalitiesDataAccessLevelsPgsql =  connection.QueryAsync<GeoMunicipalitiesDataAccessLevel>("select * from geo_municipalities_data_access_level").Result.ToList<GeoMunicipalitiesDataAccessLevel>();

        foreach (GeoMunicipalitiesDataAccessLevel geoMunicipalitiesDataAccessLevel in geoMunicipalitiesDataAccessLevelsPgsql.Except(geoMunicipalitiesDataAccessLevels).ToList())
        { 
            await connection.ExecuteAsync(
            "delete from geo_municipalities_data_access_level"+
            " where id = @id", 
            geoMunicipalitiesDataAccessLevel);
        }

        foreach (GeoMunicipalitiesDataAccessLevel geoMunicipalitiesDataAccessLevel in geoMunicipalitiesDataAccessLevels)
        {
            try
            {
                await connection.ExecuteAsync(
                "insert into geo_municipalities_data_access_level"+
                " values(@id, @geo_municipality_id, @data_access_level_id, @date_off)", 
                geoMunicipalitiesDataAccessLevel);
            }
            catch(NpgsqlException e)
            {
                if (e.ErrorCode == -2147467259)
                {
                    await connection.ExecuteAsync(
                    "update geo_municipalities_data_access_level set"+
                    " id = @id, geo_municipality_id = @geo_municipality_id, data_access_level_id = @data_access_level_id, "+
                    "date_off = @date_off where id = @id", 
                    geoMunicipalitiesDataAccessLevel);
                }
            }
        }
    }
}