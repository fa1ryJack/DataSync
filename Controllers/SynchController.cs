using Dapper;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using DataModel;
using LiteDB;
using Synchronization;

namespace Controllers;
[ApiController]
[Route("api/synch")]
public class SynchController : ControllerBase
{
    private readonly IConfiguration _config;
    public SynchController(IConfiguration config)
    {
        _config = config;
    }


    [HttpGet("LiteDB")]
    public OkResult SynchLiteDB()
    {
        DataSynch dataSynch = new DataSynch(_config);
        dataSynch.ToLiteDB();
        return Ok();
    }

    [HttpGet("PostgreSQL")]
    public OkResult SynchPgsql()
    {
        DataSynch dataSynch = new DataSynch(_config);
        dataSynch.ToPgsql();
        return Ok();
    }
}