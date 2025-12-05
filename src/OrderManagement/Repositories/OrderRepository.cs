using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using OrderManagement.Data;
using OrderManagement.Models;

namespace OrderManagement.Repositories;

/// <summary>
/// Repository for Order CRUD operations with Dataverse.
/// </summary>
public class OrderRepository : IOrderRepository
{
    private readonly IDataverseConnection _dataverseConnection;
    private readonly ILogger<OrderRepository> _logger;

    // Dataverse entity and attribute names for the custom Order entity
    private const string EntityName = "cr_order";
    private const string IdField = "cr_orderid";
    private const string OrderNumberField = "cr_ordernumber";
    private const string CustomerNameField = "cr_customername";
    private const string CustomerEmailField = "cr_customeremail";
    private const string TotalAmountField = "cr_totalamount";
    private const string StatusField = "cr_status";
    private const string ShippingAddressField = "cr_shippingaddress";
    private const string DescriptionField = "cr_description";

    /// <summary>
    /// Initializes a new instance of the OrderRepository class.
    /// </summary>
    public OrderRepository(IDataverseConnection dataverseConnection, ILogger<OrderRepository> logger)
    {
        _dataverseConnection = dataverseConnection ?? throw new ArgumentNullException(nameof(dataverseConnection));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets all orders from Dataverse.
    /// </summary>
    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        try
        {
            _logger.LogInformation("Retrieving all orders from Dataverse");

            var query = new QueryExpression(EntityName)
            {
                ColumnSet = new ColumnSet(true)
            };

            var result = await Task.Run(() => _dataverseConnection.Client.RetrieveMultiple(query));
            
            return result.Entities.Select(MapToOrder).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving orders from Dataverse");
            throw;
        }
    }

    /// <summary>
    /// Gets an order by its ID.
    /// </summary>
    public async Task<Order?> GetByIdAsync(Guid id)
    {
        try
        {
            _logger.LogInformation("Retrieving order {OrderId} from Dataverse", id);

            var entity = await Task.Run(() => 
                _dataverseConnection.Client.Retrieve(EntityName, id, new ColumnSet(true)));

            return entity != null ? MapToOrder(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving order {OrderId} from Dataverse", id);
            return null;
        }
    }

    /// <summary>
    /// Creates a new order in Dataverse.
    /// </summary>
    public async Task<Order> CreateAsync(Order order)
    {
        try
        {
            _logger.LogInformation("Creating new order {OrderNumber} in Dataverse", order.OrderNumber);

            var entity = MapToEntity(order);
            
            var newId = await Task.Run(() => _dataverseConnection.Client.Create(entity));
            
            order.Id = newId;
            order.CreatedOn = DateTime.UtcNow;
            order.ModifiedOn = DateTime.UtcNow;

            _logger.LogInformation("Successfully created order {OrderId} in Dataverse", newId);
            
            return order;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order in Dataverse");
            throw;
        }
    }

    /// <summary>
    /// Updates an existing order in Dataverse.
    /// </summary>
    public async Task<Order> UpdateAsync(Order order)
    {
        try
        {
            _logger.LogInformation("Updating order {OrderId} in Dataverse", order.Id);

            var entity = MapToEntity(order);
            entity.Id = order.Id;

            await Task.Run(() => _dataverseConnection.Client.Update(entity));
            
            order.ModifiedOn = DateTime.UtcNow;

            _logger.LogInformation("Successfully updated order {OrderId} in Dataverse", order.Id);
            
            return order;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order {OrderId} in Dataverse", order.Id);
            throw;
        }
    }

    /// <summary>
    /// Deletes an order from Dataverse.
    /// </summary>
    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            _logger.LogInformation("Deleting order {OrderId} from Dataverse", id);

            await Task.Run(() => _dataverseConnection.Client.Delete(EntityName, id));

            _logger.LogInformation("Successfully deleted order {OrderId} from Dataverse", id);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting order {OrderId} from Dataverse", id);
            return false;
        }
    }

    /// <summary>
    /// Gets orders by status.
    /// </summary>
    public async Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status)
    {
        try
        {
            _logger.LogInformation("Retrieving orders with status {Status} from Dataverse", status);

            var query = new QueryExpression(EntityName)
            {
                ColumnSet = new ColumnSet(true),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression(StatusField, ConditionOperator.Equal, (int)status)
                    }
                }
            };

            var result = await Task.Run(() => _dataverseConnection.Client.RetrieveMultiple(query));
            
            return result.Entities.Select(MapToOrder).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving orders by status from Dataverse");
            throw;
        }
    }

    /// <summary>
    /// Gets orders by customer name.
    /// </summary>
    public async Task<IEnumerable<Order>> GetByCustomerNameAsync(string customerName)
    {
        try
        {
            _logger.LogInformation("Retrieving orders for customer {CustomerName} from Dataverse", customerName);

            var query = new QueryExpression(EntityName)
            {
                ColumnSet = new ColumnSet(true),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression(CustomerNameField, ConditionOperator.Like, $"%{customerName}%")
                    }
                }
            };

            var result = await Task.Run(() => _dataverseConnection.Client.RetrieveMultiple(query));
            
            return result.Entities.Select(MapToOrder).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving orders by customer name from Dataverse");
            throw;
        }
    }

    /// <summary>
    /// Maps a Dataverse entity to an Order model.
    /// </summary>
    private static Order MapToOrder(Entity entity)
    {
        return new Order
        {
            Id = entity.Id,
            OrderNumber = entity.GetAttributeValue<string>(OrderNumberField) ?? string.Empty,
            CustomerName = entity.GetAttributeValue<string>(CustomerNameField) ?? string.Empty,
            CustomerEmail = entity.GetAttributeValue<string>(CustomerEmailField) ?? string.Empty,
            TotalAmount = entity.GetAttributeValue<Money>(TotalAmountField)?.Value ?? 0m,
            Status = (OrderStatus)(entity.GetAttributeValue<OptionSetValue>(StatusField)?.Value ?? 0),
            ShippingAddress = entity.GetAttributeValue<string>(ShippingAddressField) ?? string.Empty,
            Description = entity.GetAttributeValue<string>(DescriptionField) ?? string.Empty,
            CreatedOn = entity.GetAttributeValue<DateTime?>("createdon") ?? DateTime.MinValue,
            ModifiedOn = entity.GetAttributeValue<DateTime?>("modifiedon") ?? DateTime.MinValue
        };
    }

    /// <summary>
    /// Maps an Order model to a Dataverse entity.
    /// </summary>
    private static Entity MapToEntity(Order order)
    {
        var entity = new Entity(EntityName);
        
        entity[OrderNumberField] = order.OrderNumber;
        entity[CustomerNameField] = order.CustomerName;
        entity[CustomerEmailField] = order.CustomerEmail;
        entity[TotalAmountField] = new Money(order.TotalAmount);
        entity[StatusField] = new OptionSetValue((int)order.Status);
        entity[ShippingAddressField] = order.ShippingAddress;
        entity[DescriptionField] = order.Description;

        return entity;
    }
}
