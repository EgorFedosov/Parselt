
namespace Backend.Core.Interfaces;

using Application.DTOs.Response;
using Infrastructure.Data.Entities;

public interface ICsvParsedCellMapper
{
    IEnumerable<CsvParsedCell> Map(CsvParsedRowDto row,  FileMetaData metaData);
}