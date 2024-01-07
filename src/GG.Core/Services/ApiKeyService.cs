using GG.Core.Dto;
using GG.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace GG.Core.Services;

public class ApiKeyService(ApplicationDbContext dbContext)
{
    public async Task<IEnumerable<ApiKeyGetDto>> List(Guid userId, CancellationToken cancellationToken)
    {
        var apiKeys = await dbContext.ApiKeys
            .Where(x => x.ApiKeyUsers.Any(x => x.UserId == userId))
            .Select(x => new ApiKeyGetDto
            {
                Id = x.Id,
                Name = x.Name,
                CreatedAt = x.Created,
                UpdatedAt = x.Updated
            })
            .ToListAsync(cancellationToken);

        return apiKeys;
    }

    public async Task<ApiKeyGetDto?> Get(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        var apiKey = await dbContext.ApiKeys.SingleOrDefaultAsync(x => x.Id == id &&
            x.ApiKeyUsers.Any(x => x.UserId == userId), cancellationToken);

        if (apiKey == null)
            return null;

        var apiKeyGetDto = new ApiKeyGetDto
        {
            Id = apiKey.Id,
            Name = apiKey.Name,
            CreatedAt = apiKey.Created,
            UpdatedAt = apiKey.Updated
        };

        return apiKeyGetDto;
    }

    public async Task<ApiKeyCreateDto> Create(ApiKeyAddDto emailAccountDto, Guid userId, CancellationToken cancellationToken)
    {
        var key = CreateSecureRandomString();
        var hashedKey = CreateSHA512Hash(key);

        var apiKey = new ApiKey
        {
            Id = Guid.NewGuid(),
            Name = emailAccountDto.Name,
            Key = hashedKey,
            Created = DateTime.UtcNow
        };

        dbContext.ApiKeys.Add(apiKey);

        var apiKeyUser = new ApiKeyUser
        {
            ApiKeyId = apiKey.Id,
            UserId = userId
        };

        dbContext.ApiKeyUsers.Add(apiKeyUser);

        await dbContext.SaveChangesAsync(cancellationToken);

        var apiKeyCreateDto = new ApiKeyCreateDto
        {
            Id = apiKey.Id,
            Name = apiKey.Name,
            Key = key,
            CreatedAt = apiKey.Created
        };

        return apiKeyCreateDto;
    }

    public void Delete(Guid id, Guid userId)
    {
        var apiKey = dbContext.ApiKeys.SingleOrDefault(x => x.Id == id && x.ApiKeyUsers.Any(x => x.UserId == userId));

        if (apiKey == null)
            return;

        dbContext.ApiKeys.Remove(apiKey);

        dbContext.SaveChanges();
    }

    public async Task<bool> Exists(string name, Guid userId, CancellationToken cancellationToken)
    {
        var apiKeyExists = await dbContext.ApiKeys.AnyAsync(x => x.Name == name && x.ApiKeyUsers.Any(x => x.UserId == userId), cancellationToken);

        return apiKeyExists;
    }

    public bool IsValidApiKey(string userApiKey)
    {
        if (string.IsNullOrWhiteSpace(userApiKey))
            return false;

        var userApiKeyHash = CreateSHA512Hash(userApiKey);

        var apiKey = dbContext.ApiKeys.SingleOrDefault(x => x.Key == userApiKeyHash);

        return apiKey != null;
    }

    /// <summary>
    /// Creates a cryptographically secure random key string.
    /// </summary>
    /// <param name="count">The number of bytes of random values to create the string from</param>
    /// <returns>A secure random string</returns>
    private static string CreateSecureRandomString(int count = 64)
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(count));
    }

    /// <summary>
    /// Computes a SHA512 hash for a string and returns the hash as a Base64 encoded string.
    /// </summary>
    /// <param name="value">The string to operate on</param>
    /// <returns>A SHA512 Base64 encoded string</returns>
    private string CreateSHA512Hash(string value)
    {
        using var sha = SHA512.Create();

        var bytes = Encoding.UTF8.GetBytes(value);
        var hash = sha.ComputeHash(bytes);

        return Convert.ToBase64String(hash);
    }
}