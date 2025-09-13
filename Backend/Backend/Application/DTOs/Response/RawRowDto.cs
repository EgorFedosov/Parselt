namespace Backend.Application.DTOs.Response;

public class RawRowDto
{
    public RawRowDto(string rawRow)
    {
        RawRow = rawRow;
    }

    public string RawRow { get; init; }
}