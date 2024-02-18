using GG.Core.Dto;
using GG.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace GG.Api.Controllers;

public class ApiKeyController(ApiKeyService apiKeyService) : AppControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Create(ApiKeyAddDto apiKey, CancellationToken cancellationToken)
    {
        var apiKeyExists = await apiKeyService.Exists(apiKey.Name, GetUserId(), cancellationToken);

        if (apiKeyExists)
        {
            return Conflict();
        }

        var newApiKey = await apiKeyService.Create(apiKey, GetUserId(), cancellationToken);

        return CreatedAtAction(nameof(Get), new { newApiKey.Id }, newApiKey);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ApiKeyGetDto>>> Get(CancellationToken cancellationToken)
    {
        var apiKeys = await apiKeyService.List(GetUserId(), cancellationToken);

        return Ok(apiKeys);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiKeyGetDto>> Get(Guid id, CancellationToken cancellationToken)
    {
        var apiKey = await apiKeyService.Get(id, GetUserId(), cancellationToken);

        return apiKey is not null ? Ok(apiKey) : NotFound();
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Update(Guid id, ApiKeyUpdateDto apiKey, CancellationToken cancellationToken)
    {
        var result = await apiKeyService.Update(id, apiKey, GetUserId(), cancellationToken);

        return result ? Ok() : NotFound();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await apiKeyService.Delete(id, GetUserId(), cancellationToken);

        return result ? Ok() : NotFound();
    }
}
