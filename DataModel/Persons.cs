using System.ComponentModel.DataAnnotations.Schema;
using Dapper;
namespace DataModel;

public class Persons
{
    public required Guid id { get; set; }
    public required string first_name { get; set; }
    public required string last_name { get; set; }
    public string? middle_name { get; set; }
    public string? position { get; set; }
    public required Guid? enterprise_id { get; set; }
    public string? phone { get; set; }
    public string? email { get; set; }
    public DateTime? date_off { get; set; }
    public DateTime? date_on { get; set; }

    //public Enterprises? Enterprise { get; set; } = null!;
    public ICollection<Users>? Users { get; } = new List<Users>();
    //public ICollection<Enterprises>? Enterprises { get; } = new List<Enterprises>();

    public override bool Equals(object? obj)
    {
        Persons? fooItem = obj as Persons;

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
