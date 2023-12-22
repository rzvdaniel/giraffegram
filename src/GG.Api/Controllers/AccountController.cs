using GG.Auth.Entities;
using GG.Auth.Models;
using GG.Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GG.Api.Controllers;

public class AccountController(UserManagerService userManagerService) : BaseController
{
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterUser model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingUser = await userManagerService.GetUserByEmailOrUserName(model.Email);

        if (existingUser != null)
        {
            return StatusCode(StatusCodes.Status409Conflict);
        }

        var newUser = new ApplicationUser { UserName = model.Email, Email = model.Email };

        var result = await userManagerService.CreateUser(newUser, model.Password);

        if (!result.Succeeded)
        {
            AddErrors(result);
        }

        return Ok();
    }
}
