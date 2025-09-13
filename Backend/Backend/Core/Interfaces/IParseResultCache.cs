using Backend.Application.DTOs.Response;

namespace Backend.Core.Interfaces;

public interface IParseResultCache
{
    string Store(ParseResultDto result);
    ParseResultDto? Retrieve(string key);
    void Remove(string key);
}