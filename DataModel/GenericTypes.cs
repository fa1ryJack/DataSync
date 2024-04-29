namespace DataModel;

public class GenericTypes
{
    public required string name { get; set; }
    public string? short_name { get; set; }
    public string? code { get; set; }
    public required Guid parent_id { get; set; }
    public required Guid id { get; set; }
    public DateTimeOffset? date_off { get; set; }

    public GenericTypes? ParentGenericType { get; set; } = null!;

    public ICollection<GeoMunicipalitiesDataAccessLevel>? GeoMunicipalitiesDataAccessLevel { get; } = new List<GeoMunicipalitiesDataAccessLevel>();
    public ICollection<ExteranalObjects>? ExteranalObjects { get; } = new List<ExteranalObjects>();
    public ICollection<EnterpriseDataAccessLevel>? EnterpriseDataAccessLevel { get; } = new List<EnterpriseDataAccessLevel>();
    public ICollection<GenericTypes>? ChildGenericTypes { get; } = new List<GenericTypes>();

    public override bool Equals(object? obj)
    {
        GenericTypes? fooItem = obj as GenericTypes;

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
