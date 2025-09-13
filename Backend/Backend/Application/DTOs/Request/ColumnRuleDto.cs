namespace Backend.Application.DTOs.Request;

public enum DataType
{
    Decimal,
    Double,
    Date,
    Bool,
    String
}

public class ColumnRuleDto
{
    public string? ColumnName { get; set; }
    public DataType DataType { get; set; }


    public decimal? MinValue { get; set; }
    public decimal? MaxValue { get; set; }

    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }

    public bool? BoolValue { get; set; }
    public string? Contains { get; set; }
}