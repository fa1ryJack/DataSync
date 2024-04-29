namespace DataModel;

public class RightGroups
{
    public required Guid id { get;  set; }
    public required int order { get; set; }
    public required string name { get; set; }
    public DateTime? date_off { get; set; }
    public DateTime? date_on { get; set; }

    public ICollection<RightGroupRights>? RightGroupRights { get; } = new List<RightGroupRights>();

    public override bool Equals(object? obj)
    {
        RightGroups? fooItem = obj as RightGroups;

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
