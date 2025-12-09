using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Moq;
using OrderManagement.Data;
using OrderManagement.Models;
using OrderManagement.Repositories;

namespace OrderManagement.Tests;

public class OrderRepositoryTests
{
    private readonly Mock<IDataverseConnection> _mockDataverseConnection;
    private readonly Mock<ILogger<OrderRepository>> _mockLogger;

    public OrderRepositoryTests()
    {
        _mockDataverseConnection = new Mock<IDataverseConnection>();
        _mockLogger = new Mock<ILogger<OrderRepository>>();
    }

    [Fact]
    public void Constructor_WithNullDataverseConnection_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new OrderRepository(null!, _mockLogger.Object));

        Assert.Equal("dataverseConnection", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new OrderRepository(_mockDataverseConnection.Object, null!));

        Assert.Equal("logger", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Act
        var repository = new OrderRepository(_mockDataverseConnection.Object, _mockLogger.Object);

        // Assert
        Assert.NotNull(repository);
    }
}

/// <summary>
/// Tests for the SanitizeForLikeQuery helper method.
/// Uses reflection to test the private method, or we can test it indirectly through public methods.
/// </summary>
public class OrderRepositorySanitizationTests
{
    [Theory]
    [InlineData("John", "John")]
    [InlineData("John%Doe", "John[%]Doe")]
    [InlineData("John_Doe", "John[_]Doe")]
    [InlineData("John[Test]", "John[[]Test]")]
    [InlineData("%_[", "[%][_][[]")]
    [InlineData("", "")]
    [InlineData(null, null)]
    public void SanitizeForLikeQuery_EscapesSpecialCharacters(string? input, string? expected)
    {
        // Use reflection to test the private static method
        var methodInfo = typeof(OrderRepository).GetMethod(
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
/// Tests for the entity mapping logic in OrderRepository.
/// </summary>
public class OrderRepositoryMappingTests
{
    [Fact]
    public void MapToOrder_WithValidEntity_ReturnsCorrectOrder()
    {
        // Use reflection to test the private static MapToOrder method
        var methodInfo = typeof(OrderRepository).GetMethod(
            "MapToOrder",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        Assert.NotNull(methodInfo);

        // Arrange
        var entityId = Guid.NewGuid();
        var entity = new Entity("crd4d_cr_order", entityId);
        entity["crd4d_ordernumber"] = "ORD-001";
        entity["crd4d_customername"] = "John Doe";
        entity["crd4d_customeremail"] = "john@example.com";
        entity["crd4d_totalamount"] = 150.50m;
        entity["crd4d_status"] = 1; // Confirmed
        entity["crd4d_shippingaddress"] = "123 Main St";
        entity["crd4d_description"] = "Test order";
        entity["createdon"] = new DateTime(2024, 1, 15, 10, 30, 0);
        entity["modifiedon"] = new DateTime(2024, 1, 16, 14, 45, 0);

        // Act
        var result = (Order?)methodInfo.Invoke(null, new object[] { entity });

        // Assert
        Assert.NotNull(result);
        Assert.Equal(entityId, result.Id);
        Assert.Equal("ORD-001", result.OrderNumber);
        Assert.Equal("John Doe", result.CustomerName);
        Assert.Equal("john@example.com", result.CustomerEmail);
        Assert.Equal(150.50m, result.TotalAmount);
        Assert.Equal(OrderStatus.Confirmed, result.Status);
        Assert.Equal("123 Main St", result.ShippingAddress);
        Assert.Equal("Test order", result.Description);
        Assert.Equal(new DateTime(2024, 1, 15, 10, 30, 0), result.CreatedOn);
        Assert.Equal(new DateTime(2024, 1, 16, 14, 45, 0), result.ModifiedOn);
    }

    [Fact]
    public void MapToOrder_WithNullValues_ReturnsOrderWithDefaults()
    {
        // Use reflection to test the private static MapToOrder method
        var methodInfo = typeof(OrderRepository).GetMethod(
            "MapToOrder",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        Assert.NotNull(methodInfo);

        // Arrange
        var entityId = Guid.NewGuid();
        var entity = new Entity("crd4d_cr_order", entityId);
        // Leave all attributes null

        // Act
        var result = (Order?)methodInfo.Invoke(null, new object[] { entity });

        // Assert
        Assert.NotNull(result);
        Assert.Equal(entityId, result.Id);
        Assert.Equal(string.Empty, result.OrderNumber);
        Assert.Equal(string.Empty, result.CustomerName);
        Assert.Equal(string.Empty, result.CustomerEmail);
        Assert.Equal(0m, result.TotalAmount);
        Assert.Equal(OrderStatus.Pending, result.Status);
        Assert.Equal(string.Empty, result.ShippingAddress);
        Assert.Equal(string.Empty, result.Description);
        Assert.Equal(DateTime.MinValue, result.CreatedOn);
        Assert.Equal(DateTime.MinValue, result.ModifiedOn);
    }

    [Fact]
    public void MapToEntity_WithValidOrder_ReturnsCorrectEntity()
    {
        // Use reflection to test the private static MapToEntity method
        var methodInfo = typeof(OrderRepository).GetMethod(
            "MapToEntity",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        Assert.NotNull(methodInfo);

        // Arrange
        var order = new Order
        {
            Id = Guid.NewGuid(),
            OrderNumber = "ORD-002",
            CustomerName = "Jane Smith",
            CustomerEmail = "jane@example.com",
            TotalAmount = 250.75m,
            Status = OrderStatus.Processing,
            ShippingAddress = "456 Oak Ave",
            Description = "Another test order"
        };

        // Act
        var result = (Entity?)methodInfo.Invoke(null, new object[] { order });

        // Assert
        Assert.NotNull(result);
        Assert.Equal("crd4d_cr_order", result.LogicalName);
        Assert.Equal("ORD-002", result["crd4d_ordernumber"]);
        Assert.Equal("Jane Smith", result["crd4d_customername"]);
        Assert.Equal("jane@example.com", result["crd4d_customeremail"]);
        Assert.Equal(250.75m, result["crd4d_totalamount"]);
        Assert.Equal(2, (int)result["crd4d_status"]); // Processing = 2
        Assert.Equal("456 Oak Ave", result["crd4d_shippingaddress"]);
        Assert.Equal("Another test order", result["crd4d_description"]);
    }
}
