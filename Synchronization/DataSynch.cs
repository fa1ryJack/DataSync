namespace Synchronization;

public class DataSynch
{
    private readonly UsersSynch usersSynch;
    private readonly EnterpriseDataAccessLevelSynch enterpriseDataAccessLevelSynch;
    private readonly EnterprisesSynch enterprisesSynch;
    private readonly ExteranalObjectsSynch exteranalObjectsSynch;
    private readonly GenericTypesSynch genericTypesSynch;
   // private GeoMunicipalitiesSynch geoMunicipalitiesSynch;
    private readonly GeoMunicipalitiesDataAccessLevelSynch geoMunicipalitiesDataAccessLevelSynch; 
   // private MigrationHistoriesSynch migrationHistoriesSynch;
    private readonly PersonsSynch personsSynch;
    private readonly RightGroupRightsSynch rightGroupRightsSynch;
    private readonly RightGroupsSynch rightGroupsSynch;
    private readonly RightsSynch rightsSynch;
    private readonly RoleRightsSynch roleRightsSynch;
    private readonly RolesSynch rolesSynch;
    //private SpatialRefSysSynch spatialRefSysSynch;
    private readonly UserRolesSynch userRolesSynch; 

    public DataSynch(IConfiguration config)
    {
        enterpriseDataAccessLevelSynch = new EnterpriseDataAccessLevelSynch(config);
        enterprisesSynch = new EnterprisesSynch(config);
        exteranalObjectsSynch = new ExteranalObjectsSynch(config);
        genericTypesSynch = new GenericTypesSynch(config);
        //geoMunicipalitiesSynch = new GeoMunicipalitiesSynch(config);
        geoMunicipalitiesDataAccessLevelSynch = new GeoMunicipalitiesDataAccessLevelSynch(config);
        //migrationHistoriesSynch = new MigrationHistoriesSynch(config);
        personsSynch = new PersonsSynch(config);
        rightGroupRightsSynch = new RightGroupRightsSynch(config);
        rightGroupsSynch = new RightGroupsSynch(config);
        rightsSynch = new RightsSynch(config);
        roleRightsSynch = new RoleRightsSynch(config);
        rolesSynch = new RolesSynch(config);
        //spatialRefSysSynch = new SpatialRefSysSynch(config);
        userRolesSynch = new UserRolesSynch(config);
        usersSynch = new UsersSynch(config);
    }

    public void ToLiteDB()
    {
        rightGroupsSynch.PgsqlToCsv();
        rightGroupsSynch.CsvToLitedb();

        rightsSynch.PgsqlToCsv();
        rightsSynch.CsvToLitedb();

        rightGroupRightsSynch.PgsqlToCsv();
        rightGroupRightsSynch.CsvToLitedb();

        rolesSynch.PgsqlToCsv();
        rolesSynch.CsvToLitedb();

        roleRightsSynch.PgsqlToCsv();
        roleRightsSynch.CsvToLitedb();

        //spatialRefSysSynch.PgsqlToCsv();
       // spatialRefSysSynch.CsvToLitedb();

       // migrationHistoriesSynch.PgsqlToCsv();
       // migrationHistoriesSynch.CsvToLitedb();

        //geoMunicipalitiesSynch.PgsqlToCsv();
        //geoMunicipalitiesSynch.CsvToLitedb();

        genericTypesSynch.PgsqlToCsv();
        genericTypesSynch.CsvToLitedb();

        exteranalObjectsSynch.PgsqlToCsv();
        exteranalObjectsSynch.CsvToLitedb();

        geoMunicipalitiesDataAccessLevelSynch.PgsqlToCsv();
        geoMunicipalitiesDataAccessLevelSynch.CsvToLitedb();

        personsSynch.PgsqlToCsv();
        personsSynch.CsvToLitedb();

        enterprisesSynch.PgsqlToCsv();
        enterprisesSynch.CsvToLitedb();
        personsSynch.CsvToLitedb();

        usersSynch.PgsqlToCsv();
        usersSynch.CsvToLitedb();
        
        enterpriseDataAccessLevelSynch.PgsqlToCsv();
        enterpriseDataAccessLevelSynch.CsvToLitedb();

         userRolesSynch.PgsqlToCsv();
         userRolesSynch.CsvToLitedb();
    }

    public void ToPgsql()
    {
        rightGroupsSynch.LitedbToCsv();
        _ = rightGroupsSynch.CsvToPgsql();

        rightsSynch.LitedbToCsv();
        _ = rightsSynch.CsvToPgsql();

        rightGroupRightsSynch.LitedbToCsv();
        _ = rightGroupRightsSynch.CsvToPgsql();

        rolesSynch.LitedbToCsv();
        _ = rolesSynch.CsvToPgsql();

        roleRightsSynch.LitedbToCsv();
        _ = roleRightsSynch.CsvToPgsql();

        //spatialRefSysSynch.LitedbToCsv();
        //_ = spatialRefSysSynch.CsvToPgsql();

       // migrationHistoriesSynch.LitedbToCsv();
        //_ = migrationHistoriesSynch.CsvToPgsql();

        //geoMunicipalitiesSynch.LitedbToCsv();
       // _ = geoMunicipalitiesSynch.CsvToPgsql();

        genericTypesSynch.LitedbToCsv();
        _ = genericTypesSynch.CsvToPgsql();

        exteranalObjectsSynch.LitedbToCsv();
        _ = exteranalObjectsSynch.CsvToPgsql();

         geoMunicipalitiesDataAccessLevelSynch.LitedbToCsv();
         _ = geoMunicipalitiesDataAccessLevelSynch.CsvToPgsql();

        personsSynch.LitedbToCsv();
        _ = personsSynch.CsvToPgsql();

        enterprisesSynch.LitedbToCsv();
        _ = enterprisesSynch.CsvToPgsql();
        _ = personsSynch.CsvToPgsql();

        usersSynch.LitedbToCsv();
        _ = usersSynch.CsvToPgsql();
        
        enterpriseDataAccessLevelSynch.LitedbToCsv();
        _ = enterpriseDataAccessLevelSynch.CsvToPgsql();

        userRolesSynch.LitedbToCsv();
       _ = userRolesSynch.CsvToPgsql();
    }

    public void LitedbToCsv()
    {
        rightGroupsSynch.LitedbToCsv();

        rightsSynch.LitedbToCsv();

        rightGroupRightsSynch.LitedbToCsv();

        rolesSynch.LitedbToCsv();

        roleRightsSynch.LitedbToCsv();

        //spatialRefSysSynch.LitedbToCsv();

        //migrationHistoriesSynch.LitedbToCsv();

       // geoMunicipalitiesSynch.LitedbToCsv();

        genericTypesSynch.LitedbToCsv();

        exteranalObjectsSynch.LitedbToCsv();

        geoMunicipalitiesDataAccessLevelSynch.LitedbToCsv();

        personsSynch.LitedbToCsv();

        enterprisesSynch.LitedbToCsv();

        usersSynch.LitedbToCsv();
        
        enterpriseDataAccessLevelSynch.LitedbToCsv();

        userRolesSynch.LitedbToCsv();
    }

    public void CsvToPgsql()
    {
        _ = rightGroupsSynch.CsvToPgsql();

        _ = rightsSynch.CsvToPgsql();

        _ = rightGroupRightsSynch.CsvToPgsql();

        _ = rolesSynch.CsvToPgsql();

        _ = roleRightsSynch.CsvToPgsql();

        //spatialRefSysSynch.CsvToPgsql();

        //migrationHistoriesSynch.CsvToPgsql();

        // geoMunicipalitiesSynch.CsvToPgsql();

        _ = genericTypesSynch.CsvToPgsql();

        _ = exteranalObjectsSynch.CsvToPgsql();

        _ = geoMunicipalitiesDataAccessLevelSynch.CsvToPgsql();

        _ = personsSynch.CsvToPgsql();

        _ = enterprisesSynch.CsvToPgsql();

        _ = usersSynch.CsvToPgsql();

        _ = enterpriseDataAccessLevelSynch.CsvToPgsql();

        _ = userRolesSynch.CsvToPgsql();
    }
}