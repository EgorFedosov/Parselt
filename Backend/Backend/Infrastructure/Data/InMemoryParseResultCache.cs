using Backend.Core.Interfaces;
using Backend.Application.DTOs;
using Backend.Application.DTOs.Response;

namespace Backend.Infrastructure.Data;

public class InMemoryParseResultCache : IParseResultCache
{
    private readonly Dictionary<string, ParseResultDto> _cache = new();

    public string Store(ParseResultDto result)
    {
        var key = Guid.NewGuid().ToString();
        _cache[key] = result;
        return key;
    }

    public ParseResultDto? Retrieve(string key)
    {
        return _cache.GetValueOrDefault(key);
    }

    public void Remove(string key) => _cache.Remove(key);
}