using OrderManagement.Models;

namespace OrderManagement.Repositories;

/// <summary>
/// Interface for Order repository operations.
/// </summary>
public interface IOrderRepository
{
    /// <summary>
    /// Gets all orders from Dataverse.
    /// </summary>
    Task<IEnumerable<Order>> GetAllAsync();

    /// <summary>
    /// Gets an order by its ID.
    /// </summary>
    /// <param name="id">The order ID.</param>
    Task<Order?> GetByIdAsync(Guid id);

    /// <summary>
    /// Creates a new order in Dataverse.
    /// </summary>
    /// <param name="order">The order to create.</param>
    Task<Order> CreateAsync(Order order);

    /// <summary>
    /// Updates an existing order in Dataverse.
    /// </summary>
    /// <param name="order">The order to update.</param>
    Task<Order> UpdateAsync(Order order);

    /// <summary>
    /// Deletes an order from Dataverse.
    /// </summary>
    /// <param name="id">The order ID to delete.</param>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// Gets orders by status.
    /// </summary>
    /// <param name="status">The order status to filter by.</param>
    Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status);

    /// <summary>
    /// Gets orders by customer name.
    /// </summary>
    /// <param name="customerName">The customer name to search for.</param>
    Task<IEnumerable<Order>> GetByCustomerNameAsync(string customerName);
}
