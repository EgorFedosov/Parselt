using Backend.Infrastructure.Data.Entities;
using Backend.Infrastructure.Data.Uploads;

namespace Backend.Infrastructure.Parser;

using Core.Logging.Enums;
using Application.DTOs.Logging;
using System.Globalization;
using Application.DTOs.Request;
using Application.DTOs.Response;
using Core.Interfaces;
using static Rules.ColumnRuleFactory;
using Utils;

public class CsvDynamicParser : ICsvDynamicParser
{
    public async Task<CsvPreviewRawRowsDto> PreviewAsync(string fileName, int previewRowsCount = 11)
    {
        var uploadPath = Path.Combine(FilePathService.UploadPath, fileName);
        var preview = new CsvPreviewRawRowsDto();


        using (var reader = new StreamReader(uploadPath))
        {
            string? line;
            int rowCount = 0;

            while ((line = await reader.ReadLineAsync()) != null && rowCount < previewRowsCount)
            {
                var rawRow = new RawRowDto(line);
                preview.Rows.Add(rawRow);
                rowCount++;
            }
        }

        return preview;
    }

    public async Task<(ParseResultDto, OperationLog)> ParseCsvAsync(
        CsvParserRequestDto request,
        ILoggingService logger)
    {
        if (string.IsNullOrEmpty(request.Delimiter)) request.Delimiter = ",";
        var rules = request.Rules.Select(MapRule).ToList();
        var result = new ParseResultDto();

        var filePath = Path.Combine(FilePathService.UploadPath, request.FileName);
        using var reader = new StreamReader(filePath);

        var operation = new OperationLog
        {
            Type = OperationType.ParseCsv,
        };
        logger.RegisterOperation(operation);
        result.OperationId = operation.OperationId;


        string? line;
        uint rowIndex = 0;
        bool isFirst = true;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            if (isFirst)
            {
                isFirst = false;
                continue;
            }

            rowIndex++;
            var values = CsvLineParser.ParseCsvLine(line, request.Delimiter);
            if (values.Length != request.Rules.Count)
            {
                logger.AddError(operation.OperationId, $"Неверное количество столбцов в строке {rowIndex}");
            }

            var parsedRow = new CsvParsedRowDto
            {
                IsValid = true,
                RowIndex = rowIndex,
                OperationId = operation.OperationId
            };

            for (int i = 0; i < rules.Count; i++)
            {
                string? value = i < values.Length ? values[i]?.Trim() : null;
                if (string.IsNullOrWhiteSpace(value))
                    value = null;

                var rule = rules[i];

                if (string.IsNullOrEmpty(rule.ColumnName))
                {
                    rule.ColumnName = $"Column_{i}";

                    logger.AddError(operation.OperationId,
                        $"Имя столбца для правила с индексом {i} не задано. Назначено имя по умолчанию: '{rule.ColumnName}'.");
                }


                object? parsed = null;
                if (value != null)
                {
                    switch (rule.DataType)
                    {
                        case DataType.Decimal:
                            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var decVal))
                            {
                                parsed = decVal;

                                if (rule.MinValue.HasValue && decVal < rule.MinValue.Value)
                                {
                                    logger.AddError(operation.OperationId,
                                        $"Значение '{decVal}' меньше допустимого минимума ({rule.MinValue.Value}) в столбце '{rule.ColumnName}' (строка {rowIndex})");
                                    parsedRow.IsValid = false;
                                }

                                if (rule.MaxValue.HasValue && decVal > rule.MaxValue.Value)
                                {
                                    logger.AddError(operation.OperationId,
                                        $"Значение '{decVal}' превышает допустимый максимум ({rule.MaxValue.Value}) в столбце '{rule.ColumnName}' (строка {rowIndex})");
                                    parsedRow.IsValid = false;
                                }
                            }
                            else
                            {
                                logger.AddError(operation.OperationId,
                                    $"Неверный тип значения '{value}' (ожидался Decimal) в столбце '{rule.ColumnName}' (строка {rowIndex})");
                                parsed = null;
                            }

                            break;

                        case DataType.Double:
                            if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var dblVal))
                            {
                                parsed = dblVal;

                                if (rule.MinValue.HasValue && (decimal)dblVal < rule.MinValue.Value)
                                {
                                    logger.AddError(operation.OperationId,
                                        $"Значение '{dblVal}' меньше допустимого минимума ({rule.MinValue.Value}) в столбце '{rule.ColumnName}' (строка {rowIndex})");
                                    parsedRow.IsValid = false;
                                }

                                if (rule.MaxValue.HasValue && (decimal)dblVal > rule.MaxValue.Value)
                                {
                                    logger.AddError(operation.OperationId,
                                        $"Значение '{dblVal}' превышает допустимый максимум ({rule.MaxValue.Value}) в столбце '{rule.ColumnName}' (строка {rowIndex})");
                                    parsedRow.IsValid = false;
                                }
                            }
                            else
                            {
                                logger.AddError(operation.OperationId,
                                    $"Неверный тип значения '{value}' (ожидался Double) в столбце '{rule.ColumnName}' (строка {rowIndex})");
                                parsed = null;
                            }

                            break;

                        case DataType.Date:
                            if (DateTime.TryParse(value, out var dateVal))
                            {
                                parsed = dateVal;

                                if (rule.DateFrom.HasValue && dateVal < rule.DateFrom.Value)
                                {
                                    logger.AddError(operation.OperationId,
                                        $"Дата '{dateVal:yyyy-MM-dd}' меньше допустимой ({rule.DateFrom.Value:yyyy-MM-dd}) в столбце '{rule.ColumnName}' (строка {rowIndex})");
                                    parsedRow.IsValid = false;
                                }

                                if (rule.DateTo.HasValue && dateVal > rule.DateTo.Value)
                                {
                                    logger.AddError(operation.OperationId,
                                        $"Дата '{dateVal:yyyy-MM-dd}' превышает допустимую ({rule.DateTo.Value:yyyy-MM-dd}) в столбце '{rule.ColumnName}' (строка {rowIndex})");
                                    parsedRow.IsValid = false;
                                }
                            }
                            else
                            {
                                logger.AddError(operation.OperationId,
                                    $"Неверный формат даты '{value}' в столбце '{rule.ColumnName}' (строка {rowIndex})");
                                parsed = null;
                            }

                            break;

                        case DataType.Bool:
                            if (bool.TryParse(value, out var boolVal))
                            {
                                parsed = boolVal;

                                if (rule.BoolValue.HasValue && boolVal != rule.BoolValue.Value)
                                {
                                    logger.AddError(operation.OperationId,
                                        
                                        $"Булево значение '{boolVal}' не совпадает с ожидаемым '{rule.BoolValue.Value}' в столбце '{rule.ColumnName}' (строка {rowIndex})");
                                    parsedRow.IsValid = false;
                                }
                            }
                            else
                            {
                                logger.AddError(operation.OperationId,
                                    $"Неверный тип значения '{value}' (ожидалось значение true/false) в столбце '{rule.ColumnName}' (строка {rowIndex})");
                                parsed = null;
                            }

                            break;

                        case DataType.String:
                            if (!string.IsNullOrEmpty(rule.Contains) &&
                                !value.Contains(rule.Contains, StringComparison.OrdinalIgnoreCase))
                            {
                                logger.AddError(operation.OperationId,
                                    $"Строка '{value}' не содержит обязательного фрагмента '{rule.Contains}' в столбце '{rule.ColumnName}' (строка {rowIndex})");
                                parsedRow.IsValid = false;
                            }
                            else
                            {
                                parsed = value;
                            }

                            break;

                        default:
                            parsed = null;
                            logger.AddError(operation.OperationId,
                                $"Неизвестный тип данных в правиле для столбца '{rule.ColumnName}' (строка {rowIndex})");
                            break;
                    }
                }
                else
                {
                    logger.AddError(operation.OperationId,
                        $"Отсутствует значение для столбца '{rule.ColumnName}' (строка {rowIndex}, индекс {i}).");
                }


                if (parsedRow.IsValid)
                {
                    parsedRow.ParsedValues[rule.ColumnName] = parsed ?? "NULL";
                }
            }

            if (request.Rules.Count == parsedRow.ParsedValues.Count)
            {
                result.Rows.Add(parsedRow);
            }
        }

        operation.TotalRows = result.Rows.Count;
        operation.FinishedAt = DateTime.Now;

        return (result, operation);
    }
}