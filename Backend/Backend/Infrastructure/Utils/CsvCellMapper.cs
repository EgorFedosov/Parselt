using Backend.Application.DTOs.Response;
using Backend.Core.Interfaces;
using Backend.Infrastructure.Data.Entities;

namespace Backend.Infrastructure.Utils;


public class CsvParsedCellMapper : ICsvParsedCellMapper
{
    public IEnumerable<CsvParsedCell> Map(CsvParsedRowDto row,  FileMetaData metaData)
    {
        foreach (var column in row.ParsedValues)
        {   
            yield return new CsvParsedCell
            {
                FileMetaData = metaData,
                FileMetaDataId = metaData.Id,
                RowIndex = row.RowIndex,
                ColumnName = column.Key,
                Value = column.Value?.ToString(),
                OperationId = row.OperationId
            };
        }
    }
}
