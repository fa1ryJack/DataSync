namespace DataModel;

public class Roles
{
    public required Guid id { get;  set; }
    public string? name { get; set; }
     public DateTime? date_off { get; set; }
    public DateTime? date_on { get; set; }

    public ICollection<RoleRights>? RoleRights { get; } = new List<RoleRights>();
    public ICollection<UserRoles>? UserRoles { get; } = new List<UserRoles>();

    public override bool Equals(object? obj)
    {
        Roles? fooItem = obj as Roles;

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
