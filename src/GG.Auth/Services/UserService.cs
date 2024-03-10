using GG.Auth.Entities;
using GG.Auth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace GG.Auth.Services;

public class UserService(
    UserManager<User> userManager, 
    RoleManager<UserRole> roleManager)
{
    public async Task<IdentityResult> CreateUser(UserCreation userCreation, CancellationToken cancellationToken)
    {
        if (userManager.Users.Any(u => u.Email == userCreation.Email))
        {
            throw new Exception($"User with email address {userCreation.Email} already registered");
        }

        var newUser = new User
        {
            UserName = userCreation.Email,
            Name = userCreation.Name,
            Email = userCreation.Email
        };

        var result = await userManager.CreateAsync(newUser);

        return result;
    }

    public async Task<User?> GetUserById(Guid userId)
    {
        var result = await GetUserById(userId.ToString());

        return result;
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

    public async Task<IEnumerable<User>> GetUsers(CancellationToken cancellationToken)
    {
        var result = await userManager.Users.ToListAsync(cancellationToken);

        return result;
    }

    public async Task<IEnumerable<User>> GetUsers(int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        var result = await userManager.Users
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

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

    public async Task<bool> LockUser(Guid userId)
    {
        var user = await GetUserById(userId) ??
            throw new Exception("Error in removing user role!");

        var lockoutEnabledResult = await userManager.SetLockoutEnabledAsync(user, true);

        if(!lockoutEnabledResult.Succeeded)
        {
            return false;
        }

        var lockoutEndResult = await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);

        if (!lockoutEndResult.Succeeded)
        {
            return false;
        }

        return true;
    }
}