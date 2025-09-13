using Backend.Application.DTOs.Logging;
using Backend.Application.DTOs.Request;
using Backend.Application.DTOs.Response;
using Backend.Infrastructure.Data.Entities;

namespace Backend.Core.Interfaces;

public interface ICsvDynamicParser
{
    Task<CsvPreviewRawRowsDto> PreviewAsync(string fileName, int previewRowsCount = 5);

    Task<(ParseResultDto, OperationLog)> ParseCsvAsync(
        CsvParserRequestDto request,
        ILoggingService logger);
}