namespace DataModel;

public class RightGroupRights
{
    public required Guid right_id { get; set; }
    public required Guid right_group_id { get; set; }

    //public Rights? Rights { get; set; } = null!;
    //public RightGroups? RightGroups { get; set; } = null!;


    public override bool Equals(object? obj)
    {
        RightGroupRights? fooItem = obj as RightGroupRights;

        if (fooItem == null) 
        {
           return false;
        }

        return fooItem.right_group_id.ToString()+fooItem.right_id.ToString() == right_group_id.ToString()+right_id.ToString();
    }
     public override int GetHashCode()
    {
        return (right_group_id.ToString()+right_id.ToString()).GetHashCode();
    }
}
