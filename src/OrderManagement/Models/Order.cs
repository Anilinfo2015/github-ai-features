namespace OrderManagement.Models;

/// <summary>
/// Represents an order entity for Dataverse integration.
/// </summary>
public class Order
{
    /// <summary>
    /// Unique identifier for the order (Dataverse GUID).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Order number for reference.
    /// </summary>
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// Customer name for the order.
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Customer email address.
    /// </summary>
    public string CustomerEmail { get; set; } = string.Empty;

    /// <summary>
    /// Total amount for the order.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Status of the order.
    /// </summary>
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    /// <summary>
    /// Date when the order was created.
    /// </summary>
    public DateTime CreatedOn { get; set; }

    /// <summary>
    /// Date when the order was last modified.
    /// </summary>
    public DateTime ModifiedOn { get; set; }

    /// <summary>
    /// Shipping address for the order.
    /// </summary>
    public string ShippingAddress { get; set; } = string.Empty;

    /// <summary>
    /// Description or notes for the order.
    /// </summary>
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Order status enumeration.
/// </summary>
public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Processing = 2,
    Shipped = 3,
    Delivered = 4,
    Cancelled = 5
}
