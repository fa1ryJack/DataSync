using Dapper;
using Npgsql;
using DataModel;
using CsvHelper;
using System.Globalization;
using LiteDB;

namespace Synchronization;

public class GeoMunicipalitiesSynch
{
    private readonly IConfiguration _config;
    public GeoMunicipalitiesSynch(IConfiguration config)
    {
        _config = config;
    }

    public void PgsqlToCsv()
    { 
        using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
        
        var geoMunicipalities =  connection.QueryAsync<GeoMunicipalities>("select * from geo_municipalities").Result;
        using (var writer = new StreamWriter("./Synchronization/geo_municipalities.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(geoMunicipalities);
        }
    }


    public void CsvToLitedb()
    {
        using var reader = new StreamReader("./Synchronization/geo_municipalities.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        List<GeoMunicipalities> geoMunicipalities = csv.GetRecords<GeoMunicipalities>().ToList<GeoMunicipalities>();

        using(var litedb = new LiteDatabase("./Synchronization/data.db"))
        {
            var geoMunicipalitiesCol = litedb.GetCollection<GeoMunicipalities>("geo_municipalities");
            
            foreach (GeoMunicipalities geoMunicipality in geoMunicipalitiesCol.FindAll().Except(geoMunicipalities).ToList())
            {
                    
                geoMunicipalitiesCol.Delete(geoMunicipality.id);
                    
            }
            
            foreach (GeoMunicipalities geoMunicipality in geoMunicipalities)
            {
                try
                {
                    geoMunicipalitiesCol.Insert(geoMunicipality);
                }
                catch(LiteException e)
                { 
                    if (e.ErrorCode == 110)
                    {
                        geoMunicipalitiesCol.Update(geoMunicipality);
                    }
                }
            }
                
        }
    }    


    public void LitedbToCsv()
    { 
        using var litedb = new LiteDatabase("./Synchronization/data.db");
        var geoMunicipalitiesCol = litedb.GetCollection<GeoMunicipalities>("geo_municipalities").FindAll();
        
        using (var writer = new StreamWriter("./Synchronization/geo_municipalities.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(geoMunicipalitiesCol);
        }
    }

    public async Task CsvToPgsql()
    {
        using var reader = new StreamReader("./Synchronization/geo_municipalities.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        List<GeoMunicipalities> geoMunicipalities = csv.GetRecords<GeoMunicipalities>().ToList<GeoMunicipalities>();
        
        using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
        List<GeoMunicipalities> geoMunicipalitiesPgsql =  connection.QueryAsync<GeoMunicipalities>("select * from geo_municipalities").Result.ToList<GeoMunicipalities>();

        foreach (GeoMunicipalities GeoMunicipality in geoMunicipalitiesPgsql.Except(geoMunicipalities).ToList())
        {  
            await connection.ExecuteAsync(
            "delete from geo_municipalities"+
            " where id = @id", 
            GeoMunicipality);
        }
            
        foreach (GeoMunicipalities GeoMunicipality in geoMunicipalities)
        {
            try
            {
                await connection.ExecuteAsync(
                "insert into geo_municipalities"+
                " values(@id, @coordinates, @name, @date_off, @level)", 
                GeoMunicipality);
            }
            catch(NpgsqlException e)
            {
                if (e.ErrorCode == -2147467259)
                {
                    await connection.ExecuteAsync(
                    "update geo_municipalities set"+
                    " id = @id, coordinates = @coordinates, name = @name, date_off = @date_off, level = @level where id = @id", 
                    GeoMunicipality);
                }
            }
        }
    }
}