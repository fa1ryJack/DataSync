using NetTopologySuite.Geometries;

namespace DataModel;

public class GeoMunicipalities
{
    public required Guid id { get; set;}
    public Geometry? coordinates  { get; set;}
    public required string name  { get; set;}
    public DateTimeOffset? date_off { get; set;}
    public required decimal level { get; set;}

    public ICollection<GeoMunicipalitiesDataAccessLevel>? GeoMunicipalitiesDataAccessLevel { get; } = new List<GeoMunicipalitiesDataAccessLevel>();

    public override bool Equals(object? obj)
    {
        GeoMunicipalities? fooItem = obj as GeoMunicipalities;

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
