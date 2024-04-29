using Dapper;
using Npgsql;
using DataModel;
using CsvHelper;
using System.Globalization;
using LiteDB;

namespace Synchronization;

public class EnterprisesSynch
{
    private readonly IConfiguration _config;
    public EnterprisesSynch(IConfiguration config)
    {
        _config = config;
    }
    
     public void PgsqlToCsv()
    { 
        using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
        
        var enterprises =  connection.QueryAsync<Enterprises>("select * from enterprises").Result;
        using (var writer = new StreamWriter("./Synchronization/enterprises.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(enterprises);
        }
    }


    public void CsvToLitedb()
    { 
        using var reader = new StreamReader("./Synchronization/enterprises.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        List<Enterprises> enterprises = csv.GetRecords<Enterprises>().ToList<Enterprises>();

        using(var litedb = new LiteDatabase("./Synchronization/data.db"))
        {
            var enterprisesCol = litedb.GetCollection<Enterprises>("enterprises");

            foreach (Enterprises enterprise in enterprisesCol.FindAll().Except(enterprises).ToList())
            {
                enterprisesCol.Delete(enterprise.id);
            }
            
            
            foreach (Enterprises enterprise in enterprises)
            {
                try
                {
                    enterprisesCol.Insert(enterprise);
                }
                catch(LiteException e)
                { 
                    if (e.ErrorCode == 110)
                    {
                        enterprisesCol.Update(enterprise);
                    }
                }
            }
                
        }
    } 


    public void LitedbToCsv()
    { 
        using var litedb = new LiteDatabase("./Synchronization/data.db");
        var enterprisesCol = litedb.GetCollection<Enterprises>("enterprises").FindAll();
        
        using (var writer = new StreamWriter("./Synchronization/enterprises.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(enterprisesCol);
        }
    }


    public async Task CsvToPgsql()
    {
        using var reader = new StreamReader("./Synchronization/enterprises.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        List<Enterprises> enterprises = csv.GetRecords<Enterprises>().ToList<Enterprises>();
        
        using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
        List<Enterprises> enterprisesPgsql =  connection.QueryAsync<Enterprises>("select * from enterprises").Result.ToList<Enterprises>();

        
                foreach (Enterprises enterprise in enterprisesPgsql.Except(enterprises).ToList())
                {
                    await connection.ExecuteAsync(
                    "delete from enterprises"+
                    " where id = @id", 
                    enterprise);
    
                }
            

        foreach (Enterprises enterprise in enterprises)
        {
            try
            {
                await connection.ExecuteAsync(
                "insert into enterprises"+
                " values(@id, @short_name, @full_name, @address, @email, @phone, @is_integration, @web_site,"+
                " @responsible_person_id, @date_on, @date_off, @inn, @kpp, @ogrn, @account, @bank, @cor_account, @bik)", 
                enterprise);
            }
            catch(NpgsqlException e)
            {
                if (e.ErrorCode == -2147467259)
                {
                    await connection.ExecuteAsync(
                    "update enterprises set"+
                    " id = @id, short_name = @short_name, full_name = @full_name, address = @address,"+
                    " email = @email, phone = @phone, is_integration = @is_integration, web_site = @web_site, "+
                    "responsible_person_id = @responsible_person_id, date_on = @date_on, date_off = @date_off,"+
                    " inn = @inn, kpp = @kpp, ogrn = @ogrn, account = @account, bank = @bank, cor_account = @cor_account, bik = @bik"+
                    " where id = @id", 
                    enterprise);
                }
            }
        }
    }
}