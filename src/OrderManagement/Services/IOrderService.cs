using OrderManagement.DTOs;
using OrderManagement.Models;

namespace OrderManagement.Services;

/// <summary>
/// Interface for Order service operations.
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// Gets all orders.
    /// </summary>
    Task<IEnumerable<OrderDto>> GetAllOrdersAsync();

    /// <summary>
    /// Gets an order by its ID.
    /// </summary>
    /// <param name="id">The order ID.</param>
    Task<OrderDto?> GetOrderByIdAsync(Guid id);

    /// <summary>
    /// Creates a new order.
    /// </summary>
    /// <param name="createOrderDto">The order creation data.</param>
    Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto);

    /// <summary>
    /// Updates an existing order.
    /// </summary>
    /// <param name="id">The order ID.</param>
    /// <param name="updateOrderDto">The order update data.</param>
    Task<OrderDto?> UpdateOrderAsync(Guid id, UpdateOrderDto updateOrderDto);

    /// <summary>
    /// Deletes an order.
    /// </summary>
    /// <param name="id">The order ID.</param>
    Task<bool> DeleteOrderAsync(Guid id);

    /// <summary>
    /// Gets orders by status.
    /// </summary>
    /// <param name="status">The order status.</param>
    Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(OrderStatus status);

    /// <summary>
    /// Gets orders by customer name.
    /// </summary>
    /// <param name="customerName">The customer name to search for.</param>
    Task<IEnumerable<OrderDto>> GetOrdersByCustomerNameAsync(string customerName);
}
