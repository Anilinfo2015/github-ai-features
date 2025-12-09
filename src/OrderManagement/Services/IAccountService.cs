using OrderManagement.DTOs;

namespace OrderManagement.Services;

/// <summary>
/// Interface for Account service operations.
/// </summary>
public interface IAccountService
{
    /// <summary>
    /// Gets all accounts.
    /// </summary>
    Task<IEnumerable<AccountDto>> GetAllAccountsAsync();

    /// <summary>
    /// Gets an account by its ID.
    /// </summary>
    /// <param name="id">The account ID.</param>
    Task<AccountDto?> GetAccountByIdAsync(Guid id);

    /// <summary>
    /// Creates a new account.
    /// </summary>
    /// <param name="createAccountDto">The account creation data.</param>
    Task<AccountDto> CreateAccountAsync(CreateAccountDto createAccountDto);

    /// <summary>
    /// Updates an existing account.
    /// </summary>
    /// <param name="id">The account ID.</param>
    /// <param name="updateAccountDto">The account update data.</param>
    Task<AccountDto?> UpdateAccountAsync(Guid id, UpdateAccountDto updateAccountDto);

    /// <summary>
    /// Deletes an account.
    /// </summary>
    /// <param name="id">The account ID.</param>
    Task<bool> DeleteAccountAsync(Guid id);

    /// <summary>
    /// Gets accounts by name.
    /// </summary>
    /// <param name="name">The account name to search for.</param>
    Task<IEnumerable<AccountDto>> GetAccountsByNameAsync(string name);
}
