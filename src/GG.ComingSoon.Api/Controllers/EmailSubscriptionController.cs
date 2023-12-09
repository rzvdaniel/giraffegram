using GG.ComingSoon.Core;
using GG.ComingSoon.Core.Dto;
using Microsoft.AspNetCore.Mvc;

namespace GG.CommingSoon.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class EmailSubscriptionController : ControllerBase
{
    private readonly ILogger<EmailSubscriptionController> logger;
    private readonly EmailSubscriptionService emailSubscriptionService;

    public EmailSubscriptionController(EmailSubscriptionService emailSubscriptionService, ILogger<EmailSubscriptionController> logger)
    {
        this.logger = logger;
        this.emailSubscriptionService = emailSubscriptionService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Create(AddEmailSubscriptionDto emailSubscription, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await emailSubscriptionService.Add(emailSubscription, cancellationToken);

        return Ok($"Email {emailSubscription.Email} subscribed!");
    }
}
