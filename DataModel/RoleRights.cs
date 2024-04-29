namespace DataModel;

public class RoleRights
{
    public required Guid role_id { get;  set; }
    public required Guid right_id { get;  set; }
    
    public DateTime? date_off { get; set; }
    public DateTime? date_on { get; set; }

    //public Rights? Rights { get; set; } = null!;
    //public Roles? Roles { get; set; } = null!;

    public override bool Equals(object? obj)
    {
        RoleRights? fooItem = obj as RoleRights;

        if (fooItem == null) 
        {
           return false;
        }

        return fooItem.role_id.ToString()+fooItem.right_id.ToString() == role_id.ToString()+right_id.ToString();
    }
     public override int GetHashCode()
    {
        return (role_id.ToString()+right_id.ToString()).GetHashCode();
    }
}
