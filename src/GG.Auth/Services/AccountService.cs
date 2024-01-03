using GG.Auth.Entities;
using GG.Auth.Models;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace GG.Auth.Services;

public class AccountService(
    UserManager<ApplicationUser> userManager, 
    RoleManager<ApplicationRole> roleManager,
    IOpenIddictApplicationManager applicationManager)
{
    public async Task<IdentityResult> CreateUser(ApplicationUser user, string password)
    {
        if (userManager.Users.Any(u => u.UserName == user.UserName))
        {
            throw new Exception($"User with user name {user.UserName} already registered");
        }

        if (userManager.Users.Any(u => u.Email == user.Email))
        {
            throw new Exception($"User with email address {user.Email} already registered");
        }

        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            result.Errors.Aggregate(
                "User Creation Failed - Identity Exception. Errors were: \n\r\n\r",
                (current, error) => current + " - " + error.Description + "\n\r"); 
        }

        return result;
    }

    public async Task<object?> CreateClient(ClientRegister client, CancellationToken cancellationToken)
    {
        if (await applicationManager.FindByClientIdAsync(client.ClientId) != null)
        {
            throw new Exception($"Client with id {client.ClientId} already registered");
        }

        var result = await applicationManager.CreateAsync(new OpenIddictApplicationDescriptor
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

        return result;
    }

    public async Task UpdatePassword(Guid userId, string oldPassword, string newPassword)
    {
        var user = await GetUserById(userId) ?? 
            throw new Exception("Error in updating password!");

        var result = await userManager.ChangePasswordAsync(user, oldPassword, newPassword);

        if (!result.Succeeded)
        {
            throw new Exception("Error in updating password!");
        }
    }

    public async Task UpdateProfile(UserProfile userProfile)
    {
        var user = await GetUserById(userProfile.Id) ??
             throw new Exception("Error in updating Profile!"); ;

        user.UserName = userProfile.UserName;
        user.Email = userProfile.Email;

        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            throw new Exception("Error in updating Profile!");
        }
    }

    public async Task<ApplicationUser?> GetUserById(Guid userId)
    {
        var result = await userManager.FindByIdAsync(userId.ToString());

        return result;
    }

    public async Task<ApplicationUser?> GetUserById(string userId)
    {
        var result = await userManager.FindByIdAsync(userId);

        return result;
    }

    public async Task<ApplicationUser?> GetUserByEmailOrUserName(string emailOrUserName)
    {
        var user = await userManager.FindByEmailAsync(emailOrUserName) ??
            await userManager.FindByNameAsync(emailOrUserName);

        return user;
    }

    public async Task<object?> GetClientById(string clientId)
    {
        var result = await applicationManager.FindByClientIdAsync(clientId);

        return result;
    }

    public async Task<bool> ValidateCredentialsAsync(string usernameOrEmail, string password)
    {
        var user = await userManager.FindByEmailAsync(usernameOrEmail) ??
            await userManager.FindByNameAsync(usernameOrEmail);

        if (user != null)
        {
            return await userManager.CheckPasswordAsync(user, password);
        }

        return false;
    }

    public async Task<bool> ValidatePasswordAsync(ApplicationUser user, string password)
    {
        if (user != null)
        {
            return await userManager.CheckPasswordAsync(user, password);
        }

        return false;
    }

    public async Task<IEnumerable<ApplicationUser>> GetUsers(string firstName, string lastName, string email, int pageIndex, int pageSize)
    {
        await Task.CompletedTask;
        var result = userManager.Users.Where(u =>
            (string.IsNullOrEmpty(email) || u.Email == email))
            .Skip(pageIndex * pageSize)
            .Take(pageSize);

        return result;
    }

    public async Task<string> GetResetPasswordToken(Guid userId)
    {
        var user = await GetUserById(userId);
        if (user != null)
        {
            return await userManager.GeneratePasswordResetTokenAsync(user);
        }

        throw new Exception();
    }

    public async Task ResetPassword(Guid userId, string token, string password)
    {
        var user = await GetUserById(userId);

        if (user != null)
        {
            var result = await userManager.ResetPasswordAsync(user, token, password);

            if (!result.Succeeded)
            {
                throw new Exception($"Error in resetting password");
            }
        }

        throw new Exception($"User with userId {userId} not found!");
    }

    public async Task<IEnumerable<string>> GetRoles(Guid userId)
    {
        var user = await GetUserById(userId) ??
             throw new Exception("Error in retrieving user roles!");

        return await userManager.GetRolesAsync(user);
    }

    public async Task AddRole(Guid userId, string roleName)
    {
        var user = await GetUserById(userId) ??
            throw new Exception("Error in adding user role!");

        var result = await userManager.AddToRoleAsync(user, roleName);
        if (!result.Succeeded)
        {
            throw new Exception("Error in Adding Role!");
        }
    }

    public async Task AddRole(Guid userId, Guid roleId)
    {
        var role = await roleManager.FindByIdAsync(roleId.ToString())
            ?? throw new ArgumentException("Invalid argument", nameof(roleId));

        if (string.IsNullOrEmpty(role.Name))
            throw new Exception("Invalid role");

        var user = await GetUserById(userId) ??
            throw new Exception("Error in adding user role!");

        var result = await userManager.AddToRoleAsync(user, role.Name);

        if (!result.Succeeded)
        {
            throw new Exception("Error in Adding Role!");
        }
    }

    public async Task RemoveRole(Guid userId, string roleName)
    {
        var user = await GetUserById(userId) ??
            throw new Exception("Error in removing user role!");

        var result = await userManager.RemoveFromRoleAsync(user, roleName);
        if (!result.Succeeded)
        {
            throw new Exception("Error in Removing Role!");
        }
    }

    public async Task RemoveRole(Guid userId, Guid roleId)
    {
        var role = await roleManager.FindByIdAsync(roleId.ToString())
            ?? throw new ArgumentException("Invalid argument", nameof(roleId));

        if (string.IsNullOrEmpty(role.Name))
            throw new Exception("Invalid role");

        var user = await GetUserById(userId) ??
            throw new Exception("Error in removing user role!");

        var result = await userManager.RemoveFromRoleAsync(user, role.Name);

        if (!result.Succeeded)
        {
            throw new Exception("Error in Removing Role!");
        }
    }

    public async Task<Dictionary<string, object>> GetUserClaims(ApplicationUser user)
    {
        // Note: the complete list of standard claims supported by the OpenID Connect specification
        // can be found here: http://openid.net/specs/openid-connect-core-1_0.html#StandardClaims

        var claims = new Dictionary<string, object>(StringComparer.Ordinal)
        {
            // Note: the "sub" claim is a mandatory claim and must be included in the JSON response.
            [Claims.Subject] = await userManager.GetUserIdAsync(user),
            [Claims.Email] = await userManager.GetEmailAsync(user)??string.Empty,
            [Claims.EmailVerified] = await userManager.IsEmailConfirmedAsync(user),
            [Claims.Role] = await userManager.GetRolesAsync(user)
        };

        return claims;
    }
}