using OrderManagement.DTOs;
using OrderManagement.Models;
using OrderManagement.Repositories;

namespace OrderManagement.Services;

/// <summary>
/// Service for Order business logic operations.
/// </summary>
public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<OrderService> _logger;

    /// <summary>
    /// Initializes a new instance of the OrderService class.
    /// </summary>
    public OrderService(IOrderRepository orderRepository, ILogger<OrderService> logger)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets all orders.
    /// </summary>
    public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
    {
        _logger.LogInformation("Getting all orders");
        
        var orders = await _orderRepository.GetAllAsync();
        return orders.Select(MapToDto);
    }

    /// <summary>
    /// Gets an order by its ID.
    /// </summary>
    public async Task<OrderDto?> GetOrderByIdAsync(Guid id)
    {
        _logger.LogInformation("Getting order {OrderId}", id);
        
        var order = await _orderRepository.GetByIdAsync(id);
        return order != null ? MapToDto(order) : null;
    }

    /// <summary>
    /// Creates a new order.
    /// </summary>
    public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto)
    {
        _logger.LogInformation("Creating new order {OrderNumber}", createOrderDto.OrderNumber);

        var order = new Order
        {
            OrderNumber = createOrderDto.OrderNumber,
            CustomerName = createOrderDto.CustomerName,
            CustomerEmail = createOrderDto.CustomerEmail,
            TotalAmount = createOrderDto.TotalAmount,
            ShippingAddress = createOrderDto.ShippingAddress,
            Description = createOrderDto.Description,
            Status = OrderStatus.Pending,
            CreatedOn = DateTime.UtcNow,
            ModifiedOn = DateTime.UtcNow
        };

        var createdOrder = await _orderRepository.CreateAsync(order);
        
        _logger.LogInformation("Successfully created order {OrderId}", createdOrder.Id);
        
        return MapToDto(createdOrder);
    }

    /// <summary>
    /// Updates an existing order.
    /// </summary>
    public async Task<OrderDto?> UpdateOrderAsync(Guid id, UpdateOrderDto updateOrderDto)
    {
        _logger.LogInformation("Updating order {OrderId}", id);

        var existingOrder = await _orderRepository.GetByIdAsync(id);
        
        if (existingOrder == null)
        {
            _logger.LogWarning("Order {OrderId} not found for update", id);
            return null;
        }

        // Update only provided fields using helper methods
        UpdateIfProvided(updateOrderDto.CustomerName, v => existingOrder.CustomerName = v);
        UpdateIfProvided(updateOrderDto.CustomerEmail, v => existingOrder.CustomerEmail = v);
        UpdateIfProvided(updateOrderDto.ShippingAddress, v => existingOrder.ShippingAddress = v);
        UpdateIfProvided(updateOrderDto.Description, v => existingOrder.Description = v);
        
        UpdateIfProvided(updateOrderDto.TotalAmount, v =>
        {
            if (v <= 0)
            {
                throw new ArgumentException("TotalAmount must be greater than zero", nameof(updateOrderDto));
            }
            existingOrder.TotalAmount = v;
        });
        
        UpdateIfProvided(updateOrderDto.Status, v =>
        {
            if (!Enum.IsDefined(typeof(OrderStatus), v))
            {
                throw new ArgumentException($"Invalid status value: {v}", nameof(updateOrderDto));
            }
            existingOrder.Status = (OrderStatus)v;
        });

        existingOrder.ModifiedOn = DateTime.UtcNow;

        var updatedOrder = await _orderRepository.UpdateAsync(existingOrder);
        
        _logger.LogInformation("Successfully updated order {OrderId}", id);
        
        return MapToDto(updatedOrder);
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
    /// Deletes an order.
    /// </summary>
    public async Task<bool> DeleteOrderAsync(Guid id)
    {
        _logger.LogInformation("Deleting order {OrderId}", id);
        
        var result = await _orderRepository.DeleteAsync(id);
        
        if (result)
        {
            _logger.LogInformation("Successfully deleted order {OrderId}", id);
        }
        else
        {
            _logger.LogWarning("Failed to delete order {OrderId}", id);
        }
        
        return result;
    }

    /// <summary>
    /// Gets orders by status.
    /// </summary>
    public async Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(OrderStatus status)
    {
        _logger.LogInformation("Getting orders by status {Status}", status);
        
        var orders = await _orderRepository.GetByStatusAsync(status);
        return orders.Select(MapToDto);
    }

    /// <summary>
    /// Gets orders by customer name.
    /// </summary>
    public async Task<IEnumerable<OrderDto>> GetOrdersByCustomerNameAsync(string customerName)
    {
        _logger.LogInformation("Getting orders for customer {CustomerName}", customerName);
        
        var orders = await _orderRepository.GetByCustomerNameAsync(customerName);
        return orders.Select(MapToDto);
    }

    /// <summary>
    /// Maps an Order model to an OrderDto.
    /// </summary>
    private static OrderDto MapToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            CustomerName = order.CustomerName,
            CustomerEmail = order.CustomerEmail,
            TotalAmount = order.TotalAmount,
            Status = order.Status.ToString(),
            CreatedOn = order.CreatedOn,
            ModifiedOn = order.ModifiedOn,
            ShippingAddress = order.ShippingAddress,
            Description = order.Description
        };
    }
}
