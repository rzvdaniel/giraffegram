using GG.Auth.Entities;
using GG.Auth.Models;
using Microsoft.AspNetCore.Identity;

namespace GG.Auth.Services;

public class UserManagerService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
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

    public async Task UpdatePassword(Guid userId, string oldPassword, string newPassword)
    {
        var user = await GetUserById(userId);

        var result = await userManager.ChangePasswordAsync(user, oldPassword, newPassword);

        if (!result.Succeeded)
        {
            throw new Exception("Error in updating password!");
        }
    }

    public async Task UpdateProfile(UserProfile userProfile)
    {
        var user = await GetUserById(userProfile.Id);

        user.FirstName = userProfile.FirstName;
        user.LastName = userProfile.LastName;
        user.UserName = userProfile.UserName;
        user.Email = userProfile.Email;

        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            throw new Exception("Error in updating Profile!");
        }
    }

    public async Task<ApplicationUser> GetUserById(Guid userId)
    {
        var result = await userManager.FindByIdAsync(userId.ToString());
        if (result == null)
            throw new Exception($"User with userId {userId} not found!");

        return result;
    }

    public async Task<ApplicationUser> GetUserByEmailOrUserName(string emailOrUserName)
    {
        var result = await userManager.FindByEmailAsync(emailOrUserName) ??
                     await userManager.FindByNameAsync(emailOrUserName);

        if (result == null)
            throw new Exception($"User with Email or UserName '{emailOrUserName}' not found!");

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
            (string.IsNullOrEmpty(firstName) || (!string.IsNullOrEmpty(u.FirstName) && u.FirstName.StartsWith(firstName))) &&
            (string.IsNullOrEmpty(lastName) || (!string.IsNullOrEmpty(u.LastName) && u.LastName.StartsWith(lastName))) &&
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
        return await userManager.GetRolesAsync(await GetUserById(userId));
    }

    public async Task AddRole(Guid userId, string roleName)
    {
        var result = await userManager.AddToRoleAsync(await GetUserById(userId), roleName);
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

        var result = await userManager.AddToRoleAsync(await GetUserById(userId), role.Name);

        if (!result.Succeeded)
        {
            throw new Exception("Error in Adding Role!");
        }
    }

    public async Task RemoveRole(Guid userId, string roleName)
    {
        var result = await userManager.RemoveFromRoleAsync(await GetUserById(userId), roleName);
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

        var result = await userManager.RemoveFromRoleAsync(await GetUserById(userId), role.Name);

        if (!result.Succeeded)
        {
            throw new Exception("Error in Removing Role!");
        }
    }
}