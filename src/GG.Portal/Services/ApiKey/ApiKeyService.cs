using GG.Portal.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace GG.Portal.Services.ApiKey;

public class ApiKeyService(ApplicationDbContext dbContext)
{
    public async Task<IEnumerable<ApiKeyGetQuery>> List(Guid userId, CancellationToken cancellationToken)
    {
        var apiKeys = await dbContext.ApiKeys
            .Where(x => x.ApiKeyUsers.Any(x => x.UserId == userId))
            .Select(x => new ApiKeyGetQuery
            {
                Id = x.Id,
                Name = x.Name,
                Created = x.Created,
                Updated = x.Updated
            })
            .ToListAsync(cancellationToken);

        return apiKeys;
    }

    public async Task<ApiKeyGetQuery?> Get(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        var apiKey = await dbContext.ApiKeys.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id &&
            x.ApiKeyUsers.Any(x => x.UserId == userId), cancellationToken);

        return apiKey is not null ?
            new ApiKeyGetQuery
            {
                Id = apiKey.Id,
                Name = apiKey.Name,
                Created = apiKey.Created,
                Updated = apiKey.Updated
            } :
            null;
    }

    public async Task<Guid> GetUserId(string key, CancellationToken cancellationToken)
    {
        var userApiKeyHash = CreateSHA512Hash(key);

        var apiKey = await dbContext.ApiKeys
            .Include(x => x.ApiKeyUsers)
            .SingleOrDefaultAsync(x => x.Key == userApiKeyHash, cancellationToken) ??
            throw new Exception("Api key not found");

        var apiKeyUser = apiKey.ApiKeyUsers.FirstOrDefault() ??
            throw new Exception("Api key user not found"); ;

        return apiKeyUser.UserId;
    }

    public async Task<ApiKeyCreateResult> Create(ApiKeyCreateCommand emailAccountDto, Guid userId, CancellationToken cancellationToken)
    {
        var key = CreateSecureRandomString();
        var hashedKey = CreateSHA512Hash(key);

        var apiKey = new ApiKeyEntity
        {
            Id = Guid.NewGuid(),
            Name = emailAccountDto.Name,
            Key = hashedKey,
            Created = DateTime.UtcNow
        };

        dbContext.ApiKeys.Add(apiKey);

        var apiKeyUser = new ApiKeyUserEntity
        {
            ApiKeyId = apiKey.Id,
            UserId = userId
        };

        dbContext.ApiKeyUsers.Add(apiKeyUser);

        await dbContext.SaveChangesAsync(cancellationToken);

        var apiKeyCreateDto = new ApiKeyCreateResult
        {
            Id = apiKey.Id,
            Name = apiKey.Name,
            Key = key,
            CreatedAt = apiKey.Created
        };

        return apiKeyCreateDto;
    }

    public async Task<bool> Update(Guid id, ApiKeyUpdateCommand apiKeyUpdateDto, Guid userId, CancellationToken cancellationToken)
    {
        var affected = await dbContext.ApiKeys
            .Include(x => x.ApiKeyUsers)
            .Where(x => x.Id == id && x.ApiKeyUsers.Any(x => x.UserId == userId))
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(m => m.Name, apiKeyUpdateDto.Name)
                .SetProperty(m => m.Updated, DateTime.UtcNow),
                cancellationToken: cancellationToken);

        return affected == 1;
    }

    public async Task<bool> Delete(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);

        var affected = await dbContext.ApiKeys
            .Where(x => x.Id == id && x.ApiKeyUsers.Any(x => x.UserId == userId))
            .ExecuteDeleteAsync(cancellationToken: cancellationToken);

        return affected == 1;
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
    private static string CreateSHA512Hash(string value)
    {
        using var sha = SHA512.Create();

        var bytes = Encoding.UTF8.GetBytes(value);
        var hash = sha.ComputeHash(bytes);

        return Convert.ToBase64String(hash);
    }
}