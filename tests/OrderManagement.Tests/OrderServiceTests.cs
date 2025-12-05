using Microsoft.Extensions.Logging;
using Moq;
using OrderManagement.DTOs;
using OrderManagement.Models;
using OrderManagement.Repositories;
using OrderManagement.Services;

namespace OrderManagement.Tests;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _mockRepository;
    private readonly Mock<ILogger<OrderService>> _mockLogger;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _mockRepository = new Mock<IOrderRepository>();
        _mockLogger = new Mock<ILogger<OrderService>>();
        _orderService = new OrderService(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllOrdersAsync_ReturnsAllOrders()
    {
        // Arrange
        var orders = new List<Order>
        {
            new Order { Id = Guid.NewGuid(), OrderNumber = "ORD-001", CustomerName = "John Doe" },
            new Order { Id = Guid.NewGuid(), OrderNumber = "ORD-002", CustomerName = "Jane Smith" }
        };
        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(orders);

        // Act
        var result = await _orderService.GetAllOrdersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetOrderByIdAsync_ExistingOrder_ReturnsOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = new Order 
        { 
            Id = orderId, 
            OrderNumber = "ORD-001", 
            CustomerName = "John Doe",
            CustomerEmail = "john@example.com",
            TotalAmount = 100.50m,
            Status = OrderStatus.Pending
        };
        _mockRepository.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);

        // Act
        var result = await _orderService.GetOrderByIdAsync(orderId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(orderId, result.Id);
        Assert.Equal("ORD-001", result.OrderNumber);
        Assert.Equal("John Doe", result.CustomerName);
    }

    [Fact]
    public async Task GetOrderByIdAsync_NonExistingOrder_ReturnsNull()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync((Order?)null);

        // Act
        var result = await _orderService.GetOrderByIdAsync(orderId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateOrderAsync_ValidOrder_ReturnsCreatedOrder()
    {
        // Arrange
        var createDto = new CreateOrderDto
        {
            OrderNumber = "ORD-003",
            CustomerName = "Alice Johnson",
            CustomerEmail = "alice@example.com",
            TotalAmount = 250.00m,
            ShippingAddress = "123 Main St",
            Description = "Test order"
        };

        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Order>()))
            .ReturnsAsync((Order o) => 
            {
                o.Id = Guid.NewGuid();
                return o;
            });

        // Act
        var result = await _orderService.CreateOrderAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("ORD-003", result.OrderNumber);
        Assert.Equal("Alice Johnson", result.CustomerName);
        Assert.Equal("Pending", result.Status);
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Order>()), Times.Once);
    }

    [Fact]
    public async Task UpdateOrderAsync_ExistingOrder_ReturnsUpdatedOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var existingOrder = new Order
        {
            Id = orderId,
            OrderNumber = "ORD-001",
            CustomerName = "John Doe",
            CustomerEmail = "john@example.com",
            TotalAmount = 100.00m,
            Status = OrderStatus.Pending
        };

        var updateDto = new UpdateOrderDto
        {
            CustomerName = "John Updated",
            Status = (int)OrderStatus.Processing
        };

        _mockRepository.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(existingOrder);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Order>())).ReturnsAsync((Order o) => o);

        // Act
        var result = await _orderService.UpdateOrderAsync(orderId, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John Updated", result.CustomerName);
        Assert.Equal("Processing", result.Status);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Order>()), Times.Once);
    }

    [Fact]
    public async Task UpdateOrderAsync_NonExistingOrder_ReturnsNull()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var updateDto = new UpdateOrderDto { CustomerName = "Updated Name" };
        _mockRepository.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync((Order?)null);

        // Act
        var result = await _orderService.UpdateOrderAsync(orderId, updateDto);

        // Assert
        Assert.Null(result);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public async Task DeleteOrderAsync_ExistingOrder_ReturnsTrue()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        _mockRepository.Setup(r => r.DeleteAsync(orderId)).ReturnsAsync(true);

        // Act
        var result = await _orderService.DeleteOrderAsync(orderId);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.DeleteAsync(orderId), Times.Once);
    }

    [Fact]
    public async Task DeleteOrderAsync_NonExistingOrder_ReturnsFalse()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        _mockRepository.Setup(r => r.DeleteAsync(orderId)).ReturnsAsync(false);

        // Act
        var result = await _orderService.DeleteOrderAsync(orderId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetOrdersByStatusAsync_ReturnsFilteredOrders()
    {
        // Arrange
        var orders = new List<Order>
        {
            new Order { Id = Guid.NewGuid(), OrderNumber = "ORD-001", Status = OrderStatus.Pending },
            new Order { Id = Guid.NewGuid(), OrderNumber = "ORD-002", Status = OrderStatus.Pending }
        };
        _mockRepository.Setup(r => r.GetByStatusAsync(OrderStatus.Pending)).ReturnsAsync(orders);

        // Act
        var result = await _orderService.GetOrdersByStatusAsync(OrderStatus.Pending);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, o => Assert.Equal("Pending", o.Status));
    }

    [Fact]
    public async Task GetOrdersByCustomerNameAsync_ReturnsMatchingOrders()
    {
        // Arrange
        var orders = new List<Order>
        {
            new Order { Id = Guid.NewGuid(), OrderNumber = "ORD-001", CustomerName = "John Doe" },
            new Order { Id = Guid.NewGuid(), OrderNumber = "ORD-003", CustomerName = "John Smith" }
        };
        _mockRepository.Setup(r => r.GetByCustomerNameAsync("John")).ReturnsAsync(orders);

        // Act
        var result = await _orderService.GetOrdersByCustomerNameAsync("John");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(r => r.GetByCustomerNameAsync("John"), Times.Once);
    }
}
