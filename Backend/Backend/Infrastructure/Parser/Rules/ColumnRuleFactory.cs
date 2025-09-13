using Backend.Application.DTOs.Request;

namespace Backend.Infrastructure.Parser.Rules;

using Application.DTOs;

public static class ColumnRuleFactory
{
    public static ColumnRuleDto MapRule(ColumnRuleDto dto)
    {
        return dto.DataType switch
        {
            DataType.Decimal or DataType.Double => new ColumnRuleDto
            {
                ColumnName = dto.ColumnName,
                DataType = dto.DataType,
                MinValue = dto.MinValue,
                MaxValue = dto.MaxValue
            },
            DataType.Date => new ColumnRuleDto
            {
                ColumnName = dto.ColumnName,
                DataType = dto.DataType,
                DateFrom = dto.DateFrom,
                DateTo = dto.DateTo
            },
            DataType.Bool => new ColumnRuleDto
            {
                ColumnName = dto.ColumnName,
                DataType = dto.DataType,
                BoolValue = dto.BoolValue
            },
            DataType.String => new ColumnRuleDto
            {
                ColumnName = dto.ColumnName,
                DataType = dto.DataType,
                Contains = dto.Contains
            },
            _ => throw new NotSupportedException($"Unsupported data type {dto.DataType}")
        };
    }
}
