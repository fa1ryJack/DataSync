namespace DataModel;

public class GeographyColumns
{
    public string? f_table_catalog { get; set; }
    public string? f_table_schema { get; set; }
    public string? f_table_name { get; set; }
    public string? f_geography_column { get; set; }
    public int? coord_dimension { get; set; }
    public int? srid { get; set; }
    public string? type { get; set; }
}
