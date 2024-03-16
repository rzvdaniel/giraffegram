using GG.Auth.Entities;
using GG.Portal.Data;
using GG.Portal.Enums;
using Microsoft.AspNetCore.Identity;

namespace GG.Portal.Services.Account;

public class AccountService(
    UserManager<ApplicationUser> userManager, 
    RoleManager<UserRoleEntity> roleManager)
{
    public async Task<IdentityResult> CreateUser(UserRegistrationCommand userRegistration, CancellationToken cancellationToken)
    {
        if (userManager.Users.Any(u => u.Email == userRegistration.Email))
        {
            throw new Exception($"User with email address {userRegistration.Email} already registered");
        }

        var newUser = new ApplicationUser
        {
            UserName = userRegistration.Email,
            Name = userRegistration.Name,
            Email = userRegistration.Email
        };

        var result = await userManager.CreateAsync(newUser, userRegistration.Password);

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

    public async Task UpdateProfile(UserProfileUpdateCommand userProfile)
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

    public async Task<bool> AdminExists()
    {
        var users = await userManager.GetUsersInRoleAsync(UserRoles.Administrator);

        return users.Any();
    }

    public async Task<ApplicationUser?> GetUserById(Guid userId)
    {
        var result = await GetUserById(userId.ToString());

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

    public async Task<IEnumerable<ApplicationUser>> GetUsers(string email, int pageIndex, int pageSize)
    {
        await Task.CompletedTask;
        var result = userManager.Users.Where(u =>
            (string.IsNullOrEmpty(email) || u.Email == email))
            .Skip(pageIndex * pageSize)
            .Take(pageSize);

        return result;
    }

    public async Task<string> GetPasswordResetToken(string email, CancellationToken cancellationToken)
    {
        var user = await GetUserByEmailOrUserName(email) ?? 
            throw new Exception("User not found");

        var token = await userManager.GeneratePasswordResetTokenAsync(user);

        return token;
    }

    public async Task<IdentityResult> ResetPassword(ApplicationUser user, string token, string password, CancellationToken cancellationToken)
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
    
    public async Task AddUserToRole(string userId, string roleName)
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
}