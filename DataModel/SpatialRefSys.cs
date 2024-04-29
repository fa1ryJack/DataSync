namespace DataModel;

public class SpatialRefSys
{
    public required int srid { get; set; }
    public string? auth_name { get; set; }
    public int? auth_srid { get; set; }
    public string? srtext { get; set; }
    public string? proj4text { get; set; }
    
    public override bool Equals(object? obj)
    {
        SpatialRefSys? fooItem = obj as SpatialRefSys;

        if (fooItem == null) 
        {
           return false;
        }

        return fooItem.srid == srid;
    }
     public override int GetHashCode()
    {
        return srid.GetHashCode();
    }
}
