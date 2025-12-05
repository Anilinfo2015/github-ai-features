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

        // Update only provided fields
        if (!string.IsNullOrEmpty(updateOrderDto.CustomerName))
        {
            existingOrder.CustomerName = updateOrderDto.CustomerName;
        }
        
        if (!string.IsNullOrEmpty(updateOrderDto.CustomerEmail))
        {
            existingOrder.CustomerEmail = updateOrderDto.CustomerEmail;
        }
        
        if (updateOrderDto.TotalAmount.HasValue)
        {
            if (updateOrderDto.TotalAmount.Value <= 0)
            {
                throw new ArgumentException("TotalAmount must be greater than zero", nameof(updateOrderDto));
            }
            existingOrder.TotalAmount = updateOrderDto.TotalAmount.Value;
        }
        
        if (updateOrderDto.Status.HasValue)
        {
            existingOrder.Status = (OrderStatus)updateOrderDto.Status.Value;
        }
        
        if (!string.IsNullOrEmpty(updateOrderDto.ShippingAddress))
        {
            existingOrder.ShippingAddress = updateOrderDto.ShippingAddress;
        }
        
        if (!string.IsNullOrEmpty(updateOrderDto.Description))
        {
            existingOrder.Description = updateOrderDto.Description;
        }

        existingOrder.ModifiedOn = DateTime.UtcNow;

        var updatedOrder = await _orderRepository.UpdateAsync(existingOrder);
        
        _logger.LogInformation("Successfully updated order {OrderId}", id);
        
        return MapToDto(updatedOrder);
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
