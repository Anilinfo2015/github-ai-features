using Microsoft.AspNetCore.Mvc;
using OrderManagement.DTOs;
using OrderManagement.Services;

namespace OrderManagement.Controllers;

/// <summary>
/// REST API controller for Account management operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly ILogger<AccountController> _logger;

    /// <summary>
    /// Initializes a new instance of the AccountController class.
    /// </summary>
    public AccountController(IAccountService accountService, ILogger<AccountController> logger)
    {
        _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets all accounts.
    /// </summary>
    /// <returns>A list of all accounts.</returns>
    /// <response code="200">Returns the list of accounts.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AccountDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<AccountDto>>> GetAllAccounts()
    {
        try
        {
            _logger.LogInformation("Getting all accounts");
            var accounts = await _accountService.GetAllAccountsAsync();
            return Ok(accounts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all accounts");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving accounts.");
        }
    }

    /// <summary>
    /// Gets an account by ID.
    /// </summary>
    /// <param name="id">The account ID.</param>
    /// <returns>The account with the specified ID.</returns>
    /// <response code="200">Returns the account.</response>
    /// <response code="404">If the account is not found.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AccountDto>> GetAccountById(Guid id)
    {
        try
        {
            _logger.LogInformation("Getting account {AccountId}", id);
            var account = await _accountService.GetAccountByIdAsync(id);

            if (account == null)
            {
                _logger.LogWarning("Account {AccountId} not found", id);
                return NotFound($"Account with ID {id} not found.");
            }

            return Ok(account);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting account {AccountId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the account.");
        }
    }

    /// <summary>
    /// Creates a new account.
    /// </summary>
    /// <param name="createAccountDto">The account creation data.</param>
    /// <returns>The created account.</returns>
    /// <response code="201">Returns the newly created account.</response>
    /// <response code="400">If the request data is invalid.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPost]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AccountDto>> CreateAccount([FromBody] CreateAccountDto createAccountDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating new account {AccountName}", createAccountDto.Name);
            var createdAccount = await _accountService.CreateAccountAsync(createAccountDto);

            return CreatedAtAction(nameof(GetAccountById), new { id = createdAccount.Id }, createdAccount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating account");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the account.");
        }
    }

    /// <summary>
    /// Updates an existing account.
    /// </summary>
    /// <param name="id">The account ID.</param>
    /// <param name="updateAccountDto">The account update data.</param>
    /// <returns>The updated account.</returns>
    /// <response code="200">Returns the updated account.</response>
    /// <response code="400">If the request data is invalid.</response>
    /// <response code="404">If the account is not found.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AccountDto>> UpdateAccount(Guid id, [FromBody] UpdateAccountDto updateAccountDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Updating account {AccountId}", id);
            var updatedAccount = await _accountService.UpdateAccountAsync(id, updateAccountDto);

            if (updatedAccount == null)
            {
                _logger.LogWarning("Account {AccountId} not found for update", id);
                return NotFound($"Account with ID {id} not found.");
            }

            return Ok(updatedAccount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating account {AccountId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the account.");
        }
    }

    /// <summary>
    /// Deletes an account.
    /// </summary>
    /// <param name="id">The account ID.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">If the account was deleted successfully.</response>
    /// <response code="404">If the account is not found.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteAccount(Guid id)
    {
        try
        {
            _logger.LogInformation("Deleting account {AccountId}", id);
            var result = await _accountService.DeleteAccountAsync(id);

            if (!result)
            {
                _logger.LogWarning("Account {AccountId} not found for deletion", id);
                return NotFound($"Account with ID {id} not found.");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting account {AccountId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the account.");
        }
    }

    /// <summary>
    /// Gets accounts by name.
    /// </summary>
    /// <param name="name">The account name to search for.</param>
    /// <returns>A list of accounts matching the name.</returns>
    /// <response code="200">Returns the list of accounts.</response>
    /// <response code="400">If the name is empty.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet("name/{name}")]
    [ProducesResponseType(typeof(IEnumerable<AccountDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<AccountDto>>> GetAccountsByName(string name)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Account name cannot be empty.");
            }

            _logger.LogInformation("Getting accounts with name {AccountName}", name);
            var accounts = await _accountService.GetAccountsByNameAsync(name);
            return Ok(accounts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting accounts by name {AccountName}", name);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving accounts by name.");
        }
    }
}
