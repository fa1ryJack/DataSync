namespace DataModel;

public class UserRoles
{
    public required Guid role_id { get;  set; }
    public required Guid user_id { get;  set; }
    public DateTime? date_off { get; set; }
    public DateTime? date_on { get; set; }

    //public Users? Users { get; set; } = null!;
    //public Roles? Roles { get; set; } = null!;

    public override bool Equals(object? obj)
    {
        UserRoles? fooItem = obj as UserRoles;

        if (fooItem == null) 
        {
           return false;
        }

        return fooItem.role_id.ToString()+fooItem.user_id.ToString() == role_id.ToString()+user_id.ToString();
    }
     public override int GetHashCode()
    {
        return (role_id.ToString()+user_id.ToString()).GetHashCode();
    }
}
