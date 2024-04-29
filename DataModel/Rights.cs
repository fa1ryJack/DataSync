namespace DataModel;

public class Rights
{
    public required Guid id { get; set; }
    public string? name { get; set; }
    public string? alias { get; set; }
    public DateTime? date_off { get; set; }
    public DateTime? date_on { get; set; }
    public string? owner { get; set; }
    public required int order { get; set; }

    public ICollection<RightGroupRights>? RightGroupRights { get; } = new List<RightGroupRights>();
    public ICollection<RoleRights>? RoleRights { get; } = new List<RoleRights>();

    public override bool Equals(object? obj)
    {
        Rights? fooItem = obj as Rights;

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
