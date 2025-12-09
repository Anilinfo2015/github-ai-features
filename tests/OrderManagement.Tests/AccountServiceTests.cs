using Microsoft.Extensions.Logging;
using Moq;
using OrderManagement.DTOs;
using OrderManagement.Models;
using OrderManagement.Repositories;
using OrderManagement.Services;

namespace OrderManagement.Tests;

public class AccountServiceTests
{
    private readonly Mock<IAccountRepository> _mockRepository;
    private readonly Mock<ILogger<AccountService>> _mockLogger;
    private readonly AccountService _accountService;

    public AccountServiceTests()
    {
        _mockRepository = new Mock<IAccountRepository>();
        _mockLogger = new Mock<ILogger<AccountService>>();
        _accountService = new AccountService(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllAccountsAsync_ReturnsAllAccounts()
    {
        // Arrange
        var accounts = new List<Account>
        {
            new Account { Id = Guid.NewGuid(), Name = "Contoso Ltd", AccountNumber = "ACC-001" },
            new Account { Id = Guid.NewGuid(), Name = "Fabrikam Inc", AccountNumber = "ACC-002" }
        };
        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(accounts);

        // Act
        var result = await _accountService.GetAllAccountsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAccountByIdAsync_ExistingAccount_ReturnsAccount()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var account = new Account 
        { 
            Id = accountId, 
            Name = "Contoso Ltd",
            AccountNumber = "ACC-001",
            EmailAddress1 = "contact@contoso.com",
            Telephone1 = "555-1234",
            AccountRatingCode = 1
        };
        _mockRepository.Setup(r => r.GetByIdAsync(accountId)).ReturnsAsync(account);

        // Act
        var result = await _accountService.GetAccountByIdAsync(accountId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(accountId, result.Id);
        Assert.Equal("Contoso Ltd", result.Name);
        Assert.Equal("ACC-001", result.AccountNumber);
    }

    [Fact]
    public async Task GetAccountByIdAsync_NonExistingAccount_ReturnsNull()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetByIdAsync(accountId)).ReturnsAsync((Account?)null);

        // Act
        var result = await _accountService.GetAccountByIdAsync(accountId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAccountAsync_ValidAccount_ReturnsCreatedAccount()
    {
        // Arrange
        var createDto = new CreateAccountDto
        {
            Name = "Adventure Works",
            AccountNumber = "ACC-003",
            EmailAddress1 = "info@adventureworks.com",
            Telephone1 = "555-5678",
            Address1_Line1 = "123 Main St",
            Address1_City = "Seattle",
            Address1_StateOrProvince = "WA",
            Address1_PostalCode = "98101",
            Address1_Country = "USA",
            AccountRatingCode = 1
        };

        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Account>()))
            .ReturnsAsync((Account a) => 
            {
                a.Id = Guid.NewGuid();
                return a;
            });

        // Act
        var result = await _accountService.CreateAccountAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Adventure Works", result.Name);
        Assert.Equal("ACC-003", result.AccountNumber);
        Assert.Equal("info@adventureworks.com", result.EmailAddress1);
        Assert.Equal(1, result.StatusCode);
        Assert.Equal(0, result.StateCode);
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Account>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAccountAsync_ExistingAccount_ReturnsUpdatedAccount()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var existingAccount = new Account
        {
            Id = accountId,
            Name = "Contoso Ltd",
            AccountNumber = "ACC-001",
            EmailAddress1 = "contact@contoso.com",
            Telephone1 = "555-1234",
            AccountRatingCode = 1
        };

        var updateDto = new UpdateAccountDto
        {
            Name = "Contoso Corporation",
            EmailAddress1 = "info@contoso.com",
            AccountRatingCode = 2
        };

        _mockRepository.Setup(r => r.GetByIdAsync(accountId)).ReturnsAsync(existingAccount);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Account>())).ReturnsAsync((Account a) => a);

        // Act
        var result = await _accountService.UpdateAccountAsync(accountId, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Contoso Corporation", result.Name);
        Assert.Equal("info@contoso.com", result.EmailAddress1);
        Assert.Equal(2, result.AccountRatingCode);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Account>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAccountAsync_NonExistingAccount_ReturnsNull()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var updateDto = new UpdateAccountDto { Name = "Updated Name" };
        _mockRepository.Setup(r => r.GetByIdAsync(accountId)).ReturnsAsync((Account?)null);

        // Act
        var result = await _accountService.UpdateAccountAsync(accountId, updateDto);

        // Assert
        Assert.Null(result);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAccountAsync_ExistingAccount_ReturnsTrue()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        _mockRepository.Setup(r => r.DeleteAsync(accountId)).ReturnsAsync(true);

        // Act
        var result = await _accountService.DeleteAccountAsync(accountId);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.DeleteAsync(accountId), Times.Once);
    }

    [Fact]
    public async Task DeleteAccountAsync_NonExistingAccount_ReturnsFalse()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        _mockRepository.Setup(r => r.DeleteAsync(accountId)).ReturnsAsync(false);

        // Act
        var result = await _accountService.DeleteAccountAsync(accountId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetAccountsByNameAsync_ReturnsMatchingAccounts()
    {
        // Arrange
        var accounts = new List<Account>
        {
            new Account { Id = Guid.NewGuid(), Name = "Contoso Ltd", AccountNumber = "ACC-001" },
            new Account { Id = Guid.NewGuid(), Name = "Contoso Corporation", AccountNumber = "ACC-003" }
        };
        _mockRepository.Setup(r => r.GetByNameAsync("Contoso")).ReturnsAsync(accounts);

        // Act
        var result = await _accountService.GetAccountsByNameAsync("Contoso");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(r => r.GetByNameAsync("Contoso"), Times.Once);
    }
}
