using DataModel;
namespace DataForLiteDB;

public class RoleRightsLite
{
    public required string Id { get; set; }
    public required Guid RoleId { get;  set; }
    public required Guid RightId { get;  set; }
    
    public DateTime? DateOff { get; set; }
    public DateTime? DateOn { get; set; }
}