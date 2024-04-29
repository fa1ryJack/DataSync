namespace DataModel;

public class Users
{
    public required Guid id { get;  set; }
    public required string login { get;  set; }
    public Guid? person_id { get;  set; }
    public required string password { get;  set; }
    public DateTime date_off { get; set; }
    public DateTime date_on { get; set; }
    public  string? token { get;  set; }
    public  string? queue { get;  set; }
    public  required bool is_blocked { get;  set; }
    public required int login_fail_count { get; set; } = 0;

    public ICollection<UserRoles>? UserRoles { get; } = new List<UserRoles>();
    //public Persons? Persons { get; set; } = null!;

    public override bool Equals(object? obj)
    {
        Users? fooItem = obj as Users;

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
