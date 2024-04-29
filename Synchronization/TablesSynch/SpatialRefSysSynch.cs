using Dapper;
using Npgsql;
using DataModel;
using CsvHelper;
using System.Globalization;
using LiteDB;

namespace Synchronization;

public class SpatialRefSysSynch
{
    private readonly IConfiguration _config;
    public SpatialRefSysSynch(IConfiguration config)
    {
        _config = config;
    }


    public void PgsqlToCsv()
    { 
        using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
        
        var spatialRefSyss =  connection.QueryAsync<SpatialRefSys>("select * from spatial_ref_sys").Result;
        using (var writer = new StreamWriter("./Synchronization/spatial_ref_sys.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(spatialRefSyss);
        }
    }


    public void CsvToLitedb()
    {
        using var reader = new StreamReader("./Synchronization/spatial_ref_sys.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        List<SpatialRefSys> spatialRefSyss = csv.GetRecords<SpatialRefSys>().ToList<SpatialRefSys>();

        using(var litedb = new LiteDatabase("./Synchronization/data.db"))
        {
            var spatialRefSyssCol = litedb.GetCollection<SpatialRefSys>("spatial_ref_sys");
            
            foreach (SpatialRefSys spatialRefSys in spatialRefSyssCol.FindAll().Except(spatialRefSyss).ToList())
            {
                spatialRefSyssCol.Delete(spatialRefSys.srid);   
            }           
            
            foreach (SpatialRefSys spatialRefSys in spatialRefSyss)
            {
                try
                {
                    spatialRefSyssCol.Insert(spatialRefSys);
                }
                catch(LiteException e)
                { 
                    if (e.ErrorCode == 110)
                    {
                        spatialRefSyssCol.Update(spatialRefSys);
                    }
                }
            }
                
        }
    }     


     public void LitedbToCsv()
    { 
        using var litedb = new LiteDatabase("./Synchronization/data.db");
        var spatialRefSyssCol = litedb.GetCollection<SpatialRefSys>("spatial_ref_sys").FindAll();
        
        using (var writer = new StreamWriter("./Synchronization/spatial_ref_sys.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(spatialRefSyssCol);
        }
    }


    public async Task CsvToPgsql()
    {
        using var reader = new StreamReader("./Synchronization/spatial_ref_sys.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        List<SpatialRefSys> spatialRefSyss = csv.GetRecords<SpatialRefSys>().ToList<SpatialRefSys>();
        
        using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
        List<SpatialRefSys> spatialRefSyssPgsql =  connection.QueryAsync<SpatialRefSys>("select * from spatial_ref_sys").Result.ToList<SpatialRefSys>();

        foreach (SpatialRefSys spatialRefSys in spatialRefSyssPgsql.Except(spatialRefSyss).ToList())
        {
            await connection.ExecuteAsync(
            "delete from spatial_ref_sys"+
            " where srid = @srid", 
            spatialRefSys);
        }

        foreach (SpatialRefSys spatialRefSys in spatialRefSyss)
        {
            try
            {
                await connection.ExecuteAsync(
                "insert into spatial_ref_sys"+
                " values(@srid, @auth_name, @auth_srid, @srtext, @proj4text)", 
                spatialRefSys);
            }
            catch(NpgsqlException e)
            {
                if (e.ErrorCode == -2147467259)
                {
                    await connection.ExecuteAsync(
                    "update spatial_ref_sys set"+
                    "srid = @srid, auth_name = @auth_name, auth_srid = @auth_srid, srtext = @srtext, "+
                    "proj4text = @proj4text where srid = @srid", 
                    spatialRefSys);
                }
            }
        }
    }
}