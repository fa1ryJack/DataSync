namespace DataModel;

public class Enterprises
{
    public required Guid id { get; set; }
    public required string short_name { get; set; }
    public string? full_name { get; set; }
    public string? address { get; set; }
    public required string email { get; set; }
    public string? phone { get; set; }
    public bool? is_integration { get; set; }
    public string? web_site { get; set; }
    public Guid? responsible_person_id { get; set; }
    public DateTime? date_on { get; set; }
    public DateTime? date_off { get; set; }
    public string? inn { get; set; }
    public string? kpp { get; set; }
    public string? ogrn { get; set; }
    public string? account { get; set; }
    public string? bank { get; set; }
    public string? cor_account { get; set; }
    public string? bik { get; set; }

    public Persons? ResponsiblePerson { get; set; } = null!;
    public ICollection<Persons>? Persons { get; } = new List<Persons>();
    public ICollection<EnterpriseDataAccessLevel>? EnterpriseDataAccessLevel { get; } = new List<EnterpriseDataAccessLevel>();
       
       public override bool Equals(object? obj)
    {
        Enterprises? fooItem = obj as Enterprises;

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
