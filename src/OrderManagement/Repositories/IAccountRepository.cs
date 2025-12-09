using OrderManagement.Models;

namespace OrderManagement.Repositories;

/// <summary>
/// Interface for Account repository operations.
/// </summary>
public interface IAccountRepository
{
    /// <summary>
    /// Gets all accounts from Dataverse.
    /// </summary>
    Task<IEnumerable<Account>> GetAllAsync();

    /// <summary>
    /// Gets an account by its ID.
    /// </summary>
    /// <param name="id">The account ID.</param>
    Task<Account?> GetByIdAsync(Guid id);

    /// <summary>
    /// Creates a new account in Dataverse.
    /// </summary>
    /// <param name="account">The account to create.</param>
    Task<Account> CreateAsync(Account account);

    /// <summary>
    /// Updates an existing account in Dataverse.
    /// </summary>
    /// <param name="account">The account to update.</param>
    Task<Account> UpdateAsync(Account account);

    /// <summary>
    /// Deletes an account from Dataverse.
    /// </summary>
    /// <param name="id">The account ID to delete.</param>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// Gets accounts by name.
    /// </summary>
    /// <param name="name">The account name to search for.</param>
    Task<IEnumerable<Account>> GetByNameAsync(string name);
}
