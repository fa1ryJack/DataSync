using Dapper;
using Npgsql;
using DataModel;
using CsvHelper;
using System.Globalization;
using LiteDB;

namespace Synchronization;

public class GenericTypesSynch
{
    private readonly IConfiguration _config;
    public GenericTypesSynch(IConfiguration config)
    {
        _config = config;
    }


    public void PgsqlToCsv()
    { 
        using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
        
        var genericTypes =  connection.QueryAsync<GenericTypes>("select * from generic_types").Result;
        using (var writer = new StreamWriter("./Synchronization/generic_types.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(genericTypes);
        }
    }


    public void CsvToLitedb()
    {
        using var reader = new StreamReader("./Synchronization/generic_types.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        List<GenericTypes> genericTypes = csv.GetRecords<GenericTypes>().ToList<GenericTypes>();

        using(var litedb = new LiteDatabase("./Synchronization/data.db"))
        {
            var genericTypesCol = litedb.GetCollection<GenericTypes>("generic_types");
            
            foreach (GenericTypes genericType in genericTypesCol.FindAll().Except(genericTypes).ToList())
            {   
                genericTypesCol.Delete(genericType.id);   
            }
            
            foreach (GenericTypes genericType in genericTypes)
            {
                try
                {
                    genericTypesCol.Insert(genericType);
                }
                catch(LiteException e)
                { 
                    if (e.ErrorCode == 110)
                    {
                        genericTypesCol.Update(genericType);
                    }
                }
            }
                
        }
    }


    public void LitedbToCsv()
    { 
        using var litedb = new LiteDatabase("./Synchronization/data.db");
        var genericTypesCol = litedb.GetCollection<GenericTypes>("generic_types").FindAll();
        
        using (var writer = new StreamWriter("./Synchronization/generic_types.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(genericTypesCol);
        }
    }


     public async Task CsvToPgsql()
    {
        using var reader = new StreamReader("./Synchronization/generic_types.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        List<GenericTypes> genericTypes = csv.GetRecords<GenericTypes>().ToList<GenericTypes>();
        
        using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
        List<GenericTypes> genericTypesPgsql =  connection.QueryAsync<GenericTypes>("select * from generic_types").Result.ToList<GenericTypes>();

        foreach (GenericTypes genericType in genericTypesPgsql.Except(genericTypes).ToList())
        {
            await connection.ExecuteAsync(
            "delete from generic_types"+
            " where id = @id", 
            genericType);
        }
            

        foreach (GenericTypes genericType in genericTypes)
        {
            try
            {
                await connection.ExecuteAsync(
                "insert into generic_types"+
                " values(@name, @short_name, @code, @parent_id, @id, @date_off)", 
                genericType);
            }
            catch(NpgsqlException e)
            {
                if (e.ErrorCode == -2147467259)
                {
                    await connection.ExecuteAsync(
                    "update generic_types set"+
                    "name = @name, short_name = @short_name, code = @code, parent_id = @parent_id, id = @id,"+
                    " date_off = @date_off, where id = @id", 
                    genericType);
                }
            }
        }
    }
}