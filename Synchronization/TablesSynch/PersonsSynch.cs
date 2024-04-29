using Dapper;
using Npgsql;
using DataModel;
using CsvHelper;
using System.Globalization;
using LiteDB;

namespace Synchronization;

public class PersonsSynch
{
    private readonly IConfiguration _config;
    public PersonsSynch(IConfiguration config)
    {
        _config = config;
    }
    
    public void PgsqlToCsv()
    { 
        using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
        
        var persons =  connection.QueryAsync<Persons>("select * from persons").Result;
        using (var writer = new StreamWriter("./Synchronization/persons.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(persons);
        }
    }

     public void CsvToLitedb()
    {
        using var reader = new StreamReader("./Synchronization/persons.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        List<Persons> persons = csv.GetRecords<Persons>().ToList<Persons>();

        using(var litedb = new LiteDatabase("./Synchronization/data.db"))
        {
            var personsCol = litedb.GetCollection<Persons>("persons");
            
            foreach (Persons person in personsCol.FindAll().Except(persons).ToList())
            {               
                personsCol.Delete(person.id);                    
            }
                        
            foreach (Persons person in persons)
            {
                try
                {
                    personsCol.Insert(person);
                }
                catch(LiteException e)
                { 
                    if (e.ErrorCode == 110)
                    {
                        personsCol.Update(person);
                    }
                }
            }
                
        }
    } 


    public void LitedbToCsv()
    { 
        using var litedb = new LiteDatabase("./Synchronization/data.db");
        var personsCol = litedb.GetCollection<Persons>("persons").FindAll();
        
        using (var writer = new StreamWriter("./Synchronization/persons.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(personsCol);
        }
    }

    public async Task CsvToPgsql()
    {
        using var reader = new StreamReader("./Synchronization/persons.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        List<Persons> persons = csv.GetRecords<Persons>().ToList<Persons>();
        
        using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
        List<Persons> personsPgsql =  connection.QueryAsync<Persons>("select * from persons").Result.ToList<Persons>();

        foreach (Persons person in personsPgsql.Except(persons).ToList())
        {
                    
            await connection.ExecuteAsync(
            "delete from persons"+
            " where id = @id", 
            person);  
        }

        foreach (Persons person in persons)
        {
            try
            {
                await connection.ExecuteAsync(
                "insert into persons"+
                " values(@id, @first_name, @last_name, @middle_name, @position, @enterprise_id, @phone, @email, @date_off, @date_on)", 
                person);
            }
            catch(NpgsqlException e)
            {
                if (e.ErrorCode == -2147467259)
                {
                    await connection.ExecuteAsync(
                    "update persons set"+
                    " id = @id, first_name = @first_name, last_name = @last_name, middle_name = @middle_name,"+
                    " position = @position, enterprise_id = @enterprise_id, phone = @phone, email = @email,"+
                    " date_off = @date_off, date_on = @date_on"+
                    " where id = @id", 
                    person);
                }
            }
        }
    }
}