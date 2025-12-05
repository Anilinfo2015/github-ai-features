using Microsoft.AspNetCore.Mvc;
using OrderManagement.DTOs;
using OrderManagement.Models;
using OrderManagement.Services;

namespace OrderManagement.Controllers;

/// <summary>
/// REST API controller for Order management operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrdersController> _logger;

    /// <summary>
    /// Initializes a new instance of the OrdersController class.
    /// </summary>
    public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
    {
        _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets all orders.
    /// </summary>
    /// <returns>A list of all orders.</returns>
    /// <response code="200">Returns the list of orders.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders()
    {
        try
        {
            _logger.LogInformation("Getting all orders");
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all orders");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving orders.");
        }
    }

    /// <summary>
    /// Gets an order by ID.
    /// </summary>
    /// <param name="id">The order ID.</param>
    /// <returns>The order with the specified ID.</returns>
    /// <response code="200">Returns the order.</response>
    /// <response code="404">If the order is not found.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OrderDto>> GetOrderById(Guid id)
    {
        try
        {
            _logger.LogInformation("Getting order {OrderId}", id);
            var order = await _orderService.GetOrderByIdAsync(id);

            if (order == null)
            {
                _logger.LogWarning("Order {OrderId} not found", id);
                return NotFound($"Order with ID {id} not found.");
            }

            return Ok(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order {OrderId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the order.");
        }
    }

    /// <summary>
    /// Creates a new order.
    /// </summary>
    /// <param name="createOrderDto">The order creation data.</param>
    /// <returns>The created order.</returns>
    /// <response code="201">Returns the newly created order.</response>
    /// <response code="400">If the request data is invalid.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPost]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto createOrderDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating new order {OrderNumber}", createOrderDto.OrderNumber);
            var createdOrder = await _orderService.CreateOrderAsync(createOrderDto);

            return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.Id }, createdOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the order.");
        }
    }

    /// <summary>
    /// Updates an existing order.
    /// </summary>
    /// <param name="id">The order ID.</param>
    /// <param name="updateOrderDto">The order update data.</param>
    /// <returns>The updated order.</returns>
    /// <response code="200">Returns the updated order.</response>
    /// <response code="400">If the request data is invalid.</response>
    /// <response code="404">If the order is not found.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OrderDto>> UpdateOrder(Guid id, [FromBody] UpdateOrderDto updateOrderDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Updating order {OrderId}", id);
            var updatedOrder = await _orderService.UpdateOrderAsync(id, updateOrderDto);

            if (updatedOrder == null)
            {
                _logger.LogWarning("Order {OrderId} not found for update", id);
                return NotFound($"Order with ID {id} not found.");
            }

            return Ok(updatedOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order {OrderId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the order.");
        }
    }

    /// <summary>
    /// Deletes an order.
    /// </summary>
    /// <param name="id">The order ID.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">If the order was deleted successfully.</response>
    /// <response code="404">If the order is not found.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteOrder(Guid id)
    {
        try
        {
            _logger.LogInformation("Deleting order {OrderId}", id);
            var result = await _orderService.DeleteOrderAsync(id);

            if (!result)
            {
                _logger.LogWarning("Order {OrderId} not found for deletion", id);
                return NotFound($"Order with ID {id} not found.");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting order {OrderId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the order.");
        }
    }

    /// <summary>
    /// Gets orders by status.
    /// </summary>
    /// <param name="status">The order status (0=Pending, 1=Confirmed, 2=Processing, 3=Shipped, 4=Delivered, 5=Cancelled).</param>
    /// <returns>A list of orders with the specified status.</returns>
    /// <response code="200">Returns the list of orders.</response>
    /// <response code="400">If the status value is invalid.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet("status/{status:int}")]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByStatus(int status)
    {
        try
        {
            if (!Enum.IsDefined(typeof(OrderStatus), status))
            {
                return BadRequest($"Invalid status value. Valid values are: {string.Join(", ", Enum.GetNames(typeof(OrderStatus)))}");
            }

            _logger.LogInformation("Getting orders by status {Status}", (OrderStatus)status);
            var orders = await _orderService.GetOrdersByStatusAsync((OrderStatus)status);
            return Ok(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting orders by status {Status}", status);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving orders by status.");
        }
    }

    /// <summary>
    /// Gets orders by customer name.
    /// </summary>
    /// <param name="customerName">The customer name to search for.</param>
    /// <returns>A list of orders matching the customer name.</returns>
    /// <response code="200">Returns the list of orders.</response>
    /// <response code="400">If the customer name is empty.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet("customer/{customerName}")]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByCustomerName(string customerName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(customerName))
            {
                return BadRequest("Customer name cannot be empty.");
            }

            _logger.LogInformation("Getting orders for customer {CustomerName}", customerName);
            var orders = await _orderService.GetOrdersByCustomerNameAsync(customerName);
            return Ok(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting orders for customer {CustomerName}", customerName);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving orders by customer name.");
        }
    }
}
