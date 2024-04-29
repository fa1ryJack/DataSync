namespace DataModel;

public class GeoMunicipalitiesDataAccessLevel
{
    public required Guid id { get; set;}
    public required Guid geo_municipality_id { get; set;}
    public required Guid data_access_level_id { get; set;}
    public DateTime? date_off { get; set;}

   // public GeoMunicipalities? GeoMunicipalities { get; set; } = null!;
   // public GenericTypes? GenericTypes { get; set; } = null!;

   public override bool Equals(object? obj)
    {
        GeoMunicipalitiesDataAccessLevel? fooItem = obj as GeoMunicipalitiesDataAccessLevel;

        if (fooItem == null) 
        {
           return false;
        }

        return fooItem.id == id;
    }
     public override int GetHashCode()
    {
        return id.GetHashCode();
    }
}
