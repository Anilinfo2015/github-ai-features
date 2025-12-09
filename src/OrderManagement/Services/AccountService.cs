using OrderManagement.DTOs;
using OrderManagement.Models;
using OrderManagement.Repositories;

namespace OrderManagement.Services;

/// <summary>
/// Service for Account business logic operations.
/// </summary>
public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ILogger<AccountService> _logger;

    /// <summary>
    /// Initializes a new instance of the AccountService class.
    /// </summary>
    public AccountService(IAccountRepository accountRepository, ILogger<AccountService> logger)
    {
        _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets all accounts.
    /// </summary>
    public async Task<IEnumerable<AccountDto>> GetAllAccountsAsync()
    {
        _logger.LogInformation("Getting all accounts");
        
        var accounts = await _accountRepository.GetAllAsync();
        return accounts.Select(MapToDto);
    }

    /// <summary>
    /// Gets an account by its ID.
    /// </summary>
    public async Task<AccountDto?> GetAccountByIdAsync(Guid id)
    {
        _logger.LogInformation("Getting account {AccountId}", id);
        
        var account = await _accountRepository.GetByIdAsync(id);
        return account != null ? MapToDto(account) : null;
    }

    /// <summary>
    /// Creates a new account.
    /// </summary>
    public async Task<AccountDto> CreateAccountAsync(CreateAccountDto createAccountDto)
    {
        _logger.LogInformation("Creating new account {AccountName}", createAccountDto.Name);

        var account = new Account
        {
            Name = createAccountDto.Name,
            AccountNumber = createAccountDto.AccountNumber,
            AccountRatingCode = createAccountDto.AccountRatingCode,
            EmailAddress1 = createAccountDto.EmailAddress1,
            Telephone1 = createAccountDto.Telephone1,
            Address1_Line1 = createAccountDto.Address1_Line1,
            Address1_City = createAccountDto.Address1_City,
            Address1_StateOrProvince = createAccountDto.Address1_StateOrProvince,
            Address1_PostalCode = createAccountDto.Address1_PostalCode,
            Address1_Country = createAccountDto.Address1_Country,
            WebsiteUrl = createAccountDto.WebsiteUrl,
            NumberOfEmployees = createAccountDto.NumberOfEmployees,
            Revenue = createAccountDto.Revenue,
            Description = createAccountDto.Description,
            StatusCode = 1,
            StateCode = 0,
            CreatedOn = DateTime.UtcNow,
            ModifiedOn = DateTime.UtcNow
        };

        var createdAccount = await _accountRepository.CreateAsync(account);
        
        _logger.LogInformation("Successfully created account {AccountId}", createdAccount.Id);
        
        return MapToDto(createdAccount);
    }

    /// <summary>
    /// Updates an existing account.
    /// </summary>
    public async Task<AccountDto?> UpdateAccountAsync(Guid id, UpdateAccountDto updateAccountDto)
    {
        _logger.LogInformation("Updating account {AccountId}", id);

        var existingAccount = await _accountRepository.GetByIdAsync(id);
        
        if (existingAccount == null)
        {
            _logger.LogWarning("Account {AccountId} not found for update", id);
            return null;
        }

        // Update only provided fields using helper methods
        UpdateIfProvided(updateAccountDto.Name, v => existingAccount.Name = v);
        UpdateIfProvided(updateAccountDto.EmailAddress1, v => existingAccount.EmailAddress1 = v);
        UpdateIfProvided(updateAccountDto.Telephone1, v => existingAccount.Telephone1 = v);
        UpdateIfProvided(updateAccountDto.Address1_Line1, v => existingAccount.Address1_Line1 = v);
        UpdateIfProvided(updateAccountDto.Address1_City, v => existingAccount.Address1_City = v);
        UpdateIfProvided(updateAccountDto.Address1_StateOrProvince, v => existingAccount.Address1_StateOrProvince = v);
        UpdateIfProvided(updateAccountDto.Address1_PostalCode, v => existingAccount.Address1_PostalCode = v);
        UpdateIfProvided(updateAccountDto.Address1_Country, v => existingAccount.Address1_Country = v);
        UpdateIfProvided(updateAccountDto.WebsiteUrl, v => existingAccount.WebsiteUrl = v);
        UpdateIfProvided(updateAccountDto.Description, v => existingAccount.Description = v);
        
        UpdateIfProvided(updateAccountDto.AccountRatingCode, v => existingAccount.AccountRatingCode = v);
        UpdateIfProvided(updateAccountDto.NumberOfEmployees, v => existingAccount.NumberOfEmployees = v);
        UpdateIfProvided(updateAccountDto.Revenue, v => existingAccount.Revenue = v);
        UpdateIfProvided(updateAccountDto.StatusCode, v => existingAccount.StatusCode = v);
        UpdateIfProvided(updateAccountDto.StateCode, v => existingAccount.StateCode = v);

        existingAccount.ModifiedOn = DateTime.UtcNow;

        var updatedAccount = await _accountRepository.UpdateAsync(existingAccount);
        
        _logger.LogInformation("Successfully updated account {AccountId}", id);
        
        return MapToDto(updatedAccount);
    }

    /// <summary>
    /// Updates a field if the provided value is not null or empty.
    /// </summary>
    private static void UpdateIfProvided(string? newValue, Action<string> updateAction)
    {
        if (!string.IsNullOrEmpty(newValue))
        {
            updateAction(newValue);
        }
    }

    /// <summary>
    /// Updates a field if the provided nullable value has a value.
    /// </summary>
    private static void UpdateIfProvided<T>(T? newValue, Action<T> updateAction) where T : struct
    {
        if (newValue.HasValue)
        {
            updateAction(newValue.Value);
        }
    }

    /// <summary>
    /// Deletes an account.
    /// </summary>
    public async Task<bool> DeleteAccountAsync(Guid id)
    {
        _logger.LogInformation("Deleting account {AccountId}", id);
        
        var result = await _accountRepository.DeleteAsync(id);
        
        if (result)
        {
            _logger.LogInformation("Successfully deleted account {AccountId}", id);
        }
        else
        {
            _logger.LogWarning("Failed to delete account {AccountId}", id);
        }
        
        return result;
    }

    /// <summary>
    /// Gets accounts by name.
    /// </summary>
    public async Task<IEnumerable<AccountDto>> GetAccountsByNameAsync(string name)
    {
        _logger.LogInformation("Getting accounts with name {AccountName}", name);
        
        var accounts = await _accountRepository.GetByNameAsync(name);
        return accounts.Select(MapToDto);
    }

    /// <summary>
    /// Maps an Account model to an AccountDto.
    /// </summary>
    private static AccountDto MapToDto(Account account)
    {
        return new AccountDto
        {
            Id = account.Id,
            Name = account.Name,
            AccountNumber = account.AccountNumber,
            AccountRatingCode = account.AccountRatingCode,
            EmailAddress1 = account.EmailAddress1,
            Telephone1 = account.Telephone1,
            Address1_Line1 = account.Address1_Line1,
            Address1_City = account.Address1_City,
            Address1_StateOrProvince = account.Address1_StateOrProvince,
            Address1_PostalCode = account.Address1_PostalCode,
            Address1_Country = account.Address1_Country,
            WebsiteUrl = account.WebsiteUrl,
            NumberOfEmployees = account.NumberOfEmployees,
            Revenue = account.Revenue,
            Description = account.Description,
            StatusCode = account.StatusCode,
            StateCode = account.StateCode,
            CreatedOn = account.CreatedOn,
            ModifiedOn = account.ModifiedOn
        };
    }
}
