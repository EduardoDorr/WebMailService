using Microsoft.AspNetCore.Mvc;

using WebMail.Application.Dtos;
using WebMail.Application.Interfaces;

namespace WebMail.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class EmailController : ControllerBase
{
    private readonly ILogger<EmailController> _logger;
    private readonly ICreateEmailService _service;

    public EmailController(
        ICreateEmailService createEmail,
        ILogger<EmailController> logger)
    {
        _service = createEmail;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateEmailResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateEmail([FromBody] CreateEmailRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Requisição para criação de um e-mail: {EmailRequest}", request);

        var createEmailResponse = await _service.CreateEmailAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetEmailById), new { id = createEmailResponse.Id }, createEmailResponse);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetEmailResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    public async Task<IActionResult> GetEmailById(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Requisição para buscar um e-mail de id {Id}", id);

        var getEmailResponse = await _service.GetEmailByIdAsync(id, cancellationToken);

        if (getEmailResponse == null)
            return NotFound($"Não há e-mail para o id: {id}");

        return Ok(getEmailResponse);
    }
}