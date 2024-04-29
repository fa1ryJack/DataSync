namespace DataModel;

public class MigrationHistories
{
    public required int id { get; set; }
    public required string script_name { get; set; }
    public DateTimeOffset? add_date { get; set; }

    public override bool Equals(object? obj)
    {
        MigrationHistories? fooItem = obj as MigrationHistories;

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
