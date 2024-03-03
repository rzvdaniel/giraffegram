using GG.Auth.Entities;
using GG.Auth.Enums;
using GG.Auth.Models;
using GG.Core.Dto;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace GG.Auth.Services;

public class AccountService(
    UserManager<User> userManager, 
    RoleManager<UserRole> roleManager,
    IOpenIddictApplicationManager applicationManager)
{
    public async Task<IdentityResult> CreateUser(UserRegisterDto userRegisterDto, CancellationToken cancellationToken)
    {
        if (userManager.Users.Any(u => u.Email == userRegisterDto.Email))
        {
            throw new Exception($"User with email address {userRegisterDto.Email} already registered");
        }

        var newUser = new User
        {
            UserName = userRegisterDto.Email,
            Name = userRegisterDto.Name,
            Email = userRegisterDto.Email
        };

        var result = await userManager.CreateAsync(newUser, userRegisterDto.Password);

        return result;
    }

    public async Task<object?> CreateClient(ApplicationRegistration client, CancellationToken cancellationToken)
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

    public async Task<User?> GetUserById(Guid userId)
    {
        var result = await userManager.FindByIdAsync(userId.ToString());
        return result;
    }

    public async Task<bool> AdminExists()
    {
        var users = await userManager.GetUsersInRoleAsync(UserRoles.Administrator);

        return users.Any();
    }

    public async Task<User?> GetAdmin()
    {
        var users = await userManager.GetUsersInRoleAsync(UserRoles.Administrator);

        return users.SingleOrDefault();
    }

    public async Task<User?> GetUserById(string userId)
    {
        var result = await userManager.FindByIdAsync(userId);

        return result;
    }

    public async Task<User?> GetUserByEmailOrUserName(string emailOrUserName)
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

    public async Task<bool> ValidatePasswordAsync(User user, string password)
    {
        if (user != null)
        {
            return await userManager.CheckPasswordAsync(user, password);
        }

        return false;
    }

    public async Task<IEnumerable<User>> GetUsers(string firstName, string lastName, string email, int pageIndex, int pageSize)
    {
        await Task.CompletedTask;
        var result = userManager.Users.Where(u =>
            (string.IsNullOrEmpty(email) || u.Email == email))
            .Skip(pageIndex * pageSize)
            .Take(pageSize);

        return result;
    }

    public async Task<string> GetPasswordResetToken(ForgotPassword forgotPasswordModel, CancellationToken cancellationToken)
    {
        var user = await GetUserByEmailOrUserName(forgotPasswordModel.Email) ?? 
            throw new Exception("User not found");

        var token = await userManager.GeneratePasswordResetTokenAsync(user);

        return token;
    }

    public async Task<IdentityResult> ResetPassword(User user, string token, string password, CancellationToken cancellationToken)
    {
        var result = await userManager.ResetPasswordAsync(user, token, password);

        return result;
    }

    public async Task<IEnumerable<string>> GetUserRoles(Guid userId)
    {
        var user = await GetUserById(userId) ??
             throw new Exception("Error in retrieving user roles!");

        return await userManager.GetRolesAsync(user);
    }
    
    public async Task AddUserToRole(Guid userId, string roleName)
    {
        var user = await GetUserById(userId) ??
            throw new Exception("Error in adding user role!");

        var result = await userManager.AddToRoleAsync(user, roleName);
        if (!result.Succeeded)
        {
            throw new Exception("Error in adding user role!");
        }
    }

    public async Task AddUserToRole(Guid userId, Guid roleId)
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

    public async Task RemoveUserRole(Guid userId, string roleName)
    {
        var user = await GetUserById(userId) ??
            throw new Exception("Error in removing user role!");

        var result = await userManager.RemoveFromRoleAsync(user, roleName);
        if (!result.Succeeded)
        {
            throw new Exception("Error in Removing Role!");
        }
    }

    public async Task RemoveUserRole(Guid userId, Guid roleId)
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

    public async Task<Dictionary<string, object>> GetUserClaims(User user)
    {
        // Note: the complete list of standard claims supported by the OpenID Connect specification
        // can be found here: http://openid.net/specs/openid-connect-core-1_0.html#StandardClaims

        var claims = new Dictionary<string, object>(StringComparer.Ordinal)
        {
            // Note: the "sub" claim is a mandatory claim and must be included in the JSON response.
            [Claims.Subject] = await userManager.GetUserIdAsync(user),
            [Claims.Email] = await userManager.GetEmailAsync(user)??string.Empty,
            [Claims.EmailVerified] = await userManager.IsEmailConfirmedAsync(user),
            [Claims.Name] = user.Name??string.Empty,
            [Claims.Role] = await userManager.GetRolesAsync(user)
        };

        return claims;
    }
}