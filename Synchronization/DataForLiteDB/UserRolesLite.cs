namespace DataForLiteDB;

public class UserRolesLite
{
    public required string Id { get; set; }
    public required Guid RoleId { get;  set; }
    public required Guid UserId { get;  set; }
    
    public DateTime? DateOff { get; set; }
    public DateTime? DateOn { get; set; }
}