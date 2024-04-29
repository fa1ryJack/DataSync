namespace DataModel;

public class EnterpriseDataAccessLevel
{
    public required Guid id { get; set; }
    public DateTime? date_on { get; set; }
    public required Guid enterprise_id { get; set; }
    public required Guid data_access_level_id { get; set; }
    public DateTimeOffset? date_off { get; set; }

    //public Enterprises? Enterprises { get; set; } = null!;
    //public GenericTypes? GenericTypes { get; set; } = null!;

    public override bool Equals(object? obj)
    {
        EnterpriseDataAccessLevel? fooItem = obj as EnterpriseDataAccessLevel;

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
