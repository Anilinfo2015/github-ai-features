using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using OrderManagement.Data;
using OrderManagement.Models;

namespace OrderManagement.Repositories;

/// <summary>
/// Repository for Account CRUD operations with Dataverse.
/// </summary>
public class AccountRepository : IAccountRepository
{
    private readonly IDataverseConnection _dataverseConnection;
    private readonly ILogger<AccountRepository> _logger;

    // Dataverse entity and attribute names for the Account entity
    private const string EntityName = "account";
    private const string IdField = "accountid";
    private const string NameField = "name";
    private const string AccountNumberField = "accountnumber";
    private const string AccountRatingCodeField = "accountratingcode";
    private const string EmailAddress1Field = "emailaddress1";
    private const string Telephone1Field = "telephone1";
    private const string Address1_Line1Field = "address1_line1";
    private const string Address1_CityField = "address1_city";
    private const string Address1_StateOrProvinceField = "address1_stateorprovince";
    private const string Address1_PostalCodeField = "address1_postalcode";
    private const string Address1_CountryField = "address1_country";
    private const string WebsiteUrlField = "websiteurl";
    private const string NumberOfEmployeesField = "numberofemployees";
    private const string RevenueField = "revenue";
    private const string DescriptionField = "description";
    private const string StatusCodeField = "statuscode";
    private const string StateCodeField = "statecode";

    /// <summary>
    /// Initializes a new instance of the AccountRepository class.
    /// </summary>
    public AccountRepository(IDataverseConnection dataverseConnection, ILogger<AccountRepository> logger)
    {
        _dataverseConnection = dataverseConnection ?? throw new ArgumentNullException(nameof(dataverseConnection));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets all accounts from Dataverse.
    /// </summary>
    public async Task<IEnumerable<Account>> GetAllAsync()
    {
        try
        {
            _logger.LogInformation("Retrieving all accounts from Dataverse");

            var query = new QueryExpression(EntityName)
            {
                ColumnSet = GetRequiredColumnSet()
            };

            var result = await Task.Run(() => _dataverseConnection.Client.RetrieveMultiple(query));
            
            return result.Entities.Select(MapToAccount).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving accounts from Dataverse");
            throw;
        }
    }

    /// <summary>
    /// Gets an account by its ID.
    /// </summary>
    public async Task<Account?> GetByIdAsync(Guid id)
    {
        try
        {
            _logger.LogInformation("Retrieving account {AccountId} from Dataverse", id);

            var entity = await Task.Run(() => 
                _dataverseConnection.Client.Retrieve(EntityName, id, GetRequiredColumnSet()));

            return entity != null ? MapToAccount(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving account {AccountId} from Dataverse", id);
            return null;
        }
    }

    /// <summary>
    /// Creates a new account in Dataverse.
    /// </summary>
    public async Task<Account> CreateAsync(Account account)
    {
        try
        {
            _logger.LogInformation("Creating new account {AccountName} in Dataverse", account.Name);

            var entity = MapToEntity(account);
            
            var newId = await Task.Run(() => _dataverseConnection.Client.Create(entity));
            
            account.Id = newId;
            account.CreatedOn = DateTime.UtcNow;
            account.ModifiedOn = DateTime.UtcNow;

            _logger.LogInformation("Successfully created account {AccountId} in Dataverse", newId);
            
            return account;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating account in Dataverse");
            throw;
        }
    }

    /// <summary>
    /// Updates an existing account in Dataverse.
    /// </summary>
    public async Task<Account> UpdateAsync(Account account)
    {
        try
        {
            _logger.LogInformation("Updating account {AccountId} in Dataverse", account.Id);

            var entity = MapToEntity(account);
            entity.Id = account.Id;

            await Task.Run(() => _dataverseConnection.Client.Update(entity));
            
            account.ModifiedOn = DateTime.UtcNow;

            _logger.LogInformation("Successfully updated account {AccountId} in Dataverse", account.Id);
            
            return account;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating account {AccountId} in Dataverse", account.Id);
            throw;
        }
    }

    /// <summary>
    /// Deletes an account from Dataverse.
    /// </summary>
    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            _logger.LogInformation("Deleting account {AccountId} from Dataverse", id);

            await Task.Run(() => _dataverseConnection.Client.Delete(EntityName, id));

            _logger.LogInformation("Successfully deleted account {AccountId} from Dataverse", id);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting account {AccountId} from Dataverse", id);
            return false;
        }
    }

    /// <summary>
    /// Gets accounts by name.
    /// </summary>
    public async Task<IEnumerable<Account>> GetByNameAsync(string name)
    {
        try
        {
            _logger.LogInformation("Retrieving accounts with name {AccountName} from Dataverse", name);

            // Sanitize account name to prevent LIKE injection attacks
            var sanitizedName = SanitizeForLikeQuery(name);

            var query = new QueryExpression(EntityName)
            {
                ColumnSet = GetRequiredColumnSet(),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression(NameField, ConditionOperator.Like, $"%{sanitizedName}%")
                    }
                }
            };

            var result = await Task.Run(() => _dataverseConnection.Client.RetrieveMultiple(query));
            
            return result.Entities.Select(MapToAccount).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving accounts by name from Dataverse");
            throw;
        }
    }

    /// <summary>
    /// Sanitizes a string for use in LIKE queries by escaping special characters.
    /// </summary>
    private static string SanitizeForLikeQuery(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        // Escape special characters that have meaning in LIKE queries
        return input
            .Replace("[", "[[]")
            .Replace("%", "[%]")
            .Replace("_", "[_]");
    }

    /// <summary>
    /// Gets the ColumnSet with only the required columns for account queries.
    /// </summary>
    private static ColumnSet GetRequiredColumnSet()
    {
        return new ColumnSet(
            IdField,
            NameField,
            AccountNumberField,
            AccountRatingCodeField,
            EmailAddress1Field,
            Telephone1Field,
            Address1_Line1Field,
            Address1_CityField,
            Address1_StateOrProvinceField,
            Address1_PostalCodeField,
            Address1_CountryField,
            WebsiteUrlField,
            NumberOfEmployeesField,
            RevenueField,
            DescriptionField,
            StatusCodeField,
            StateCodeField,
            "createdon",
            "modifiedon"
        );
    }

    /// <summary>
    /// Maps a Dataverse entity to an Account model.
    /// </summary>
    private static Account MapToAccount(Entity entity)
    {
        return new Account
        {
            Id = entity.Id,
            Name = entity.GetAttributeValue<string>(NameField) ?? string.Empty,
            AccountNumber = entity.GetAttributeValue<string>(AccountNumberField) ?? string.Empty,
            AccountRatingCode = entity.GetAttributeValue<int>(AccountRatingCodeField),
            EmailAddress1 = entity.GetAttributeValue<string>(EmailAddress1Field) ?? string.Empty,
            Telephone1 = entity.GetAttributeValue<string>(Telephone1Field) ?? string.Empty,
            Address1_Line1 = entity.GetAttributeValue<string>(Address1_Line1Field) ?? string.Empty,
            Address1_City = entity.GetAttributeValue<string>(Address1_CityField) ?? string.Empty,
            Address1_StateOrProvince = entity.GetAttributeValue<string>(Address1_StateOrProvinceField) ?? string.Empty,
            Address1_PostalCode = entity.GetAttributeValue<string>(Address1_PostalCodeField) ?? string.Empty,
            Address1_Country = entity.GetAttributeValue<string>(Address1_CountryField) ?? string.Empty,
            WebsiteUrl = entity.GetAttributeValue<string>(WebsiteUrlField) ?? string.Empty,
            NumberOfEmployees = entity.GetAttributeValue<int?>(NumberOfEmployeesField),
            Revenue = entity.GetAttributeValue<Money>(RevenueField)?.Value,
            Description = entity.GetAttributeValue<string>(DescriptionField) ?? string.Empty,
            StatusCode = entity.GetAttributeValue<int>(StatusCodeField),
            StateCode = entity.GetAttributeValue<int>(StateCodeField),
            CreatedOn = entity.GetAttributeValue<DateTime?>("createdon") ?? DateTime.MinValue,
            ModifiedOn = entity.GetAttributeValue<DateTime?>("modifiedon") ?? DateTime.MinValue
        };
    }

    /// <summary>
    /// Maps an Account model to a Dataverse entity.
    /// </summary>
    private static Entity MapToEntity(Account account)
    {
        var entity = new Entity(EntityName);
        
        entity[NameField] = account.Name;
        entity[AccountNumberField] = account.AccountNumber;
        entity[AccountRatingCodeField] = account.AccountRatingCode;
        entity[EmailAddress1Field] = account.EmailAddress1;
        entity[Telephone1Field] = account.Telephone1;
        entity[Address1_Line1Field] = account.Address1_Line1;
        entity[Address1_CityField] = account.Address1_City;
        entity[Address1_StateOrProvinceField] = account.Address1_StateOrProvince;
        entity[Address1_PostalCodeField] = account.Address1_PostalCode;
        entity[Address1_CountryField] = account.Address1_Country;
        entity[WebsiteUrlField] = account.WebsiteUrl;
        
        if (account.NumberOfEmployees.HasValue)
        {
            entity[NumberOfEmployeesField] = account.NumberOfEmployees.Value;
        }
        
        if (account.Revenue.HasValue)
        {
            entity[RevenueField] = new Money(account.Revenue.Value);
        }
        
        entity[DescriptionField] = account.Description;
        entity[StatusCodeField] = account.StatusCode;
        entity[StateCodeField] = account.StateCode;

        return entity;
    }
}
