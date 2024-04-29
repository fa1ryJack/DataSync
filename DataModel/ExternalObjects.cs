namespace DataModel;

public class ExteranalObjects
{
    public required Guid id { get; set; }
    public DateTimeOffset? date_on { get; set;}
    public required Guid object_id { get; set; }
    public required Guid system_id { get; set; }
    public required string external_id { get; set; }
    public string? hash { get; set; }
    public DateTimeOffset? date_off { get; set;}

    //public GenericTypes? GenericTypes { get; set;} = null!;

    public override bool Equals(object? obj)
    {
        ExteranalObjects? fooItem = obj as ExteranalObjects;

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
