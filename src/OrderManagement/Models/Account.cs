namespace OrderManagement.Models;

/// <summary>
/// Represents an account entity for Dataverse integration.
/// </summary>
public class Account
{
    /// <summary>
    /// Unique identifier for the account (Dataverse GUID).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the account.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Account number for reference.
    /// </summary>
    public string AccountNumber { get; set; } = string.Empty;

    /// <summary>
    /// Account rating code.
    /// </summary>
    public int AccountRatingCode { get; set; }

    /// <summary>
    /// Primary email address.
    /// </summary>
    public string EmailAddress1 { get; set; } = string.Empty;

    /// <summary>
    /// Primary telephone number.
    /// </summary>
    public string Telephone1 { get; set; } = string.Empty;

    /// <summary>
    /// Address line 1.
    /// </summary>
    public string Address1_Line1 { get; set; } = string.Empty;

    /// <summary>
    /// Address city.
    /// </summary>
    public string Address1_City { get; set; } = string.Empty;

    /// <summary>
    /// Address state or province.
    /// </summary>
    public string Address1_StateOrProvince { get; set; } = string.Empty;

    /// <summary>
    /// Address postal code.
    /// </summary>
    public string Address1_PostalCode { get; set; } = string.Empty;

    /// <summary>
    /// Address country.
    /// </summary>
    public string Address1_Country { get; set; } = string.Empty;

    /// <summary>
    /// Website URL.
    /// </summary>
    public string WebsiteUrl { get; set; } = string.Empty;

    /// <summary>
    /// Number of employees.
    /// </summary>
    public int? NumberOfEmployees { get; set; }

    /// <summary>
    /// Revenue amount.
    /// </summary>
    public decimal? Revenue { get; set; }

    /// <summary>
    /// Account description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Status code of the account (1=Active, 2=Inactive).
    /// </summary>
    public int StatusCode { get; set; } = 1;

    /// <summary>
    /// State code of the account (0=Active, 1=Inactive).
    /// </summary>
    public int StateCode { get; set; } = 0;

    /// <summary>
    /// Date when the account was created.
    /// </summary>
    public DateTime CreatedOn { get; set; }

    /// <summary>
    /// Date when the account was last modified.
    /// </summary>
    public DateTime ModifiedOn { get; set; }
}
