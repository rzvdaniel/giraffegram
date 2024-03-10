using GG.Auth.Entities;
using GG.Auth.Enums;
using GG.Auth.Models;
using GG.Auth.Services;
using GG.Core.Models;
using GG.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GG.Api.Controllers;

public class UsersController(UserService userService, AppEmailService appEmailService) : AppControllerBase
{

    [HttpPost]
    [Authorize(UserRoles.Administrator)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> Create([FromBody] UserCreation userCreation, CancellationToken cancellationToken)
    {
        var existingUser = await userService.GetUserByEmailOrUserName(userCreation.Email);

        if (existingUser != null)
        {
            return Conflict();
        }

        var result = await userService.CreateUser(userCreation, cancellationToken);

        if (!result.Succeeded)
        {
            return BadRequest();
        }

        var user = await userService.GetUserByEmailOrUserName(userCreation.Email);

        if (user == null)
        {
            return BadRequest();
        }

        await userService.AddUserToRole(user.Id, UserRoles.User);

        var userDetails = new UserDetails { Email = userCreation.Email, Name = userCreation.Name };
        await appEmailService.SendRegistrationEmail(userDetails, cancellationToken);

        return Created();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<User>>> Get(CancellationToken cancellationToken)
    {
        var users = await userService.GetUsers(cancellationToken);

        return Ok(users);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<User>> Get(Guid id)
    {
        var user = await userService.GetUserById(id);

        return user is not null ? Ok(user) : NotFound();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Delete(Guid id)
    {
        var result = await userService.LockUser(id);

        return result ? Ok() : BadRequest();
    }
}
