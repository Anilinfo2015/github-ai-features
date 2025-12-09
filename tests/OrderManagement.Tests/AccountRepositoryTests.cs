using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Moq;
using OrderManagement.Data;
using OrderManagement.Models;
using OrderManagement.Repositories;

namespace OrderManagement.Tests;

public class AccountRepositoryTests
{
    private readonly Mock<IDataverseConnection> _mockDataverseConnection;
    private readonly Mock<ILogger<AccountRepository>> _mockLogger;

    public AccountRepositoryTests()
    {
        _mockDataverseConnection = new Mock<IDataverseConnection>();
        _mockLogger = new Mock<ILogger<AccountRepository>>();
    }

    [Fact]
    public void Constructor_WithNullDataverseConnection_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new AccountRepository(null!, _mockLogger.Object));

        Assert.Equal("dataverseConnection", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new AccountRepository(_mockDataverseConnection.Object, null!));

        Assert.Equal("logger", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Act
        var repository = new AccountRepository(_mockDataverseConnection.Object, _mockLogger.Object);

        // Assert
        Assert.NotNull(repository);
    }
}

/// <summary>
/// Tests for the SanitizeForLikeQuery helper method.
/// </summary>
public class AccountRepositorySanitizationTests
{
    [Theory]
    [InlineData("Contoso", "Contoso")]
    [InlineData("Contoso%Ltd", "Contoso[%]Ltd")]
    [InlineData("Fabrikam_Inc", "Fabrikam[_]Inc")]
    [InlineData("Adventure[Works]", "Adventure[[]Works]")]
    [InlineData("%_[", "[%][_][[]")]
    [InlineData("", "")]
    [InlineData(null, null)]
    public void SanitizeForLikeQuery_EscapesSpecialCharacters(string? input, string? expected)
    {
        // Use reflection to test the private static method
        var methodInfo = typeof(AccountRepository).GetMethod(
            "SanitizeForLikeQuery",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        Assert.NotNull(methodInfo);

        // Act
        var result = methodInfo.Invoke(null, new object?[] { input });

        // Assert
        Assert.Equal(expected, result);
    }
}

/// <summary>
/// Tests for the entity mapping logic in AccountRepository.
/// </summary>
public class AccountRepositoryMappingTests
{
    [Fact]
    public void MapToAccount_WithValidEntity_ReturnsCorrectAccount()
    {
        // Use reflection to test the private static MapToAccount method
        var methodInfo = typeof(AccountRepository).GetMethod(
            "MapToAccount",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        Assert.NotNull(methodInfo);

        // Arrange
        var entityId = Guid.NewGuid();
        var entity = new Entity("account", entityId);
        entity["name"] = "Contoso Ltd";
        entity["accountnumber"] = "ACC-001";
        entity["accountratingcode"] = 1;
        entity["emailaddress1"] = "contact@contoso.com";
        entity["telephone1"] = "555-1234";
        entity["address1_line1"] = "123 Main St";
        entity["address1_city"] = "Seattle";
        entity["address1_stateorprovince"] = "WA";
        entity["address1_postalcode"] = "98101";
        entity["address1_country"] = "USA";
        entity["websiteurl"] = "https://contoso.com";
        entity["numberofemployees"] = 250;
        entity["revenue"] = new Money(5000000m);
        entity["description"] = "Leading software company";
        entity["statuscode"] = 1;
        entity["statecode"] = 0;
        entity["createdon"] = new DateTime(2024, 1, 15, 10, 30, 0);
        entity["modifiedon"] = new DateTime(2024, 1, 16, 14, 45, 0);

        // Act
        var result = (Account?)methodInfo.Invoke(null, new object[] { entity });

        // Assert
        Assert.NotNull(result);
        Assert.Equal(entityId, result.Id);
        Assert.Equal("Contoso Ltd", result.Name);
        Assert.Equal("ACC-001", result.AccountNumber);
        Assert.Equal(1, result.AccountRatingCode);
        Assert.Equal("contact@contoso.com", result.EmailAddress1);
        Assert.Equal("555-1234", result.Telephone1);
        Assert.Equal("123 Main St", result.Address1_Line1);
        Assert.Equal("Seattle", result.Address1_City);
        Assert.Equal("WA", result.Address1_StateOrProvince);
        Assert.Equal("98101", result.Address1_PostalCode);
        Assert.Equal("USA", result.Address1_Country);
        Assert.Equal("https://contoso.com", result.WebsiteUrl);
        Assert.Equal(250, result.NumberOfEmployees);
        Assert.Equal(5000000m, result.Revenue);
        Assert.Equal("Leading software company", result.Description);
        Assert.Equal(1, result.StatusCode);
        Assert.Equal(0, result.StateCode);
        Assert.Equal(new DateTime(2024, 1, 15, 10, 30, 0), result.CreatedOn);
        Assert.Equal(new DateTime(2024, 1, 16, 14, 45, 0), result.ModifiedOn);
    }

    [Fact]
    public void MapToAccount_WithNullValues_ReturnsAccountWithDefaults()
    {
        // Use reflection to test the private static MapToAccount method
        var methodInfo = typeof(AccountRepository).GetMethod(
            "MapToAccount",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        Assert.NotNull(methodInfo);

        // Arrange
        var entityId = Guid.NewGuid();
        var entity = new Entity("account", entityId);
        // Leave all attributes null

        // Act
        var result = (Account?)methodInfo.Invoke(null, new object[] { entity });

        // Assert
        Assert.NotNull(result);
        Assert.Equal(entityId, result.Id);
        Assert.Equal(string.Empty, result.Name);
        Assert.Equal(string.Empty, result.AccountNumber);
        Assert.Equal(0, result.AccountRatingCode);
        Assert.Equal(string.Empty, result.EmailAddress1);
        Assert.Equal(string.Empty, result.Telephone1);
        Assert.Null(result.NumberOfEmployees);
        Assert.Null(result.Revenue);
        Assert.Equal(DateTime.MinValue, result.CreatedOn);
        Assert.Equal(DateTime.MinValue, result.ModifiedOn);
    }

    [Fact]
    public void MapToEntity_WithValidAccount_ReturnsCorrectEntity()
    {
        // Use reflection to test the private static MapToEntity method
        var methodInfo = typeof(AccountRepository).GetMethod(
            "MapToEntity",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        Assert.NotNull(methodInfo);

        // Arrange
        var account = new Account
        {
            Id = Guid.NewGuid(),
            Name = "Fabrikam Inc",
            AccountNumber = "ACC-002",
            AccountRatingCode = 2,
            EmailAddress1 = "info@fabrikam.com",
            Telephone1 = "555-5678",
            Address1_Line1 = "456 Oak Ave",
            Address1_City = "Portland",
            Address1_StateOrProvince = "OR",
            Address1_PostalCode = "97201",
            Address1_Country = "USA",
            WebsiteUrl = "https://fabrikam.com",
            NumberOfEmployees = 150,
            Revenue = 3500000m,
            Description = "Manufacturing company",
            StatusCode = 1,
            StateCode = 0
        };

        // Act
        var result = (Entity?)methodInfo.Invoke(null, new object[] { account });

        // Assert
        Assert.NotNull(result);
        Assert.Equal("account", result.LogicalName);
        Assert.Equal("Fabrikam Inc", result["name"]);
        Assert.Equal("ACC-002", result["accountnumber"]);
        Assert.Equal(2, (int)result["accountratingcode"]);
        Assert.Equal("info@fabrikam.com", result["emailaddress1"]);
        Assert.Equal("555-5678", result["telephone1"]);
        Assert.Equal("456 Oak Ave", result["address1_line1"]);
        Assert.Equal("Portland", result["address1_city"]);
        Assert.Equal("OR", result["address1_stateorprovince"]);
        Assert.Equal("97201", result["address1_postalcode"]);
        Assert.Equal("USA", result["address1_country"]);
        Assert.Equal("https://fabrikam.com", result["websiteurl"]);
        Assert.Equal(150, (int)result["numberofemployees"]);
        Assert.Equal(3500000m, ((Money)result["revenue"]).Value);
        Assert.Equal("Manufacturing company", result["description"]);
        Assert.Equal(1, (int)result["statuscode"]);
        Assert.Equal(0, (int)result["statecode"]);
    }
}
