﻿using GG.Auth.Entities;
using GG.Auth.Models;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using static OpenIddict.Abstractions.OpenIddictConstants;
using OpenIddict.EntityFrameworkCore.Models;
using GG.Auth.Dtos;

namespace GG.Auth.Services;

public class ClientService(AuthDbContext dbContext, OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication> applicationManager)
{
    public IAsyncEnumerable<OpenIddictEntityFrameworkCoreApplication> List(Guid userId, CancellationToken cancellationToken)
    {
        var applications = applicationManager.ListAsync(apps => 
            apps.Where(app => dbContext.ClientUsers
                .Where(x => x.UserId == userId)
                .Select(x => x.ClientId)
                .Contains(app.ClientId)),
                cancellationToken);

        return applications;
    }

    public async ValueTask<ClientResultDto?> Get(string clientId, Guid userId, CancellationToken cancellationToken)
    {
        var application = await applicationManager.GetAsync(apps =>
            apps.Where(app => app.ClientId == clientId && 
                dbContext.ClientUsers
                .Where(x => x.UserId == userId)
                .Select(x => x.ClientId)
                .Contains(app.ClientId)),
                cancellationToken);

        if (application == null)
            return null;

        var applicationResult = new ClientResultDto
        {
            ClientId = application.ClientId,
            Id = application.Id,
            DisplayName = application.DisplayName
        };

        return applicationResult;
    }

    public async Task<object?> CreateClient(RegisterClient client, Guid userId, CancellationToken cancellationToken)
    {
        var existingApplication = await applicationManager.FindByClientIdAsync(client.ClientId, cancellationToken);

        if (existingApplication != null)
        {
            throw new Exception($"Client with id {client.ClientId} already registered");
        }

        var application = await applicationManager.CreateAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = client.ClientId,
            ClientSecret = client.ClientPassword,
            DisplayName = client.DisplayName,
            Permissions =
            {
                Permissions.Endpoints.Token,
                Permissions.GrantTypes.ClientCredentials
            }
        }, cancellationToken);

        if (application != null)
        {
            var emailHostUser = new ClientUser
            {
                ClientId = client.ClientId,
                UserId = userId
            };

            dbContext.ClientUsers.Add(emailHostUser);

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return application;
    }
}
