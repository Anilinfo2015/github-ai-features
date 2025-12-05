namespace OrderManagement.DTOs;

/// <summary>
/// DTO for creating a new order.
/// </summary>
public class CreateOrderDto
{
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
    /// Shipping address for the order.
    /// </summary>
    public string ShippingAddress { get; set; } = string.Empty;

    /// <summary>
    /// Description or notes for the order.
    /// </summary>
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// DTO for updating an existing order.
/// </summary>
public class UpdateOrderDto
{
    /// <summary>
    /// Customer name for the order.
    /// </summary>
    public string? CustomerName { get; set; }

    /// <summary>
    /// Customer email address.
    /// </summary>
    public string? CustomerEmail { get; set; }

    /// <summary>
    /// Total amount for the order.
    /// </summary>
    public decimal? TotalAmount { get; set; }

    /// <summary>
    /// Status of the order.
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// Shipping address for the order.
    /// </summary>
    public string? ShippingAddress { get; set; }

    /// <summary>
    /// Description or notes for the order.
    /// </summary>
    public string? Description { get; set; }
}

/// <summary>
/// DTO for order response.
/// </summary>
public class OrderDto
{
    /// <summary>
    /// Unique identifier for the order.
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
    public string Status { get; set; } = string.Empty;

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
