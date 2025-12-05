using Microsoft.Extensions.Logging;
using Moq;
using OrderManagement.Data;

namespace OrderManagement.Tests;

public class DataverseConnectionTests
{
    private readonly Mock<ILogger<DataverseConnection>> _mockLogger;
    private readonly DataverseSettings _validSettings;

    public DataverseConnectionTests()
    {
        _mockLogger = new Mock<ILogger<DataverseConnection>>();
        _validSettings = new DataverseSettings
        {
            Url = "https://test-org.crm.dynamics.com",
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret",
            TenantId = "test-tenant-id"
        };
    }

    [Fact]
    public void Constructor_WithNullSettings_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => 
            new DataverseConnection(null!, _mockLogger.Object));
        
        Assert.Equal("settings", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => 
            new DataverseConnection(_validSettings, null!));
        
        Assert.Equal("logger", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Act
        using var connection = new DataverseConnection(_validSettings, _mockLogger.Object);

        // Assert
        Assert.NotNull(connection);
        Assert.False(connection.IsConnected);
    }

    [Fact]
    public void IsConnected_BeforeConnect_ReturnsFalse()
    {
        // Arrange
        using var connection = new DataverseConnection(_validSettings, _mockLogger.Object);

        // Act & Assert
        Assert.False(connection.IsConnected);
    }

    [Fact]
    public void Client_BeforeConnect_ThrowsInvalidOperationException()
    {
        // Arrange
        using var connection = new DataverseConnection(_validSettings, _mockLogger.Object);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => _ = connection.Client);
        Assert.Contains("Dataverse connection is not established", exception.Message);
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        // Arrange
        var connection = new DataverseConnection(_validSettings, _mockLogger.Object);

        // Act & Assert - should not throw
        connection.Dispose();
        connection.Dispose();
    }

    [Fact]
    public void IsConnected_AfterDispose_ReturnsFalse()
    {
        // Arrange
        var connection = new DataverseConnection(_validSettings, _mockLogger.Object);

        // Act
        connection.Dispose();

        // Assert
        Assert.False(connection.IsConnected);
    }
}

public class DataverseSettingsTests
{
    [Fact]
    public void DefaultValues_AreEmptyStrings()
    {
        // Arrange & Act
        var settings = new DataverseSettings();

        // Assert
        Assert.Equal(string.Empty, settings.Url);
        Assert.Equal(string.Empty, settings.ClientId);
        Assert.Equal(string.Empty, settings.ClientSecret);
        Assert.Equal(string.Empty, settings.TenantId);
    }

    [Fact]
    public void Properties_CanBeSetAndRetrieved()
    {
        // Arrange
        var settings = new DataverseSettings
        {
            Url = "https://test.crm.dynamics.com",
            ClientId = "client-123",
            ClientSecret = "secret-456",
            TenantId = "tenant-789"
        };

        // Assert
        Assert.Equal("https://test.crm.dynamics.com", settings.Url);
        Assert.Equal("client-123", settings.ClientId);
        Assert.Equal("secret-456", settings.ClientSecret);
        Assert.Equal("tenant-789", settings.TenantId);
    }
}
